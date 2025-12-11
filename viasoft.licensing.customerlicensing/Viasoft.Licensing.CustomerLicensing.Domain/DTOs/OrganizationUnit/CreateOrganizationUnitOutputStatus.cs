namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit
{
    public enum CreateOrganizationUnitOutputStatus
    {
        OrganizationNotFound = 0,
        NameConflict = 1,
        Ok = 2,
        IdConflict = 3
    }
}