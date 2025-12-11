import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CreateOrganizationUnitOutputStatus,
  OrganizationUnit,
  UpdateOrganizationUnitOutputStatus,
} from '@viasoft/licensing-management/clients/licensing-management';
import { first } from 'rxjs/operators';

import { LicenseOrganizationService } from '../../services/license-organization.service';

@Component({
  selector: 'app-add-organization-unit-modal',
  templateUrl: './add-organization-unit-modal.component.html',
  styleUrls: ['./add-organization-unit-modal.component.scss']
})
export class AddOrganizationUnitModalComponent implements OnInit {
  public form: FormGroup;
  public nameConflict: boolean;
  public unknownError: boolean;

  public get organizationId(): string {
    return this.data.organizationId;
  }

  public get unitId(): string {
    return this.data.unitId;
  }

  public get licenseIdentifier(): string {
    return this.data.licenseIdentifier;
  }

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddOrganizationUnitModalComponent>,
    @Inject(MAT_DIALOG_DATA) private data: { organizationId: string; unitId: string; licenseIdentifier?: string; },
    private licenseOrganizationService: LicenseOrganizationService
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
    this.licenseOrganizationService.getUnit(this.licenseIdentifier, this.unitId)
      .pipe(first())
      .subscribe((res) => {
        this.createForm(res)
      }, () => {
        this.closeModal();
      });
  }

  private createUnit(): void {
    this.nameConflict = false;
    this.unknownError = false;
    const name = this.form.get('name').value;
    const description = this.form.get('description').value;
    const organizationId = this.form.get('organizationId').value;
    this.licenseOrganizationService.createUnit(this.licenseIdentifier, {
      name,
      description,
      organizationId,
      isActive: true
    })
      .pipe(first())
      .subscribe((res) => {
        const status = <CreateOrganizationUnitOutputStatus>res.statusMessage;
        if (status === CreateOrganizationUnitOutputStatus.Ok) {
          this.closeModal(true);
        } else if (status === CreateOrganizationUnitOutputStatus.NameConflict) {
          this.nameConflict = true;
        } else if (status === CreateOrganizationUnitOutputStatus.OrganizationNotFound) {
          this.unknownError = true;
        }
      }, () => {
        this.unknownError = true;
      });
  }

  private updateUnit(): void {
    this.nameConflict = false;
    this.unknownError = false;
    const name = this.form.get('name').value;
    const description = this.form.get('description').value;
    const organizationId = this.form.get('organizationId').value;
    this.licenseOrganizationService.updateUnit(this.licenseIdentifier, {
      id: this.unitId,
      name,
      description,
      organizationId
    })
      .pipe(first())
      .subscribe((res) => {
        const status = <UpdateOrganizationUnitOutputStatus>res.statusMessage;
        if (status === UpdateOrganizationUnitOutputStatus.Ok) {
          this.closeModal(true);
        } else if (status === UpdateOrganizationUnitOutputStatus.NameConflict) {
          this.nameConflict = true;
        } else if (status === UpdateOrganizationUnitOutputStatus.NotFound) {
          this.unknownError = true;
        }
      }, () => {
        this.unknownError = true;
      });
  }

  public closeModal(data = false): void {
    this.dialogRef.close(data);
  }
}
