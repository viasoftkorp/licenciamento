import {Component, Inject, OnDestroy, OnInit, Optional} from '@angular/core';
import {FormControl, FormGroup} from "@angular/forms";
import {
  LicenseDetailLicensesServerService
} from "@viasoft/licensing-management/app/pages/licensings/license-detail/license-detail-licenses-server/license-detail-licenses-server.service";
import {VsSubscriptionManager} from "@viasoft/common";
import {
  LicensedTenantSettingsOutput
} from "@viasoft/licensing-management/clients/licensing-management/model/licensedTenantSettingsOutput";
import {
  LicenseDetailComponent
} from "@viasoft/licensing-management/app/pages/licensings/license-detail/license-detail.component";
import {
  LicensedTenantSettingsInfoComponent
} from "@viasoft/licensing-management/app/tokens/modals/licensed-tenant-settings-info/licensed-tenant-settings-info.component";
import {MatDialog} from "@angular/material/dialog";
import {
  LicensedTenantSettingsKeys
} from "@viasoft/licensing-management/app/tokens/classes/licensedTenantSettingsKeys.class";

@Component({
  selector: 'app-license-detail-licenses-server',
  templateUrl: './license-detail-licenses-server.component.html',
  styleUrls: ['./license-detail-licenses-server.component.scss']
})
export class LicenseDetailLicensesServerComponent implements OnInit, OnDestroy {
  public formGroup: FormGroup;
  public useSimpleHardwareIdKey: string = LicensedTenantSettingsKeys.UseSimpleHardwareId;
  private subscriptions: VsSubscriptionManager = new VsSubscriptionManager();

  constructor(
    @Optional() @Inject(LicenseDetailComponent) public licenseDetail: LicenseDetailComponent,
    private readonly matDialog: MatDialog,
    private readonly licenseDetailLicensesServerService: LicenseDetailLicensesServerService
  ) {}

  ngOnInit(): void {
    if (this.licenseDetail.identifierReady) {
      this.initialSubscriptions();
    } else {
      this.subscriptions.add('licensingIdentifierReadySubject',
        this.licenseDetailLicensesServerService.licensingIdentifierReadySubject.subscribe(
        () => {
          this.initialSubscriptions();
        }
      ));
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.clear();
  }

  public showLicensedTenantSettingsInfo(type: string): void {
    this.matDialog.open(LicensedTenantSettingsInfoComponent, {
      data: {
        infoType: type
      },
      maxWidth: '30vw'
    });
  }

  public preventDefault(event: MouseEvent): void {
    event.preventDefault();
    event.stopImmediatePropagation();
  }

  private initialSubscriptions(): void {
    this.subscriptions.add('getLicensedTenantSettings',
      this.licenseDetailLicensesServerService.getLicensedTenantSettings(this.licenseDetail.licenseIdentifier)
        .subscribe((licensedTenantSettings: LicensedTenantSettingsOutput) => {
          this.licenseDetail.licensedTenantSettingsInitialValues = licensedTenantSettings;
          this.createForm(licensedTenantSettings);
        }));
  }

  private createForm(licensedTenantSettings: LicensedTenantSettingsOutput): void {
    this.formGroup = new FormGroup({
      useSimpleHardwareId: new FormControl(licensedTenantSettings.value.toLowerCase() === 'true')
    });

    this.formValueChangedSubscription();
  }

  private formValueChangedSubscription(): void {
    this.subscriptions.add('licensesServerFormValueChanged',
      this.formGroup.valueChanges.subscribe((formValue: any) => {
        this.licenseDetailLicensesServerService.licensedTenantSettingsValueChanged.next(formValue);
      }));
  }
}
