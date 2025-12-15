using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Shared.Attributes;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.Consul;

namespace Viasoft.Licensing.LicenseServer.Shared.Consul
{
    public class ConsulSettingsProvider: IConsulSettingsProvider, ISingletonDependency
    {
        private ConsulSettings _consulSettings;
        private readonly IConfiguration _configuration;

        public ConsulSettingsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ConsulSettings GetSettingsFromConsul()
        {
            if (_consulSettings == null)
                throw new Exception("ConsulSettings is null");

            return _consulSettings;
        }

        public void LoadSettingsFromConsul()
        {
            
            if (_consulSettings != null)
                return;
            
            var authority = _configuration["Authentication:Authority"];
            var customerLicensingSecret = _configuration["CustomerLicensingSecret"];
            var licensingManagementSecret = _configuration["LicensingManagementSecret"];
            
            if (string.IsNullOrEmpty(authority) || string.IsNullOrEmpty(licensingManagementSecret) || string.IsNullOrEmpty(customerLicensingSecret))
            {
                throw new Exception("Missing consul settings, these settings should be present in the build/publish commands");
            }

            _consulSettings = new ConsulSettings
            {
                Authentication = new ConsulAuthentication
                {
                    Authority = authority,
                    CustomerLicensingSecret = customerLicensingSecret,
                    LicensingManagementSecret = licensingManagementSecret
                }
            };
        }
    }
} 