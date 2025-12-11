using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings
{
    public class ServerDeployOutput
    {
        public ServerDeployOutput(Guid tenantId, string token, string version, 
            DateTime? deployDateTime, bool completed)
        {
            TenantId = tenantId;
            Token = token;
            Version = version;
            DeployDateTime = deployDateTime;
            Completed = completed;
        }

        public Guid TenantId { get; set; }
        public string Token { get; set; }
        public string Version { get; set; }
        public DateTime? DeployDateTime { get; set; }
        public bool Completed { get; set; }
    }
}