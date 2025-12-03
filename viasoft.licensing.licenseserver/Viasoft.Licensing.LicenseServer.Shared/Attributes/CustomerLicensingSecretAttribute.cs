using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CustomerLicensingSecretAttribute: Attribute
    {
        public string CustomerLicensingSecret { get; }

        public CustomerLicensingSecretAttribute(string customerLicensingSecret)
        {
            CustomerLicensingSecret = customerLicensingSecret;
        }
    }
}