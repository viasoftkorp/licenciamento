using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicensingManager;

namespace Viasoft.Licensing.LicenseServer.Domain.Services;

public interface ILicensingManagerServiceFactory
{
    ILicensingManagerService CreateLicensingManagerService(LicenseByTenantId license);
}