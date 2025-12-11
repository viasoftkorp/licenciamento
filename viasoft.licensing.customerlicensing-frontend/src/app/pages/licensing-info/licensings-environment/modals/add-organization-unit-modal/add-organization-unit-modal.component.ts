import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {UUID} from "angular2-uuid";

import {LicenseOrganizationService} from "../license-organization.service";
import {OrganizationUnit} from "../../../../../../client/customer-licensing/model/organizationUnit";
import {UpdateOrganizationUnitOutputStatus} from "../../../../../../client/customer-licensing/model/updateOrganizationUnitOutputStatus";
import {VsSubscriptionManager} from "@viasoft/common";

@Component({
  selector: 'app-add-organization-unit-modal',
  templateUrl: './add-organization-unit-modal.component.html',
  styleUrls: ['./add-organization-unit-modal.component.scss']
})

export class AddOrganizationUnitModalComponent implements OnInit {

  public form: FormGroup;
  public errorMessage: string;
  private subs: VsSubscriptionManager = new VsSubscriptionManager();

  public get organizationId(): string {
    return this.data.organizationId;
  }

  public get unitId(): string {
    return this.data.unitId;
  }

  public get licenseIdentifier(): string {
    return this.data.licenseIdentifier;
  }

  public get isDisabled(): boolean {
    return Boolean(!this.form || this.form.invalid);
  }

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddOrganizationUnitModalComponent>,
    private licenseOrganizationService: LicenseOrganizationService,
    @Inject(MAT_DIALOG_DATA) private data: { organizationId: string; unitId: string; licenseIdentifier?: string; }
  ) { }

  public ngOnInit(): void {
    if (!this.organizationId && !this.unitId) {
      this.closeModal();
    }
    if (this.unitId) {
      this.getUnit();
    } else {
      this.createForm();
    }
  }

  public ngOnDestroy() {
    this.subs.clear();
  }

  private createForm(data?: OrganizationUnit) {
    this.form = this.fb.group({
      name: [data?.name || '', [Validators.required]],
      description: [data?.description || '', []],
      organizationId: [data?.organizationId || this.organizationId, []],
    });
  }

  public createOrUpdateUnit(): void {
    if (this.form.invalid) {
      return;
    }
    if (this.unitId) {
      this.updateUnit();
    } else {
      this.createUnit();
    }
  }

  public getUnit(): void {
    this.subs.add('get-unit',
      this.licenseOrganizationService.getUnit(this.licenseIdentifier, this.unitId)
        .subscribe((res) => {
          this.createForm(res)
        }, () => {
          this.closeModal();
        })
      )
  }

  private createUnit(): void {
    const errorMessagePrefix = 'Environment.Organization.AddUnitModal.Errors';
    const name = this.form.get('name').value;
    const description = this.form.get('description').value;
    const organizationId = this.form.get('organizationId').value;
    this.subs.add('create-unit',
      this.licenseOrganizationService.createUnit(organizationId, {
        id: UUID.UUID(),
        name,
        description,
        organizationId,
        isActive: true
      })
        .subscribe(response => {
            if(response.statusMessage == UpdateOrganizationUnitOutputStatus.Ok) {
              this.closeModal(true);
            }
          }, (errors) => {
            this.errorMessage = `${errorMessagePrefix}.${errors.error.statusMessage}`
          }
        )
    )
  }

  private updateUnit(): void {
    const errorMessagePrefix = 'Environment.Organization.AddUnitModal.Errors';
    const name = this.form.get('name').value;
    const description = this.form.get('description').value;
    const organizationId = this.form.get('organizationId').value;
    this.subs.add('update-unit',
      this.licenseOrganizationService.updateUnit( {
        id: this.unitId,
        name,
        description,
        organizationId
      })
        .subscribe(response => {
            if(response.statusMessage == UpdateOrganizationUnitOutputStatus.Ok) {
              this.closeModal(true);
            }
          }, (errors) => {
            this.errorMessage = `${errorMessagePrefix}.${errors.error.statusMessage}`
          }
        )
    )

  }
  public closeModal(data = false): void {
    this.dialogRef.close(data);
  }
}
