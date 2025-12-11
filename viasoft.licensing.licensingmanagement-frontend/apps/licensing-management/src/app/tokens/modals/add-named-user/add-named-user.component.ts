import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { VsAutocompleteGetInput, VsAutocompleteOption, VsAutocompleteOutput } from '@viasoft/components';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { LicensingMode } from '../../enum/licensing-mode.enum';
import { AddNamedUserOutput } from '../../interfaces/add-named-user-output.interface';
import { AddNamedUserService } from '../../services/modals-service/add-named-user.service';

@Component({
  selector: 'app-add-named-user',
  templateUrl: './add-named-user.component.html',
  styleUrls: ['./add-named-user.component.scss']
})
export class AddNamedUserComponent implements OnInit {
  public form: FormGroup;
  public title = this.data.namedUserEmail ? 'licensings.transferLicense' : 'licensings.addUser';
  public currentUser = this.data?.namedUserEmail;
  public isOfflineMode = this.data?.licensingMode === LicensingMode.Offline;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly addNamedUserService: AddNamedUserService,
    private readonly dialogRef: MatDialogRef<AddNamedUserComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
    this.configureForm();
  }

  public onUserSelected(option: VsAutocompleteOption<string>) {
    this.form.get('key').setValue(option.name);
  }

  public save() {
    this.dialogRef.close(<AddNamedUserOutput>{
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
    const userSelectInitialValue = this.data.namedUserEmail ? this.data.namedUserId : null;
    const keyInitialValue = this.data.namedUserEmail ? this.data.namedUserEmail : '';
    const deviceIdInitialValue = this.data.deviceId ? this.data.deviceId : '';

    this.form = this.formBuilder.group({
      userSelect: this.formBuilder.control(userSelectInitialValue, [Validators.required]),
      deviceId: this.formBuilder.control(deviceIdInitialValue),
      key: this.formBuilder.control(keyInitialValue)
    });
  }

  public getUserSelectAutocompleteOptions =
    (input: VsAutocompleteGetInput): Observable<VsAutocompleteOutput<string>> => this.addNamedUserService.getAllUsers({
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
              // Show email instead of username
              name: item?.email,
              value: item?.id
            }))
          } as VsAutocompleteOutput<string>
        },
      )
    );

  public getUserSelectAutocompleteName = () => of(this.form.get('key').value);
}
