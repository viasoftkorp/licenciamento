namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings
{
    public class GetDeployCommandByVersionOutput
    {
        public string DeployCommand { get; set; }

        public GetDeployCommandByVersionOutput(string deployCommand)
        {
            DeployCommand = deployCommand;
        }
    }
}