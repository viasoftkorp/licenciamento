namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment
{
    public enum UpdateEnvironmentOutputStatus
    {
        NameConflict,
        NotFound,
        InProductionConflict,
        NotTypedEnvironment,
        InvalidDatabaseName,
        Ok
    }
}