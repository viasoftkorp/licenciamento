namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings
{
    public class GetUpdateVersionCommandByVersionOutput
    {
        public string UpdateVersionCommand { get; set; }

        public GetUpdateVersionCommandByVersionOutput(string updateVersionCommand)
        {
            UpdateVersionCommand = updateVersionCommand;
        }
    }
}