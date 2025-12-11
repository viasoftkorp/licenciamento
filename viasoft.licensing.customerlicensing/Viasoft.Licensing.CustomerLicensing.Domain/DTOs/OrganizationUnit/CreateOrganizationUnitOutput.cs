namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit
{
    public class CreateOrganizationUnitOutput
    {
        public OrganizationUnitOutput Unit { get; set; }
        public CreateOrganizationUnitOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}