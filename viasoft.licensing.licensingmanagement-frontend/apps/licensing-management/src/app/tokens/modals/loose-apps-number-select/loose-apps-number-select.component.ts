import { Component, Inject, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppSelectService } from '../../services/modals-service/app-select.service';
import { LicensingsService } from '@viasoft/licensing-management/app/pages/licensings/licensings.service';
import { LicensedAppUpdateInput } from '@viasoft/licensing-management/clients/licensing-management';
import { VsSubscriptionManager } from '@viasoft/common';
import { LicensingModel } from '../../enum/licensing-model.enum';
import { LicensingMode } from '../../enum/licensing-mode.enum';
import { VsSelectOption } from '@viasoft/components';
import { validateMinDate } from '../../utils/validate-min-date.function';

const temporaryLicensesValidator = (form: FormGroup): { [key: string]: boolean } | null => {
  if (form.get('numberOfLicenses').value <= 0) {
    return { 'numberOfLicensesIsRequired': true };
  }
  return null;
}

@Component({
  selector: 'app-loose-apps-number-select',
  templateUrl: './loose-apps-number-select.component.html',
  styleUrls: ['./loose-apps-number-select.component.scss']
})
export class LooseAppsNumberSelectComponent implements OnInit, OnDestroy {

  public licensingModeDisabled = !this.data?.licensingModel;
  form: FormGroup;
  licensedApps: Array<string> = [];
  isDisabled: boolean;
  hasUpdatePermission = false;
  private subs = new VsSubscriptionManager();
  public optionsStatusSelect: VsSelectOption[] = [
    { name: 'licensings.blocked', value: 0 },
    { name: 'licensings.active', value: 1 },
  ];

  constructor(private dialogRef: MatDialogRef<LooseAppsNumberSelectComponent>,
              private appselect: AppSelectService,
              private licensingService: LicensingsService,
              @Inject(MAT_DIALOG_DATA) public data: any,
              private changeDetectorRef: ChangeDetectorRef,
              private readonly formBuilder: FormBuilder) {
    if (this.data) {
      this.hasUpdatePermission = this.data.hasUpdatePermission;
    }
  }

  ngOnInit() {
    this.createForm();
    if (this.data && !this.data.listOfDomains) {
      this.isDisabled = true;
      this.form.get('appName').setValue(this.data.name);
      this.form.get('appId').setValue(this.data.appId);
      this.form.get('numberOfLicenses').setValue(this.data.numberOfLicenses);
      this.form.get('licensingMode').setValue(this.data.licensingMode);
      this.form.get('licensingModel').setValue(this.data.licensingModel);
      this.form.get('expirationDateTime').setValue(this.data.expirationDateTime);
      this.form.get('status').setValue(this.data.status);
    }
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  createForm() {
    if (this.data && !this.data.listOfDomains) {
      this.isDisabled = true;
      this.form = this.formBuilder.group({
        numberOfLicenses: this.formBuilder.control(this.data.numberOfLicenses, [Validators.required, Validators.min(0)]),
        appName: this.formBuilder.control(this.data.name),
        appId: this.formBuilder.control(this.data.appId, [Validators.required]),
        licensingModel: this.formBuilder.control(Boolean(this.data.licensingModel), []),
        licensingMode: this.formBuilder.control(Boolean(this.data.licensingMode), []),
        expirationDateTime: this.formBuilder.control(this.data.expirationDateTime, [validateMinDate]),
        status: this.formBuilder.control(this.data.status, [])
      }, {
        validators: [temporaryLicensesValidator]
      });
      // NECESSARRY BECAUSE CUSTOM VALIDATOR CAUSE CHANGE DETECTION
      this.changeDetectorRef.detectChanges();
    } else {
      this.form = this.formBuilder.group({
        numberOfLicenses: this.formBuilder.control(1, [Validators.required, Validators.min(0)]),
        appName: this.formBuilder.control(''),
        appId: this.formBuilder.control('', [Validators.required]),
        licensingModel: this.formBuilder.control(false, []),
        licensingMode: this.formBuilder.control(false, []),
        expirationDateTime: this.formBuilder.control('', [validateMinDate]),
        status: this.formBuilder.control('', [])
      }, {
        validators: [temporaryLicensesValidator]
      });
    }
    if (this.data && !this.hasUpdatePermission) {
      this.setFormDisabled();
    }

    if (!this.form.get('licensingModel').value) {
      this.form.get('licensingMode').disable();
    } else {
      this.form.get('licensingMode').enable();
    }

    this.subs.add(
      'value-changes',
      this.form.get('licensingModel').valueChanges.subscribe(value => {
        if (value) {
          this.form.get('licensingMode').enable();
        } else {
          this.form.get('licensingMode').setValue(false);
          this.form.get('licensingMode').disable();
        }
    }));
  }


  setFormDisabled() {
    this.form.disable();
  }

  onAppSearch() {
    this.subs.add(
      'get-all',
      this.licensingService.getAllLicensedApps(this.licensingService.licensedTenant.LicensedTenantId)
      .subscribe((licensedApps) => {
        for (const app of licensedApps.items) {
          this.licensedApps.push(app.id);
        }
        this.subs.add(
          'dialog',
          this.appselect.openDialog({ licensedApps: this.licensedApps, listOfDomains: this.data.listOfDomains })
          .subscribe((app) => {
            if (app) {
              this.form.get('appName').setValue(app.name);
              this.form.get('appId').setValue(app.id);
            }
            this.licensedApps = [];
          }));
      }));
  }

  cancel() {
    this.dialogRef.close();
  }

  save() {
    const licensingModel = this.form.get('licensingModel').value ? LicensingModel.Named : LicensingModel.Floating;
    let licensingMode = null;

    if (Boolean(licensingModel)) {
      licensingMode = this.form.get('licensingMode').value ? LicensingMode.Offline : LicensingMode.Online;
    }
    const appToLicense = {
      licensedTenantId: this.licensingService.licensedTenant.LicensedTenantId,
      numberOfLicenses: this.form.get('numberOfLicenses').value,
      additionalNumberOfLicenses: 0,
      status: this.form.get('status').value,
      appId: this.form.get('appId').value,
      numberOfTemporaryLicenses: 0,
      expirationDateOfTemporaryLicenses: null,
      licensingMode,
      licensingModel,
      expirationDateTime: this.form.get('expirationDateTime').value
    } as LicensedAppUpdateInput;
    this.dialogRef.close(appToLicense);
  }

}
