namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.OrganizationUnit
{
    public class CreateOrganizationUnitOutput
    {
        public OrganizationUnit Unit { get; set; }
        public CreateOrganizationUnitOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}