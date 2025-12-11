import { DataLivelyUpdate } from "@viasoft/common";
import { LicensingStatus } from "@viasoft/licensing-licensingmanagement-licensedtenant";
import { CurrentSagaStatus } from "@viasoft/licensing-management/app/tokens/enum/current-saga-status.enum";
import { LicensedTenantSagaInfo } from "@viasoft/licensing-management/app/tokens/interfaces/licensed-tenant-saga-info.interface";

export class LicensedTenantSagaStatusUpdateNotification implements DataLivelyUpdate {
    uniqueTypeName: string;
    amCreatingNewLicensedTenant: boolean;
    status: CurrentSagaStatus;
    currentLicensedTenantStatus: LicensingStatus;
    newEmail: string;
    currentTick: number;
}