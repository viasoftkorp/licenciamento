import { LicensingModes } from "./LicensingModes";

export interface AddNamedUserDialogDataInput {
    licensedTenantId: string;
    licensedBundleId: string;
    licensingMode: LicensingModes;
    numberOfLicenses: number;
    deviceId: string;
    licensedTenantIdentifier: string;
}