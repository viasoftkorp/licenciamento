import { Component, Inject, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { LicensingsService } from '@viasoft/licensing-management/app/pages/licensings/licensings.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { VsGridOptions, VsSelectOption } from '@viasoft/components';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { LicenseDetailBundleSelectService } from '../../services/modals-service/license-detail-bundle-select.service';
import { BundlesService } from '@viasoft/licensing-management/app/pages/bundles/bundles.service';
import { LicensedBundleUpdateInput } from '../../inputs/licensed-bundle-update.input';
import { LicensedBundleStatus } from '../../enum/licensed-bundle-status.enum';
import { LicensingMode } from '../../enum/licensing-mode.enum';
import { LicensingModel } from '../../enum/licensing-model.enum';
import { VsSubscriptionManager } from '@viasoft/common';
import { validateMinDate } from '../../utils/validate-min-date.function';

const temporaryLicensesValidator = (form: FormGroup): { [key: string]: boolean } | null => {
  if (form.get('numberOfLicenses').value <= 0) {
    return { 'numberOfLicensesIsRequired': true };
  }
  return null;
}

@Component({
  selector: 'app-licenses-number-select',
  templateUrl: './licenses-number-select.component.html',
  styleUrls: ['./licenses-number-select.component.scss']
})
export class LicensesNumberSelectComponent implements OnInit, OnDestroy {

  public licensingModeDisabled = !this.data?.licensingModel;
  license: LicensedBundleUpdateInput;
  form: FormGroup;

  grid: VsGridOptions;
  isDisabled: boolean;
  private subs = new VsSubscriptionManager();
  public optionsStatusSelect: VsSelectOption[] = [
    { name: 'licensings.blocked', value: 0 },
    { name: 'licensings.active', value: 1 },
  ];

  constructor(private licenseGridService: LicensingsService,
              private dialogRef: MatDialogRef<LicensesNumberSelectComponent>,
              private dialog: LicenseDetailBundleSelectService,
              private bundlesService: BundlesService,
              @Inject(MAT_DIALOG_DATA) public data: any,
              private changeDetectorRef: ChangeDetectorRef,
              private readonly formBuilder: FormBuilder) {
  }

  ngOnInit() {
    this.license = {
      licensedTenantId: this.licenseGridService.licensedTenant.LicensedTenantId,
      bundleId: '',
      numberOfLicenses: 1,
      status: LicensedBundleStatus.BundleActive,
      numberOfTemporaryLicenses: 0,
      licensingMode: LicensingMode.Offline,
      licensingModel: LicensingModel.Floating
    };
    this.createForm();
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  createForm() {
    if (this.data) {
      this.isDisabled = true;
      this.form = this.formBuilder.group({
        numberOfLicenses: this.formBuilder.control(this.data.numberOfLicenses, [Validators.required, Validators.min(0)]),
        bundleId: this.formBuilder.control(this.data.bundleId, [Validators.required]),
        bundleName: this.formBuilder.control(this.data.bundleName, [Validators.required]),
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
        numberOfLicenses: this.formBuilder.control(this.license.numberOfLicenses, [Validators.required, Validators.min(0)]),
        bundleId: this.formBuilder.control(this.license.bundleId, [Validators.required]),
        bundleName: this.formBuilder.control('', [Validators.required]),
        licensingModel: this.formBuilder.control(false, []),
        licensingMode: this.formBuilder.control(false, []),
        expirationDateTime: this.formBuilder.control('', [validateMinDate]),
        status: this.formBuilder.control('', [])
      }, {
        validators: [temporaryLicensesValidator]
      });
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

  onBundleSearch() {
    this.bundlesService.getAllBundlesMinusLicensedBundlesFlag = true;
    this.bundlesService.licensedTenantId = this.licenseGridService.licensedTenant.LicensedTenantId;
    this.subs.add(
      'dialog',
      this.dialog.openDialog({
      id: this.licenseGridService.licensedTenant.LicensedTenantId
    }).subscribe((result) => {
      if (result) {
        this.form.get('bundleId').setValue(result.id);
        this.form.get('bundleName').setValue(result.name);
      }
    }));
  }

  cancel() {
    this.dialogRef.close();
  }

  save() {
    this.license.numberOfLicenses = this.form.get('numberOfLicenses').value;
    this.license.bundleId = this.form.get('bundleId').value;
    this.license.numberOfTemporaryLicenses = 0;
    this.license.expirationDateOfTemporaryLicenses = null;
    this.license.licensingModel = this.form.get('licensingModel').value ? LicensingModel.Named : LicensingModel.Floating;
    this.license.expirationDateTime = this.form.get('expirationDateTime').value;
    this.license.status = this.form.get('status').value;
    if (Boolean(this.license.licensingModel)) {
      this.license.licensingMode = this.form.get('licensingMode').value ? LicensingMode.Offline : LicensingMode.Online;
    } else {
      this.license.licensingMode = null;
    }
    this.dialogRef.close(this.license);
  }
}

