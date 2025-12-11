namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings
{
    public class DeployVersion
    {
        public const string Key = "DeployVersions";
        public string Version { get; set; }
        public string AppendToCommand { get; set; }
    }
}