namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Organization
{
    public class UpdateOrganizationOutput
    {
        public Organization Organization { get; set; }
        public UpdateOrganizationOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}