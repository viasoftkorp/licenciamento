import { UserBehaviourStatus } from "./UserBehaviourStatus";

export interface licenseUserBehaviourNamedOnlineOutput { 
    licensingIdentifier?: string;
    appIdentifier?: string | null;
    appName?: string | null;
    bundleIdentifier?: string | null;
    bundleName?: string | null;
    softwareName?: string | null;
    softwareIdentifier?: string | null;
    user?: string | null;
    softwareVersion?: string | null;
    hostName?: string | null;
    hostUser?: string | null;
    localIpAddress?: string | null;
    language?: string | null;
    osInfo?: string | null;
    browserInfo?: string | null;
    databaseName?: string | null;
    startTime?: Date;
    lastUpdate?: Date;
    accessDuration?: string;
    readonly accessDurationFormatted?: string | null;
    status: UserBehaviourStatus | null;
}
