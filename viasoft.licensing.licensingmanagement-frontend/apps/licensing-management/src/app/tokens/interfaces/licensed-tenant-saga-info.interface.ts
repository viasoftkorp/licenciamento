import { CurrentSagaStatus } from "../enum/current-saga-status.enum";
import { LicensingStatus } from "../enum/licensing-status.enum";

export interface LicensedTenantSagaInfo {
    amCreatingNewLicensedTenant: boolean;
    status: CurrentSagaStatus;
}