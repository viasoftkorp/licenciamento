using System.Text.Json.Serialization;

namespace Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration
{
    public class AutoUpdateSettings
    {
        public string UpdateTime { get; set; }
        [JsonIgnore]
        public string AppCastUrl { get; set; }
        [JsonIgnore]
        public string PublicKey { get; set; }
        [JsonIgnore]
        public bool SkipAutoUpdate { get; set; }
    }
}