import { OperationValidation } from '../enum/operation-validation.enum';

export interface NamedUserOutput {
    id: string;
    tenantId: string;
    licensedTenantId: string;
    namedUserId: string;
    namedUserEmail: string;
    deviceId: string;
    operationValidation: OperationValidation;
}
