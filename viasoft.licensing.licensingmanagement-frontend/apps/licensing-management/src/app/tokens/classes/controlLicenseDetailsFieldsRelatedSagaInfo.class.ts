import { Subject } from "rxjs";
import { LicensedTenantSagaStatusUpdateNotification } from "../../pages/licensings/license-detail/notifications/licensed-tenant-saga-status-update-notification";
import { CurrentSagaStatus } from "../enum/current-saga-status.enum";
import { LicensedTenantSagaInfo } from "../interfaces/licensed-tenant-saga-info.interface";

export class ControlLicenseDetailsFieldsRelatedSagaInfo {
    readonly tooltipPrefix = "licensings.tooltipForSagaInfo.";
    readonly icons = {
        Processing: "fas fa-ellipsis-stroke",
        CompletedSuccessfully: "fas fa-circle-check",
        CompletedWithFailure: "far fa-circle-xmark"
    };
    readonly tooltips = {
        status: {
            processing: this.tooltipPrefix + "status.processing",
            failure: this.tooltipPrefix + "status.failure"
        },
        email: {
            success: this.tooltipPrefix + "email.success",
            processing: this.tooltipPrefix + "email.processing",
            failure: this.tooltipPrefix + "email.failure"
        }
    };

    lastNotificationTick: number = null;
    emailSuffixIcon: string;
    emailSuffixIconColor: string;
    disableStatusSubject: Subject<boolean> = new Subject();
    statusTooltip: string;
    emailTooltip: string;

    setValuesAccordingLicensedTenantSagaInfo(sagaInfo: LicensedTenantSagaInfo){
        if(sagaInfo.status == CurrentSagaStatus.CompletedSuccessfully){
            this.emailSuffixIcon = this.icons.CompletedSuccessfully;
            this.disableStatusSubject.next(false);
            this.statusTooltip = '';
            this.emailTooltip = this.tooltips.email.success;
            this.emailSuffixIconColor = "";
        }else if (sagaInfo.status == CurrentSagaStatus.Processing){
            this.emailSuffixIcon = this.icons.Processing;
            this.disableStatusSubject.next(sagaInfo.amCreatingNewLicensedTenant);
            this.statusTooltip = sagaInfo.amCreatingNewLicensedTenant ? this.tooltips.status.processing : '';
            this.emailTooltip = this.tooltips.email.processing;
            this.emailSuffixIconColor = "";
        }else {
            this.emailSuffixIcon = this.icons.CompletedWithFailure;
            this.disableStatusSubject.next(sagaInfo.amCreatingNewLicensedTenant);
            this.statusTooltip = sagaInfo.amCreatingNewLicensedTenant ? this.tooltips.status.failure : '';
            this.emailTooltip = this.tooltips.email.failure;
            this.emailSuffixIconColor = "color: #ff0000";
        }
    }

    //este método é necessário pois as notifications podem chegar no front com uma ordem diferente da que foi enviada pelo back-end
    shouldUpdateValues(sagaUpdateNotification: LicensedTenantSagaStatusUpdateNotification){
        if(this.lastNotificationTick == null || this.lastNotificationTick < sagaUpdateNotification.currentTick ||
            (this.lastNotificationTick == sagaUpdateNotification.currentTick && sagaUpdateNotification.status != CurrentSagaStatus.Processing)){
            this.lastNotificationTick = sagaUpdateNotification.currentTick;
            return true;
        }
        return false;
    }

    setInitialStatusTooltip(){
        this.statusTooltip = this.tooltips.status.processing;
    }
}
