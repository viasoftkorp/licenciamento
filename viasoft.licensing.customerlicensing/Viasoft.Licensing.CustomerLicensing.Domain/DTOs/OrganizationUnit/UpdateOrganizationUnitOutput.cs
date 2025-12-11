namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit
{
    public class UpdateOrganizationUnitOutput
    {
        public OrganizationUnitOutput Unit { get; set; }
        public UpdateOrganizationUnitOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}