import { LicenseConsumeType } from '../enum/license-consume-type.enum';
import { LicensingStatus } from '../enum/licensing-status.enum';

export interface LicenseTenantUpdateInput {
    id: string;
    accountId: string;
    status: LicensingStatus;
    identifier: string;
    expirationDateTime: Date;
    licenseConsumeType: LicenseConsumeType;
    licensedCnpjs: string;
    administratorEmail: string;
    notes: string;
    hardwareId: string;
}
