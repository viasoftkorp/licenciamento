using System.Collections.Generic;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings
{
    public class GetSettingsOutput
    {
        public GetSettingsOutput(string gatewayAddress, List<DeployVersion> deployVersions)
        {
            GatewayAddress = gatewayAddress;
            DeployVersions = deployVersions;
        }
        public string GatewayAddress {get; set; }
        public List<DeployVersion> DeployVersions {get; set; }
    }
}