using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LicensingManagementSecretAttribute: Attribute
    {
        public string LicensingManagementSecret { get; }

        public LicensingManagementSecretAttribute(string licensingManagementSecret)
        {
            LicensingManagementSecret = licensingManagementSecret;
        }
    }
}