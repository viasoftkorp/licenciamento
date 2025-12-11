using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.Settings
{
    public interface ISettingsService
    {
        Task<GetSettingsOutput> GetSettings(Guid tenantId);
        Task<InfrastructureConfigurationUpdateOutput> PutSettings(InfrastructureConfigurationUpdateInput input);
        Task<GetDeployCommandByVersionOutput> GetDeployCommandByVersion(string version);
        Task<GetUpdateVersionCommandByVersionOutput> GetDeployCommandByUpdateVersion(string version);
        Task<GetUninstallCommandByVersionOutput> GetDeployCommandByUninstallVersion(string version);
    }
}