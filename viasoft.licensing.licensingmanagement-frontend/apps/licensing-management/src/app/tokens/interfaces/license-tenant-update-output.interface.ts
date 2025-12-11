import { LicenseConsumeType } from '../enum/license-consume-type.enum';
import { LicensingStatus } from '../enum/licensing-status.enum';
import { OperationValidation } from '../enum/operation-validation.enum';

export interface LicenseTenantUpdateOutput {
    id: string;
    accountId: string;
    accountName: string;
    status: LicensingStatus;
    identifier: string;
    expirationDateTime: Date;
    licensedCnpjs: string;
    administratorEmail: string;
    licenseConsumeType: LicenseConsumeType;
    operationValidation: OperationValidation;
    operationValidationDescription: string;
    notes: string;
    hardwareId: string;
}
