using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.Identity.Abstractions;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.CustomerLicensing.Domain.Consts;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Domain;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;
using Viasoft.Licensing.CustomerLicensing.Domain.Events;
using Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.Account;
using Viasoft.Licensing.CustomerLicensing.Domain.Messages;
using Z.EntityFramework.Plus;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageInRealTime
{
    public class LicenseUsageInRealTimeService: ILicenseUsageInRealTimeService, ITransientDependency
    {
        private readonly IRepository<Entities.LicenseUsageInRealTime> _licenseUsageInRealTime;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceBus _serviceBus;
        private readonly ICurrentUser _currentUser;
        private readonly IAccountService _accountService;
        private readonly IRepository<OwnedAppCount> _ownedAppCount;
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly ILogger<LicenseUsageInRealTimeService> _logger;

        public LicenseUsageInRealTimeService(IRepository<Entities.LicenseUsageInRealTime> licenseUsageInRealTime, IUnitOfWork unitOfWork, 
            IMapper mapper, IServiceBus serviceBus, ICurrentUser currentUser, IAccountService accountService, IRepository<OwnedAppCount> ownedAppCount, 
            IApiClientCallBuilder apiClientCallBuilder, ILogger<LicenseUsageInRealTimeService> logger)
        {
            _licenseUsageInRealTime = licenseUsageInRealTime;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceBus = serviceBus;
            _currentUser = currentUser;
            _accountService = accountService;
            _ownedAppCount = ownedAppCount;
            _apiClientCallBuilder = apiClientCallBuilder;
            _logger = logger;
        }

        public async Task UpdateLicensedApps(LicensingDetailsUpdated licensingDetails)
        {
            var howManyLicensedAppsToInsert = licensingDetails.LicenseByIdentifier.LicensedTenantDetails.OwnedApps.Count;

            var counterToUpdate =
                await _ownedAppCount.FirstOrDefaultAsync(c => c.LicensingIdentifier == licensingDetails.TenantId);
            if (counterToUpdate != null)
            {
                counterToUpdate.Count = howManyLicensedAppsToInsert;
                await _ownedAppCount.UpdateAsync(counterToUpdate, true);
                
                return;
            }

            await _ownedAppCount.InsertAsync(new OwnedAppCount
            {
                LicensingIdentifier = licensingDetails.TenantId,
                Count = howManyLicensedAppsToInsert
            }, true);
        }

        public async Task ImportLicenseUsage(LicenseUsageInRealTimeImportInput input)
        {
            _logger.LogInformation("Received license usage in real time for tenant {0}, the license server reported {1} apps", input.TenantId, input.LicenseUsageInRealTimeDetails.Count);

            _currentUser.Id = Guid.Parse("16a7571e-7ff6-479e-a6f5-3514414179dc");
            
            var accountInfoTask = _accountService.GetAccountNameFromLicensingIdentifier(input.TenantId);

            var licenseUsagesToInsert = _mapper.Map<List<Entities.LicenseUsageInRealTime>>(input.LicenseUsageInRealTimeDetails);

            var appIdentifiers = licenseUsagesToInsert.Select(x => x.AppIdentifier).ToList();
            
            var domainsTaskInput = new DomainsFromAppsIdsInput
            {
                AppsIds = appIdentifiers,
                LicensingIdentifier = input.TenantId
            };

            var domainsTask = GetDomainsFromAppsIds(domainsTaskInput);

            var lastUpdate = DateTime.UtcNow;
            foreach (var licenseUsage in licenseUsagesToInsert)
                licenseUsage.LastUpdate = lastUpdate;

            var appsInUse = new List<string>();
            
            //quando a licença é do tipo nomeado offline, o consumo da licença ocorre somente no login do app e não há refresh
            //então somente virá para cá uma vez o registro desse consumo
            //para mantermos um registro na grid mostrando o "visto por último" não apagamos os registros que são LicensingModels.Named e LicensingModes.Offline
            foreach (var licenseUsageDetails in input.LicenseUsageInRealTimeDetails)
            {
                if (licenseUsageDetails.LicensingModel == LicensingModels.Named &&
                    licenseUsageDetails.LicensingMode == LicensingModes.Offline)
                {
                    appsInUse.Add(licenseUsageDetails.AppIdentifier);    
                }
            }
            
            using (_unitOfWork.Begin(options => options.LazyTransactionInitiation = false))
            {
                await _licenseUsageInRealTime.Where(e => e.LicensingIdentifier == input.TenantId && input.SoftwareUtilized.Contains(e.SoftwareIdentifier)
                                                         && ((e.LicensingModel == LicensingModels.Floating || (e.LicensingModel == LicensingModels.Named && e.LicensingMode == LicensingModes.Online))
                                                             || (e.LicensingModel == LicensingModels.Named
                                                                 && e.LicensingMode == LicensingModes.Offline
                                                                 && appsInUse.Contains(e.AppIdentifier))))
                    .DeleteAsync();
                await _unitOfWork.CompleteAsync();
            }
            
            var accountInfo = await accountInfoTask;

            var domains = await domainsTask;
            
            foreach (var licenseUsage in licenseUsagesToInsert)
            {
                if (domains.ContainsKey(licenseUsage.AppIdentifier))
                {
                    licenseUsage.Domain = domains[licenseUsage.AppIdentifier];
                }
            }
            
            var insertedEvent = new LicenseUsageInRealTimeInserted
            {
                AccountId = accountInfo.AccountId,
                AccountName = accountInfo.AccountName,
                LicensingIdentifier = input.TenantId,
                InsertedLicensesUsages = _mapper.Map<List<InsertedLicenseUsageInRealTime>>(licenseUsagesToInsert)
            };

            using (_unitOfWork.Begin())
            {
                foreach (var licenseUsage in licenseUsagesToInsert)
                {
                    licenseUsage.AccountId = accountInfo.AccountId;
                    licenseUsage.AccountName = accountInfo.AccountName;
                    await _licenseUsageInRealTime.InsertAsync(licenseUsage); 
                }
                
                await _serviceBus.Publish(insertedEvent);

                await _unitOfWork.CompleteAsync();
            }
        }

        private async Task<Dictionary<string, Domains>> GetDomainsFromAppsIds(DomainsFromAppsIdsInput input)
        {
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(ExternalServicesConsts.LicensingManagement.DomainController.GetDomainsFromAppIds)
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithBody(input)
                .WithHttpMethod(HttpMethod.Post)
                
                .Build();

            var gatewayCallResponse = await gatewayCall.ResponseCallAsync<Dictionary<string, Domains>>();

            return gatewayCallResponse;
        }

        public async Task<Dictionary<string, int>> GetLicensesConsumed(Guid licensingIdentifier, List<string> bundleIdentifiers, List<string> appIdentifiers)
        {
            var bundleLicenseUsage = (await _licenseUsageInRealTime
                    .Where(lu =>
                        lu.LicensingIdentifier == licensingIdentifier &&
                        bundleIdentifiers.Contains(lu.BundleIdentifier))
                    .ToListAsync())
                .GroupBy(e => e.BundleIdentifier)
                .ToDictionary(e => e.Key, e => e.ToList().First().AppLicensesConsumed);

            var appLicenseUsage = (await _licenseUsageInRealTime
                    .Where(lu =>
                        lu.LicensingIdentifier == licensingIdentifier &&
                        appIdentifiers.Contains(lu.AppIdentifier))
                    .ToListAsync())
                .GroupBy(e => e.AppIdentifier)
                .ToDictionary(e => e.Key, e => e.ToList().First().AppLicensesConsumed);
            
            return bundleLicenseUsage.Concat(appLicenseUsage)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}