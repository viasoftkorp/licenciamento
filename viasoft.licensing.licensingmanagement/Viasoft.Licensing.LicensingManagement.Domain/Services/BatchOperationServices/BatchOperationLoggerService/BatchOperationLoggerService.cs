using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Services.AuditingLogService;
using Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices.Consts;
using Viasoft.Licensing.LicensingManagement.Domain.Services.UserDetails;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices.BatchOperationLoggerService
{
    public class BatchOperationLoggerService: IBatchOperationLoggerService, ITransientDependency
    {
        private readonly IUserDetailsService _userDetailsServiceService;
        private readonly IAuditingLogService _auditingLogService;

        public BatchOperationLoggerService(IUserDetailsService userDetailsServiceService, IAuditingLogService auditingLogService)
        {
            _userDetailsServiceService = userDetailsServiceService;
            _auditingLogService = auditingLogService;
        }

        public async Task LogInsertAppsInBundles(List<string> appIdentifiers, List<string> bundleIdentifiers)
        {
            var aggregatedAppsToDetails = appIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var aggregatedBundlesToDetails = bundleIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.InsertAppsInBundles, aggregatedAppsToDetails, aggregatedBundlesToDetails);
            await InsertBatchOperationLog(LogAction.InsertMultipleAppsInMultipleBundles, logDetails, ActionNameConsts.InsertAppsInBundlesActionName);
        }

        public async Task LogInsertAppsFromBundlesInLicenses(List<string> appIdentifiers, List<Guid> licenseTenantIdentifiers)
        {
            var aggregatedAppsToDetails = appIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var aggregatedLicensedTenantToDetails = licenseTenantIdentifiers.Select(guid => guid.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.InsertAppsFromBundlesInLicenses, aggregatedAppsToDetails, aggregatedLicensedTenantToDetails);
            await InsertBatchOperationLog(LogAction.InsertMultipleAppsFromBundlesInLicenses, logDetails, ActionNameConsts.InsertAppsFromBundlesInLicensesActionName);
        }

        public async Task LogRemoveAppFromBundleInLicenses(string appIdentifier, List<Guid> licenseTenantIdentifiers)
        {
            var identifiersToDetails = licenseTenantIdentifiers.Select(guid => guid.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.RemoveAppInLicensesDetails, appIdentifier, identifiersToDetails);
            await InsertBatchOperationLog(LogAction.RemoveAppInLicenses, logDetails, ActionNameConsts.RemovedAppInLicensesActionName);
        }

        public async Task LogInsertBundleInLicenses(int licensesNumber, List<string> bundleIdentifiers, List<Guid> licenseTenantIdentifiers)
        {
            var aggregatedBundlesToDetails = bundleIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var aggregatedLicensedTenantToDetails = licenseTenantIdentifiers.Select(guid => guid.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.InsertBundlesInLicensesDetails, aggregatedBundlesToDetails, licensesNumber, aggregatedLicensedTenantToDetails);
            await InsertBatchOperationLog(LogAction.InsertMultipleBundlesInMultipleLicenses, logDetails, ActionNameConsts.InsertBundlesInLicensesActionName);
        }

        public async Task LogAppsInLicenses(int licensesNumber, List<string> appIdentifiers, List<Guid> licenseTenantIdentifiers)
        {
            var aggregatedAppsToDetails = appIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var aggregatedLicensedTenantToDetails = licenseTenantIdentifiers.Select(guid => guid.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.InsertAppsInLicensesDetails, aggregatedAppsToDetails, licensesNumber, aggregatedLicensedTenantToDetails);
            await InsertBatchOperationLog(LogAction.InsertMultipleAppsInMultipleLicenses, logDetails, ActionNameConsts.InsertAppsInLicensesActionName);
        }
        
        private async Task InsertBatchOperationLog(LogAction action, string logDetails, string actionName)
        {
            var user = await _userDetailsServiceService.GetUserDetailsAsync();
            var log = new AuditingLog
            {
                Action = action,
                Type = LogType.BatchAction,
                DateTime = DateTime.UtcNow,
                UserName = user.UserName,
                UserId = user.UserId,
                ActionName = actionName,
                Details = logDetails
            };
            await _auditingLogService.InsertLogs(log);
        }
    }
}