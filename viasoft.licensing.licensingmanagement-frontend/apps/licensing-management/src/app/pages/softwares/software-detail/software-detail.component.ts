import { Component, OnDestroy, OnInit, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UUID } from 'angular2-uuid';
import { Subscription } from 'rxjs';
import { SoftwareCreateInput, SoftwareUpdateInput } from '@viasoft/licensing-management/clients/licensing-management';

import { SoftwaresFormControlService } from '../softwares-form-control.service';
import { SoftwaresService } from '../softwares.service';
import { MessageService } from '@viasoft/common';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-software-detail',
  templateUrl: './software-detail.component.html',
  styleUrls: ['./software-detail.component.scss']
})
export class SoftwareDetailComponent implements OnInit, OnDestroy {

  id: string;
  form: FormGroup;
  software: any;
  subs: Array<Subscription> = [];
  notShowSaveButton = false;

  constructor(
    private SoftwareService: SoftwaresFormControlService,
    private softwares: SoftwaresService,
    private notification: MessageService,
    @Inject(MAT_DIALOG_DATA)
    private data: any
  ) { }

  ngOnInit() {
    this.software = {
      id: UUID.UUID(),
      name: '',
      identifier: '',
      isActive: true,
    } as SoftwareCreateInput;

    if (this.data) {
      this.id = this.data.id;
    }

    if (this.id) {
      this.subs.push(this.softwares.getById(this.id).subscribe(data => {
        if (data) {
          this.software = data as SoftwareUpdateInput;
          this.createFormOrResetForm();
        }
      }));
    }

    this.createFormOrResetForm();
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
    this.SoftwareService.softwareFormIsInvalid();
  }

  createFormOrResetForm() {
    if (this.form) {
      this.form.reset({
        name: this.software.name,
        identifier: this.software.identifier,
        isActive: this.software.isActive
      });
    } else {
      this.form = new FormGroup({
        name: new FormControl(this.software.name, [Validators.required]),
        identifier: new FormControl(this.software.identifier, [Validators.required]),
        isActive: new FormControl(this.software.isActive),
      });
    }
    if (this.data && !this.data.hasUpdatePermission) {
      this.setFormDisabled();
    }
  }

  setFormDisabled() {
    this.form.disable();
    this.notShowSaveButton = true;
  }

  get validToSave() {
    return !!(this.form && this.form.valid && this.form.dirty);
  }

  saveOnClick() {
    if (!this.id) {
      this.software.name = this.form.get('name').value;
      this.software.identifier = this.form.get('identifier').value;
      this.software.isActive = this.form.get('isActive').value;

      this.subs.push(this.softwares.create(this.software)
        .subscribe(result => {
          if (result.operationValidationDescription === 'DuplicatedIdentifier') {
            this.notification.error(
              'common.error.identifier_already_exists',
              'common.error.could_not_create|name:' + this.software.name
            );
          }
          this.id = this.software.id;
          this.form.markAsPristine();
          this.SoftwareService.refreshGrid();
        }));

    } else {

      this.software.name = this.form.get('name').value;
      this.software.identifier = this.form.get('identifier').value;
      this.software.isActive = this.form.get('isActive').value;

      this.subs.push(this.softwares.update(this.software)
        .subscribe(result => {
          if (result.operationValidationDescription === 'DuplicatedIdentifier') {
            this.notification.error(
              'common.error.identifier_already_exists',
              'common.error.could_not_edit|name:' + this.software.name
            );
          } else {
            this.form.markAsPristine();
            this.SoftwareService.refreshGrid();
          }
        }));
    }
  }

  save() {
    this.saveOnClick();
  }

}
