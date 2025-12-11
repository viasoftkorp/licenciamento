namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment
{
    public enum UpdateEnvironmentOutputStatus
    {
        NameConflict = 0,
        NotFound = 1,
        InProductionConflict = 2,
        NotTypedEnvironment = 3,
        InvalidDatabaseName = 4,
        Ok = 5
    }
}