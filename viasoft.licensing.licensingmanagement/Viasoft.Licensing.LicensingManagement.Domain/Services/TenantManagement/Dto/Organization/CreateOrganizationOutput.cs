namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Organization
{
    public class CreateOrganizationOutput
    {
        public Organization Organization { get; set; }
        public CreateOrganizationOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}