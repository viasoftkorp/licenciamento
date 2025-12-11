using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Viasoft.Core.Caching.DistributedCache;
using Viasoft.Core.DDD.Application.Dto.BaseCrud;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.Const;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO;
using Viasoft.Licensing.LicensingManagement.Host.Extensions;

namespace Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.Service
{
    public class InfrastructureConfigurationService: IInfrastructureConfigurationService, ITransientDependency
    {
        private readonly IRepository<Entities.InfrastructureConfiguration> _infrastructureConfigurations;
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly IMapper _mapper;
        private readonly IDistributedCacheService _distributedCacheService;
        
        public InfrastructureConfigurationService(IRepository<Entities.InfrastructureConfiguration> infrastructureConfigurations, IMapper mapper, IDistributedCacheService distributedCacheService, IRepository<Entities.LicensedTenant> licensedTenants)
        {
            _infrastructureConfigurations = infrastructureConfigurations;
            _mapper = mapper;
            _distributedCacheService = distributedCacheService;
            _licensedTenants = licensedTenants;
        }

        public async Task<InfrastructureConfigurationCreateOutput> CreateAsync(InfrastructureConfigurationCreateInput input)
        {
            if (await InfrastructureConfigurationAlreadyExistsAsync(input))
            {
                return FailedConfigurationCreateOutput();
            }
            
            var entity = _mapper.Map<Entities.InfrastructureConfiguration>(input);
            
            var output = await _infrastructureConfigurations.InsertAsync(entity, true);
            
            var createOutput = _mapper.Map<InfrastructureConfigurationCreateOutput>(output);
            createOutput.Success = true;
            return createOutput;
        }

        private async Task<bool> InfrastructureConfigurationAlreadyExistsAsync(InfrastructureConfigurationCreateInput input)
        {
            return await _infrastructureConfigurations.AnyAsync(i => i.LicensedTenantId == input.LicensedTenantId);
        }

        public async Task<InfrastructureConfigurationDeleteOutput> DeleteAsync(Guid tenantId)
        {
            var entityToDelete = await _infrastructureConfigurations.FirstOrDefaultAsync(i => i.LicensedTenantId == tenantId);
            if (entityToDelete == null)
            {
                return new InfrastructureConfigurationDeleteOutput
                {
                    Success = false
                };
            }

            await _infrastructureConfigurations.DeleteAsync(entityToDelete.Id, true);
            
            return new InfrastructureConfigurationDeleteOutput
            {
                Success = true
            };
        }

        public async Task<InfrastructureConfigurationUpdateOutput> UpdateAsync(InfrastructureConfigurationUpdateInput input)
        {
            if (!GatewayAddressRegexValidation(input.GatewayAddress))
            {
                return FailedConfigurationUpdateOutput();
            }

            input.GatewayAddress = input.GatewayAddress.EnsureNotTrailingSlash();
            var entity = await _infrastructureConfigurations.FirstOrDefaultAsync(i => i.LicensedTenantId == input.LicensedTenantId);
            _mapper.Map(input, entity);
            var updatedEntity = await _infrastructureConfigurations.UpdateAsync(entity, true);
            var identifier = await _licensedTenants.Select(t => new {t.Identifier, t.Id}).FirstAsync(t => t.Id == input.LicensedTenantId);
            
            await _distributedCacheService.RemoveAsync(InfrastructureConfigurationConsts.CacheKey,
                new TenantDistributedCacheKeyStrategy(identifier.Identifier));
            
            var output = _mapper.Map<InfrastructureConfigurationUpdateOutput>(updatedEntity);
            output.Success = true;
            return output;
        }

        public async Task<InfrastructureConfigurationCreateOutput> GetByTenantIdAsync(Guid tenantId)
        {
            var cache = await GetInfrastructureConfigurationCache(tenantId);
            if (cache != null) return JsonConvert.DeserializeObject<InfrastructureConfigurationCreateOutput>(Encoding.UTF8.GetString(cache));
            
            var licensedTenantId = await _licensedTenants.Select(t => new {t.Identifier, t.Id})
                .FirstAsync(t => t.Identifier == tenantId);
            var entity = await _infrastructureConfigurations.FirstOrDefaultAsync(i => i.LicensedTenantId == licensedTenantId.Id);

            var output = _mapper.Map<InfrastructureConfigurationCreateOutput>(entity);
            await SetInfrastructureConfigurationCache(new InfrastructureConfigurationCache
            {
                GatewayAddress = output.GatewayAddress,
                DesktopDatabaseName = output.DesktopDatabaseName,
                LicensedTenantId = output.LicensedTenantId
            }, tenantId);
            return output;
        }

        public async Task<List<Guid>> GetTenantIdListAsync()
        {
            var tenantIds = await _infrastructureConfigurations.Select(i => i.LicensedTenantId).ToListAsync();
            return tenantIds;
        }

        private InfrastructureConfigurationCreateOutput FailedConfigurationCreateOutput()
            {
                return new InfrastructureConfigurationCreateOutput
                {
                    Errors = new List<BaseCrudResponseError<OperationValidation>>
                    {
                        new BaseCrudResponseError<OperationValidation>
                        {
                            ErrorCode = OperationValidation.InfrastructureConfigurationAlreadyExists,
                            Message = nameof(OperationValidation.InfrastructureConfigurationAlreadyExists)
                        }
                    }
                };
            }

            private InfrastructureConfigurationUpdateOutput FailedConfigurationUpdateOutput()
            {
                return new InfrastructureConfigurationUpdateOutput
                {
                    Success = false,
                    Errors = new List<BaseCrudResponseError<OperationValidation>>
                    {
                        new BaseCrudResponseError<OperationValidation>
                        {
                            ErrorCode = OperationValidation.InvalidGateway,
                            Message = nameof(OperationValidation.InvalidGateway)
                        }
                    }
                };
            }

            private async Task<byte[]> GetInfrastructureConfigurationCache(Guid tenantId)
            {
                var cache = await _distributedCacheService.GetAsync(InfrastructureConfigurationConsts.CacheKey, new TenantDistributedCacheKeyStrategy(tenantId));
                return cache;
            }

            private async Task SetInfrastructureConfigurationCache(InfrastructureConfigurationCache configuration, Guid tenantId)
            {
                var cacheBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(configuration));
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
                await _distributedCacheService.SetAsync(InfrastructureConfigurationConsts.CacheKey, cacheBytes, options, new TenantDistributedCacheKeyStrategy(tenantId));
            }

            private bool GatewayAddressRegexValidation(string gatewayAddress)
            {
                if (string.IsNullOrEmpty(gatewayAddress))
                {
                    return true;
                }
                var regex = new Regex(@"((([0-9]{1,3}\.){3}[0-9])|([-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,12}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?))(\:\d{2,4}|)");
                return regex.IsMatch(gatewayAddress);
            }
        }
    }