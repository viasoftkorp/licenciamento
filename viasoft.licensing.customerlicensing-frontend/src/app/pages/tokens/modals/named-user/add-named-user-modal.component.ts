import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { VsAutocompleteGetFn, VsAutocompleteGetInput, VsAutocompleteGetNameFn, VsAutocompleteOptions, VsAutocompleteOutput, VsAutocompleteValue } from '@viasoft/components';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { AddNamedUserDialogDataOutput } from 'src/client/customer-licensing/model/AddNamedUserDialogDataOutput';
import { LicensingModes } from 'src/client/customer-licensing/model/LicensingModes';
import { AddNamedUserService } from '../services/add-named-user.service';

@Component({
  selector: 'app-add-named-user-modal',
  templateUrl: './add-named-user-modal.component.html',
  styleUrls: ['./add-named-user-modal.component.scss']
})
export class AddNamedUserModalComponent implements OnInit {

  public form: FormGroup;
  public autocompleteMaxDropSize = 5;
  public title = ''
  public currentUser = ''
  public isOfflineMode = false

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly addNamedUserService: AddNamedUserService,
    private readonly dialogRef: MatDialogRef<AddNamedUserModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.title = this.data?.namedUserEmail ? 'LicensingInfo.TransferLicense' : 'LicensingInfo.AssignLicense';
    this.currentUser = this.data?.namedUserEmail;
    this.isOfflineMode = this.data?.licensingMode === LicensingModes.Offline;
  }

  ngOnInit(): void {
    this.configureForm();
  }

  public onUserSelected({ key, value }: { key: string; value: string; }) {
    this.form.get('key').setValue(key);
  }

  public save() {
    this.dialogRef.close(<AddNamedUserDialogDataOutput>{
      key: this.form.get('key').value,
      value: this.form.get('userSelect').value,
      deviceId: this.form.get('deviceId').value
    });
  }

  public cancel() {
    this.dialogRef.close();
  }

  public get isFormValid(): boolean {
    return this.form.valid;
  }

  private configureForm(): void {
    const userSelectInitialValue = this.data.namedUserEmail ? {key: this.data.namedUserEmail, value: this.data.namedUserId} : null;
    const keyInitialValue = this.data.namedUserEmail ? this.data.namedUserEmail : '';
    const deviceIdInitialValue = this.data.deviceId ? this.data.deviceId : '';
    this.form = this.formBuilder.group({
      userSelect: this.formBuilder.control(userSelectInitialValue, [Validators.required]),
      deviceId: this.formBuilder.control(deviceIdInitialValue),
      key: this.formBuilder.control(keyInitialValue)
    });
  }

  public autocompleteGet: VsAutocompleteGetFn<string> = (input: VsAutocompleteGetInput) => {
    return this.addNamedUserService.getAllUsers({
      filter: input.valueToFilter,
      maxResultCount: input.maxDropSize,
      skipCount: input.skipCount
    }, this.data.licensedTenantIdentifier)
    .pipe(
      map(
        (output) => {
          if (!output?.items?.length) {
            return output;
          }

          return {
            totalCount: output.totalCount,
            items: output.items.map((item) => ({
              name: item?.email,
              value: item?.id
            })),
          } as VsAutocompleteOutput<string>;
        },
      )
    );
  }

  public getAutocompleteNames: VsAutocompleteGetNameFn<string> = (value, options, controlName) => {
    return of(this.form.get('userSelect').value.key) || null;
  };
}
