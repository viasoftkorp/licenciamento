namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.OrganizationUnit
{
    public class UpdateOrganizationUnitOutput
    {
        public OrganizationUnit Unit { get; set; }
        public UpdateOrganizationUnitOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}