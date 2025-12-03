using System;
using Viasoft.Licensing.LicenseServer.Shared.Enums;

namespace Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant
 {
     public enum LicensingStatus
     {
         NeedsApproval = 0,
         Blocked = 1,
         Trial = 2,
         Active = 3,
         ReadOnly = 4
     }

     public static class LicensingStatusExtension
     {
         public static TenantLicenseStatus GetTenantLicenseStatusFromLicensingStatus(this LicensingStatus licensingStatus) =>
             licensingStatus switch
             {
                 LicensingStatus.NeedsApproval => TenantLicenseStatus.NeedsApproval,
                 LicensingStatus.Blocked       => TenantLicenseStatus.Blocked,
                 LicensingStatus.Trial         => TenantLicenseStatus.Trial,
                 LicensingStatus.Active        => TenantLicenseStatus.Active,
                 LicensingStatus.ReadOnly => TenantLicenseStatus.ReadOnly,
                 _                             => throw new ArgumentException("Invalid enum value", nameof(licensingStatus)),
             };
     }

 }