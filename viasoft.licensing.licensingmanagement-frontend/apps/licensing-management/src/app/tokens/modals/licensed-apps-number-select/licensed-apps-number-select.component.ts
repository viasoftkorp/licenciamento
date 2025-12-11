import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { LicensedAppCreateInput } from '@viasoft/licensing-management/clients/licensing-management';
import { LicensingsService } from '@viasoft/licensing-management/app/pages/licensings/licensings.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-licensed-apps-number-select',
  templateUrl: './licensed-apps-number-select.component.html',
  styleUrls: ['./licensed-apps-number-select.component.scss']
})
export class LicensedAppsNumberSelectComponent implements OnInit, OnDestroy {

  form: FormGroup;
  app: LicensedAppCreateInput;
  subs: Array<Subscription> = [];
  hasUpdatePermission = false;

  constructor(private dialogRef: MatDialogRef<LicensedAppsNumberSelectComponent>,
    private licenseGridService: LicensingsService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (this.data) {
      this.hasUpdatePermission = this.data.hasUpdatePermission;
    }
  }

  ngOnInit() {
    this.app = this.data;
    this.createForm();
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  createForm() {
    this.form = new FormGroup({
      additionalNumberOfLicenses: new FormControl(this.app.additionalNumberOfLicenses, [Validators.required])
    });
    if (this.data && !this.hasUpdatePermission) {
      this.setFormDisabled();
    }
  }

  setFormDisabled() {
    this.form.disable();
  }

  save() {
    this.app.additionalNumberOfLicenses = this.form.get('additionalNumberOfLicenses').value;
    this.subs.push(this.licenseGridService.updateBundledAppFromLicense(this.app).subscribe(() => {
      this.dialogRef.close(this.app);
    }));
  }

  cancel() {
    this.dialogRef.close();
  }
}
