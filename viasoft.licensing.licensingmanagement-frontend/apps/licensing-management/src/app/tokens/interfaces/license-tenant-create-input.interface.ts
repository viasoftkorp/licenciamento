import { LicenseConsumeType } from '../enum/license-consume-type.enum';
import { LicensingStatus } from '../enum/licensing-status.enum';

export interface LicenseTenantCreateInput {
    id: string;
    accountId: string;
    status: LicensingStatus;
    identifier: string;
    licenseConsumeType: LicenseConsumeType;
    expirationDateTime: Date;
    licensedCnpjs: string;
    administratorEmail: string;
    notes: string;
    bundleIds: string[];
    numberOfLicenses: number;
    hardwareId: string;
    desktopDatabaseName: string;
    accountName: string;
    numberOfDaysToExpiration: number;
    gatewayAddress: string;
}
