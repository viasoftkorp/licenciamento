namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings
{
    public class GetUninstallCommandByVersionOutput
    {
        public string UninstallVersionCommand { get; set; }

        public GetUninstallCommandByVersionOutput(string uninstallVersionCommand)
        {
            UninstallVersionCommand = uninstallVersionCommand;
        }
    }
}