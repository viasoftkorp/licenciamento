namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment
{
    public enum CreateEnvironmentOutputStatus
    {
        NameConflict,
        OrganizationUnitNotFound,
        InProductionConflict,
        NotTypedEnvironment,
        InvalidDatabaseName,
        Ok
    }
}