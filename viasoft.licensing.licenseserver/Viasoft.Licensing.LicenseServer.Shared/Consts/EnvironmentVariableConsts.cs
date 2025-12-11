namespace Viasoft.Licensing.LicenseServer.Shared.Consts
{
    public static class EnvironmentVariableConsts
    {
        public static string TenantLegacyDatabaseMappingConfiguration => "TENANT_LEGACY_DATABASE_MAPPING_CONFIGURATION";
        public static string LicenseUsageBehaviourUploadFrequencyInDays => "LICENSE_USAGE_BEHAVIOUR_UPLOAD_FREQUENCY_IN_DAYS";
        public static string LicenseUsageInRealTimeUploadFrequencyInMinutes => "LICENSE_USAGE_IN_REAL_TIME_UPLOAD_FREQUENCY_IN_MINUTES";
        public static string MinimumAllowedHeartbeatInSeconds => "MINIMUM_ALLOWED_HEARTBEAT_IN_SECONDS";
        public static string HttpPort => "PORTA_HTTP";
        public static string UrlGateway => "URL_GATEWAY";
        public static string AspNetCoreEnvironment => "ASPNETCORE_ENVIRONMENT";
        public static string LegacyWithBroker => "LEGACY_WITH_BROKER";
    }
}