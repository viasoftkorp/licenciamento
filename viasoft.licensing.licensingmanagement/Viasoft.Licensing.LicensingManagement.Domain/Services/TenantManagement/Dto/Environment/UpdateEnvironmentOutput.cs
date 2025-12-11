namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment
{
    public class UpdateEnvironmentOutput
    {
        public OrganizationUnitEnvironment Environment { get; set; }
        public UpdateEnvironmentOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}