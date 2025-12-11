using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Events;


namespace Viasoft.Licensing.LicensingManagement.Domain.Services.NamedUserBundleLicense
{
    public class NamedUserBundleLicenseService: INamedUserBundleLicenseService, ITransientDependency
    {
        private readonly IRepository<Entities.NamedUserBundleLicense> _namedUserBundleLicenses;
        private readonly ICurrentTenant _currentTenant;
        private readonly IServiceBus _serviceBus;

        public NamedUserBundleLicenseService(IRepository<Entities.NamedUserBundleLicense> namedUserBundleLicenses, ICurrentTenant currentTenant, IServiceBus serviceBus)
        {
            _namedUserBundleLicenses = namedUserBundleLicenses;
            _currentTenant = currentTenant;
            _serviceBus = serviceBus;
        }

        public async Task<GetNamedUserFromBundleOutput> GetNamedUserFromBundle(Entities.LicensedTenant licensedTenant,
            Entities.LicensedBundle licensedBundle, GetAllNamedUserBundleInput input)
        {
            var query = _namedUserBundleLicenses.Select(n => new NamedUserBundleLicenseOutput
            {
                Id = n.Id,
                DeviceId = n.DeviceId,
                TenantId = n.TenantId,
                LicensedBundleId = n.LicensedBundleId,
                LicensedTenantId = n.LicensedTenantId,
                NamedUserEmail = n.NamedUserEmail,
                NamedUserId = n.NamedUserId
            }).Where(n => n.LicensedBundleId == licensedBundle.Id && n.LicensedTenantId == licensedTenant.Id);

            if (!string.IsNullOrEmpty(input.Sorting))
                query = query.ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting);
            else
            {
                var (defaultSorting, ascSorting) = DefaultGetAllSorting();
                query = query.ApplyAdvancedFilter(input.AdvancedFilter, defaultSorting, ascSorting);
            }

            var totalCount = await query.CountAsync();

            var result = await AdvancedFilterBasicOperations.PageBy(query, input.SkipCount, input.MaxResultCount).ToListAsync();

            return new GetNamedUserFromBundleOutput
            {
                NamedUserBundleLicenseOutputs = new PagedResultDto<NamedUserBundleLicenseOutput>
                {
                    Items = result,
                    TotalCount = totalCount
                },
                NamedUserFromBundleValidationCode = GetNamedUserFromBundleValidationCode.NoError
            };
        }

        public async Task<NamedUserBundleLicenseOutput> AddNamedUserToLicensedBundle(Entities.LicensedTenant licensedTenant, Entities.LicensedBundle licensedBundle, CreateNamedUserBundleLicenseInput input)
        {
            var namedBundleLicenses = await _namedUserBundleLicenses.Where(b => b.LicensedBundleId == licensedBundle.Id && b.LicensedTenantId == licensedTenant.Id).ToListAsync();

            if (namedBundleLicenses.Count >= licensedBundle.NumberOfLicenses)
                return new NamedUserBundleLicenseOutput
                {
                    OperationValidation = OperationValidation.TooManyNamedUserBundleLicenses
                };

            var namedBundleLicenseByEmail = await _namedUserBundleLicenses.FirstOrDefaultAsync(b => b.NamedUserEmail == input.NamedUserEmail && b.LicensedBundleId == licensedBundle.Id && b.LicensedTenantId == licensedTenant.Id);

            if (namedBundleLicenseByEmail != null)
                return new NamedUserBundleLicenseOutput
                {
                    OperationValidation = OperationValidation.NamedUserEmailAlreadyInUse
                };
                
            var entity = new Entities.NamedUserBundleLicense
            {
                Id = Guid.NewGuid(),
                DeviceId = input.DeviceId,
                TenantId = _currentTenant.Id,
                LicensedBundleId = licensedBundle.Id,
                NamedUserId = input.NamedUserId,
                NamedUserEmail = input.NamedUserEmail,
                LicensedTenantId = licensedTenant.Id
            };
            
            var output = await _namedUserBundleLicenses.InsertAsync(entity);
            return NamedUserBundleLicenseOutput.ConstructFromEntity(output);
        }

        public async Task<UpdateNamedUsersFromBundleOutput> UpdateNamedUsersFromBundle(
            Entities.LicensedTenant licensedTenant, Entities.LicensedBundle licensedBundle,
            UpdateNamedUserBundleLicenseInput input, Guid namedUserId)
        {
            var entity = await _namedUserBundleLicenses.FirstOrDefaultAsync(b => b.Id == namedUserId && b.LicensedTenantId == licensedTenant.Id && b.LicensedBundleId == licensedBundle.Id);

            if (entity == null)
                return new UpdateNamedUsersFromBundleOutput
                {
                    Success = false,
                    ValidationCode = UpdateNamedUsersFromBundleValidationCode.NoNamedUser
                };

            var namedUserByEmail = await _namedUserBundleLicenses.FirstOrDefaultAsync(b =>
                b.NamedUserEmail == input.NamedUserEmail && b.Id != entity.Id && b.LicensedTenantId == licensedTenant.Id && b.LicensedBundleId == licensedBundle.Id);

            if (namedUserByEmail != null)
                return new UpdateNamedUsersFromBundleOutput
                {
                    Success = false,
                    ValidationCode = UpdateNamedUsersFromBundleValidationCode.NamedUserEmailAlreadyInUse
                };

            entity.DeviceId = input.DeviceId;
            entity.NamedUserEmail = input.NamedUserEmail;
            entity.NamedUserId = input.NamedUserId;

            await _namedUserBundleLicenses.UpdateAsync(entity);
            
            return new UpdateNamedUsersFromBundleOutput
            {
                Success = true,
                ValidationCode = UpdateNamedUsersFromBundleValidationCode.NoError
            };
        }

        public async Task<RemoveNamedUserFromBundleOutput> RemoveNamedUserFromBundle(
            Entities.LicensedTenant licensedTenant, Entities.LicensedBundle licensedBundle, Guid namedUserId)
        {
            var namedUserBundleLicense = await _namedUserBundleLicenses.FirstOrDefaultAsync(b =>
                b.Id == namedUserId && b.LicensedTenantId == licensedTenant.Id &&
                b.LicensedBundleId == licensedBundle.Id);

            if (namedUserBundleLicense == null)
                return new RemoveNamedUserFromBundleOutput
                {
                    Success = false,
                    ValidationCode = RemoveNamedUserFromBundleValidationCode.NoNamedUser
                };

            await _namedUserBundleLicenses.DeleteAsync(namedUserBundleLicense);

            var namedUserRemoved = new NamedUserRemoved()
            {
                LicensingIdentifier = licensedTenant.Identifier,
                NamedUserEmail = namedUserBundleLicense.NamedUserEmail
            };

            await _serviceBus.Publish(namedUserRemoved);
            
            return new RemoveNamedUserFromBundleOutput
            {
                Success = true,
                ValidationCode = RemoveNamedUserFromBundleValidationCode.NoError
            };
        }
        
        private static (Expression<Func<NamedUserBundleLicenseOutput, string>>, bool) DefaultGetAllSorting()
        {
            return (b => b.NamedUserEmail, true);
        }
    }
}