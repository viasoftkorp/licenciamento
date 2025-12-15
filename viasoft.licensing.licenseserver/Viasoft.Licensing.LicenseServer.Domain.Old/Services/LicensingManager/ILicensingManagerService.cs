using System;
using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Shared.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensingManager
{
    public interface ILicensingManagerService
    {
        ConsumeLicenseOutputOld ConsumeLicense(ConsumeLicenseInput input);
        
        ReleaseLicenseOutputOld ReleaseLicense(ReleaseLicenseInput input);

        LicenseTenantStatusCurrentOld GetCurrentState();

        void EvaluateAndReleaseLicensesBasedOnHeartbeat();
        
        RefreshAppLicenseInUseByUserOutputOld RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInputOld inputOld);

        TenantLicensedAppsOutput GetTenantLicensedApps();
        
        TenantLicenseStatus GetTenantLicenseStatus();
        
        bool IsTenantCnpjLicensed(string cnpj);

        DateTime GetLastUpdatedDateTime();

        IEnumerable<AppLicenseConsumer> GetAllLicensesInUse();

        void RestoreLicensesInUse(IEnumerable<AppLicenseConsumer> licensesInUse);
    }
}