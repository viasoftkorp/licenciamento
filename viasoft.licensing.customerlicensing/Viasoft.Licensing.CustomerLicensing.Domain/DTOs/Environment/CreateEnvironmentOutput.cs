namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment
{
    public class CreateEnvironmentOutput
    {
        public OrganizationUnitEnvironmentOutput Environment { get; set; }
        public CreateEnvironmentOutputStatus Status { get; set; }
        public string StatusMessage => Status.ToString();
    }
}