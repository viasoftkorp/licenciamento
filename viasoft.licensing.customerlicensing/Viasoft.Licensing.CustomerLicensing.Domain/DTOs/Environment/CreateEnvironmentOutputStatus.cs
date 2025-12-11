namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment
{
    public enum CreateEnvironmentOutputStatus
    {
        NameConflict = 0,
        OrganizationUnitNotFound = 1,
        InProductionConflict = 2,
        NotTypedEnvironment = 3,
        InvalidDatabaseName = 4,
        Ok = 5,
        IdConflict = 6
    }
}