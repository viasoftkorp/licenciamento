using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;
using Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class LicensedTenantBatchOperationsController: BaseController
    {
        private readonly ILicensedTenantBatchOperationsService _licensedTenantBatchOperationsService;
        
        public LicensedTenantBatchOperationsController(ILicensedTenantBatchOperationsService licensedTenantBatchOperationsService)
        {
            _licensedTenantBatchOperationsService = licensedTenantBatchOperationsService;
        }
        
        [HttpPost]
        [Authorize(Policy.InsertAppInLicenses)]
        public async Task InsertNewAppFromBundleInLicenses(BundledAppCreateInput input)
        {
            var appsByBundles = new List<LicensedBundleApp> {new LicensedBundleApp {AppId = input.AppId, BundleId = input.BundleId}};
            await _licensedTenantBatchOperationsService.InsertAppsFromBundlesInLicenses(appsByBundles);
        }

        [HttpPost]
        [Authorize(Policy.RemoveAppInLicenses)]
        public async Task RemoveAppFromBundleInLicenses(BundledAppDeleteInput input)
        {
            await _licensedTenantBatchOperationsService.RemoveAppFromBundleInLicenses(input.BundleId, input.AppId);
        }
        
        [HttpPost]
        [Authorize(Policy.InsertBundlesInLicenses)]
        public async Task InsertBundlesInLicenses(BatchOperationsInput input)
        {
            await _licensedTenantBatchOperationsService.InsertBundlesInLicenses(input);
        }
        
        [HttpPost]
        [Authorize(Policy.InsertAppsInLicenses)]
        public async Task InsertAppsInLicenses(BatchOperationsInput input)
        {
            await _licensedTenantBatchOperationsService.InsertAppsInLicenses(input);
        }
        
        [HttpPost]
        [Authorize(Policy.InsertAppsInBundles)]
        public async Task<List<LicensedBundleApp>> InsertAppsInBundles(BatchOperationsInput input)
        {
            return await _licensedTenantBatchOperationsService.InsertAppsInBundles(input);
        }
        
        [HttpPost]
        [Authorize(Policy.InsertAppsInBundles)]
        public async Task InsertAppsFromBundlesInLicenses(List<LicensedBundleApp> input)
        {
            await _licensedTenantBatchOperationsService.InsertAppsFromBundlesInLicenses(input);
        }
        
    }
}