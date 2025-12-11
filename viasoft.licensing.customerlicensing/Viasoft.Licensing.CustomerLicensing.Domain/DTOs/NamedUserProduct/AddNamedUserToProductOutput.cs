using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserApp;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;
using Viasoft.Licensing.CustomerLicensing.Domain.Extensions;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserProduct
{
    public class AddNamedUserToProductOutput
    {
        public AddNamedUserToProductValidationCode ValidationCode { get; set; }
        public NamedUserProductLicenseOutput Output { get; set; }

        public static AddNamedUserToProductOutput FromAddNamedUserToLicensedAppOutput(AddNamedUserToLicensedAppOutput licensedAppOutput)
        {
            return new()
            {
                ValidationCode = licensedAppOutput.ValidationCode.ToAddNamedUserToProductValidationCode(),
                Output = licensedAppOutput.Output == null ? new NamedUserProductLicenseOutput() : new NamedUserProductLicenseOutput()
                {
                    Id = licensedAppOutput.Output.Id,
                    DeviceId = licensedAppOutput.Output.DeviceId,
                    TenantId = licensedAppOutput.Output.TenantId,
                    ProductId = licensedAppOutput.Output.LicensedAppId,
                    LicensedTenantId = licensedAppOutput.Output.LicensedTenantId,
                    NamedUserEmail = licensedAppOutput.Output.NamedUserEmail,
                    NamedUserId = licensedAppOutput.Output.NamedUserId
                }
            };
        }
        
        public static AddNamedUserToProductOutput FromAddNamedUserToLicensedBundleOutput(NamedUserBundleLicenseOutput licensedBundleOutput)
        {
            return new()
            {
                ValidationCode = licensedBundleOutput.OperationValidation.ToAddNamedUserBundleToProductValidationCode(),
                Output = new NamedUserProductLicenseOutput
                {
                    Id = licensedBundleOutput.Id,
                    DeviceId = licensedBundleOutput.DeviceId,
                    TenantId = licensedBundleOutput.TenantId,
                    ProductId = licensedBundleOutput.LicensedBundleId,
                    LicensedTenantId = licensedBundleOutput.LicensedTenantId,
                    NamedUserEmail = licensedBundleOutput.NamedUserEmail,
                    NamedUserId = licensedBundleOutput.NamedUserId
                }
            };
        }
    }
}