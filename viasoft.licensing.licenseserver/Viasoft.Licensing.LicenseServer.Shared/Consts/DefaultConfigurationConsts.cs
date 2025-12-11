namespace Viasoft.Licensing.LicenseServer.Shared.Consts
{
    public static class DefaultConfigurationConsts
    {
        public static int MinimumAllowedHeartbeatInSeconds => 150; // 2 minutes 30 seconds
        public static int LicenseUsageBehaviourUploadFrequencyInDays => 1;
        public static int LicenseUsageInRealTimeUploadFrequencyInMinutes => 1;
        public static bool IsRunningAsLegacy { get; set; }
        
        public static bool IsRunningAsLegacyWithBroker { get; set; }
    }
}