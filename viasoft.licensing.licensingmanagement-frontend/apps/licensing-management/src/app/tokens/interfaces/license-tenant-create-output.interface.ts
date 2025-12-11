import { LicenseConsumeType } from '../enum/license-consume-type.enum';
import { LicensingStatus } from '../enum/licensing-status.enum';
import { OperationValidation } from '../enum/operation-validation.enum';
import { LicensedTenantSagaInfo } from './licensed-tenant-saga-info.interface';

export interface LicenseTenantCreateOutput {
    id: string;
    accountId: string;
    accountName: string;
    status: LicensingStatus;
    identifier: string;
    expirationDateTime: Date;
    licensedCnpjs: string;
    administratorEmail: string;
    licenseConsumeType: LicenseConsumeType;
    licenseConsumeDescription: string;
    notes: string;
    hardwareId: string;
    operationValidation: OperationValidation;
    operationValidationDescription: string;
    sagaInfo: LicensedTenantSagaInfo;
}
