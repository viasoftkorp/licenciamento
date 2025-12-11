using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant.Store;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings;
using Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.LicensingManagement;
using Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.TenantManagement;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.Settings
{
    public class SettingsService : ISettingsService, ITransientDependency
    {
        private readonly ILicensingManagementApiCaller _licensingManagementApiCaller;
        private readonly ITenantManagementApiCaller _tenantManagementApiCaller;
        private readonly ITenancyStore _tenancyStore;
        private readonly IConfiguration _configuration;

        public SettingsService(ILicensingManagementApiCaller licensingManagementApiCaller,
            ITenantManagementApiCaller tenantManagementApiCaller, IConfiguration configurantion,
            ITenancyStore tenancyStore)
        {
            _licensingManagementApiCaller = licensingManagementApiCaller;
            _tenantManagementApiCaller = tenantManagementApiCaller;
            _configuration = configurantion;
            _tenancyStore = tenancyStore;
        }

        public async Task<GetSettingsOutput> GetSettings(Guid tenantId)
        {
            var result = await _tenancyStore.GetInfrastructureConfigurationAsync(tenantId);
            var deployVersions = GetDeployVersions();

            return new GetSettingsOutput(result.GatewayAddress, deployVersions);
        }

        public async Task<InfrastructureConfigurationUpdateOutput> PutSettings(
            InfrastructureConfigurationUpdateInput input)
        {
            var result = await _licensingManagementApiCaller.UpdateInfrastructureConfiguration(input);
            var updateOutput = await result.GetResponse();
            return updateOutput;
        }

        public async Task<GetDeployCommandByVersionOutput> GetDeployCommandByVersion(string version)
        {
            var token = await GetTokenByVersion(version);
            var baseDeployCommand = _configuration["DeployCommand"].Trim();
            
            var deployVersions = GetDeployVersions();
            var appendToCommand = deployVersions.FirstOrDefault(x => x.Version == version)?.AppendToCommand ?? string.Empty;

            baseDeployCommand = ConfigureCommandReleaseBranch(baseDeployCommand, version);
            var deployCommand = $"{baseDeployCommand} token={token} {appendToCommand}";

            return new GetDeployCommandByVersionOutput(deployCommand);
        }

        public async Task<GetUpdateVersionCommandByVersionOutput> GetDeployCommandByUpdateVersion(string version)
        {
            var token = await GetTokenByVersion(version);
            var baseUpdateVersionCommand = _configuration["UpdateVersionCommand"].Trim();
            
            var deployVersions = GetDeployVersions();
            var appendToCommand = deployVersions.FirstOrDefault(x => x.Version == version)?.AppendToCommand ?? string.Empty;

            baseUpdateVersionCommand = ConfigureCommandReleaseBranch(baseUpdateVersionCommand, version);
            var updateVersionCommand = $"{baseUpdateVersionCommand} token={token} {appendToCommand}";

            return new GetUpdateVersionCommandByVersionOutput(updateVersionCommand);
        }

        public async Task<GetUninstallCommandByVersionOutput> GetDeployCommandByUninstallVersion(string version)
        {
            var token = await GetTokenByVersion(version);
            var baseUninstallVersionCommand = _configuration["UninstallVersionCommand"].Trim();
            
            var deployVersions = GetDeployVersions();
            var appendToCommand = deployVersions.FirstOrDefault(x => x.Version == version)?.AppendToCommand ?? string.Empty;
            
            baseUninstallVersionCommand = ConfigureCommandReleaseBranch(baseUninstallVersionCommand, version);
            var uninstallVersionCommand = $"{baseUninstallVersionCommand} token={token} removed_version={version} {appendToCommand}";

            return new GetUninstallCommandByVersionOutput(uninstallVersionCommand);
        }

        private async Task<string> GetTokenByVersion(string version)
        {
            var result = await _tenantManagementApiCaller.GetServerByVersion(version);
            var serverDeployOutput = await result.GetResponse();
            return serverDeployOutput.Token;
        }

        private string ConfigureCommandReleaseBranch(string command, string version)
        {
            return string.Format(command, $"release/{version}.x");
        }
        private List<DeployVersion> GetDeployVersions()
        {
            var deployVersions = new List<DeployVersion>();
            _configuration.GetSection(DeployVersion.Key).Bind(deployVersions);
            return deployVersions;
        }
    }
}