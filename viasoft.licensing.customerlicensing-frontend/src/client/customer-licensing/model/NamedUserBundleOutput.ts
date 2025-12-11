import { OperationValidation } from "./operationValidation";

export interface NamedUserBundleOutput {
    id: string | null;
    tenantId: string | null;
    licensedTenantId: string | null;
    licensedBundleId: string | null;
    namedUserId: string | null;
    namedUserEmail: string | null;
    deviceId: string | null;
    OperationValidation: OperationValidation | null;
}