using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Repositories;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicensingManager;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Services;

public class LicensingManagerServiceFactory: ILicensingManagerServiceFactory, ITransientDependency
{
    private readonly ILicenseServerRepository _licenseServerRepository;
    private readonly IConfiguration _configuration;
    private readonly IProvideHardwareIdService _provideHardwareIdService;
    private readonly IExternalLicensingManagementService _externalLicensingManagementService;
    private readonly ILogger<LicenseTenantStatusCurrent> _loggerLicenseTenantStatusCurrent;

    public LicensingManagerServiceFactory(ILicenseServerRepository licenseServerRepository, IConfiguration configuration, 
        IProvideHardwareIdService provideHardwareIdService, IExternalLicensingManagementService externalLicensingManagementService, 
        ILogger<LicenseTenantStatusCurrent> loggerLicenseTenantStatusCurrent)
    {
        _licenseServerRepository = licenseServerRepository;
        _configuration = configuration;
        _provideHardwareIdService = provideHardwareIdService;
        _externalLicensingManagementService = externalLicensingManagementService;
        _loggerLicenseTenantStatusCurrent = loggerLicenseTenantStatusCurrent;
    }

    public ILicensingManagerService CreateLicensingManagerService(LicenseByTenantId license)
    {
        var licenseTenantStatusCurrent = new LicenseTenantStatusCurrent(license, _provideHardwareIdService, 
            _externalLicensingManagementService, _loggerLicenseTenantStatusCurrent);

        return new LicensingManagerService(licenseTenantStatusCurrent, _configuration, _licenseServerRepository);
    }
}