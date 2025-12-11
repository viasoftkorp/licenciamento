namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment
{
    public class CreateEnvironmentOutput
    {
        public OrganizationUnitEnvironment Environment { get; set; }
        public CreateEnvironmentOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}