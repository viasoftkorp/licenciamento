namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment
{
    public class UpdateEnvironmentOutput
    {
        public OrganizationUnitEnvironmentOutput Environment { get; set; }
        public UpdateEnvironmentOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}