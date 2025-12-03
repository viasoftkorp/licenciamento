using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.Classes;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers
{
    public class LicenseConsumersToRelease
    {
        public string Token { get; set; }
        public List<AppLicenseConsumer> Consumers { get; set; }
    }
}