import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  CreateEnvironmentOutputStatus,
  OrganizationUnitEnvironment,
  UpdateEnvironmentOutputStatus,
} from '@viasoft/licensing-management/clients/licensing-management';
import { first, map } from 'rxjs/operators';

import { LicenseOrganizationService } from '../../services/license-organization.service';
import { AddOrganizationUnitModalComponent } from '../add-organization-unit-modal/add-organization-unit-modal.component';
import { DatabaseRequiredOnDesktopTypeValidator } from './database-required-on-desktop.validator';

@Component({
  selector: 'app-add-organization-environment-modal',
  templateUrl: './add-organization-environment-modal.component.html',
  styleUrls: ['./add-organization-environment-modal.component.scss']
})
export class AddOrganizationEnvironmentModalComponent implements OnInit {
  private isUpdating: boolean;
  public form: FormGroup;
  public errorMessage: string;

  public get unitId(): string {
    return this.data.unitId;
  }
  public get environmentId(): string {
    return this.data.environment?.id;
  }
  public get licenseIdentifier(): string {
    return this.data.licenseIdentifier;
  }

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddOrganizationUnitModalComponent>,
    @Inject(MAT_DIALOG_DATA) private data: { unitId?: string; environment?: OrganizationUnitEnvironment; licenseIdentifier?: string; },
    private licenseOrganizationService: LicenseOrganizationService
  ) {
  }

  public ngOnInit(): void {
    if (!this.unitId && !this.environmentId) {
      this.closeModal();
    }
    if (this.data.environment) {
      this.isUpdating = true;
      this.createForm(this.data.environment);
    } else {
      this.createForm();
    }
  }

  private createForm(environment?: OrganizationUnitEnvironment) {
    this.form = this.fb.group({
      name: [environment?.name || '', [Validators.required]],
      description: [environment?.description || '', []],
      isProduction: [environment?.isProduction],
      // Default value is true
      isMobile: [typeof environment?.isMobile !== 'boolean' || environment?.isMobile],
      isDesktop: [environment?.isDesktop],
      isWeb: [environment?.isWeb],
      databaseName: [environment?.databaseName || '', [Validators.pattern(/^[a-zA-Z0-9\s\-_.]+$/)]],
      desktopDatabaseVersion: [environment?.desktopDatabaseVersion || '', [Validators.pattern(/^\d{4}\.\d{1,2}\.\d{1,2}$/)]],
    }, {
      validators: DatabaseRequiredOnDesktopTypeValidator
    });
  }

  public createOrUpdateEnvironment(): void {
    if (this.form.invalid) {
      return;
    }
    this.errorMessage = '';
    const name = this.form.get('name').value;
    const description = this.form.get('description').value;
    const isProduction = Boolean(this.form.get('isProduction').value);
    const isMobile = Boolean(this.form.get('isMobile').value);
    const isDesktop = Boolean(this.form.get('isDesktop').value);
    const isWeb = Boolean(this.form.get('isWeb').value);
    const databaseName = this.form.get('databaseName').value;
    const desktopDatabaseVersion = isDesktop ? this.form.get('desktopDatabaseVersion').value : null;
    if (this.isUpdating) {
      this.updateEnvironment(name, description, isProduction, isMobile, isDesktop, isWeb, databaseName, desktopDatabaseVersion);
    } else {
      this.createEnvironment(name, description, isProduction, isMobile, isDesktop, isWeb, databaseName, desktopDatabaseVersion);
    }
  }

  private createEnvironment(name: string, description: string, isProduction: boolean, isMobile: boolean, isDesktop: boolean, isWeb: boolean, databaseName: string, desktopDatabaseVersion: string): void {
    const errorMessagePrefix = 'Organization.UnitEnvironment.Errors';
    this.licenseOrganizationService.createEnvironment(this.licenseIdentifier, {
      name,
      description,
      isProduction,
      isMobile,
      isDesktop,
      isWeb,
      databaseName,
      organizationUnitId: this.unitId,
      isActive: true,
      desktopDatabaseVersion
    }).pipe(first())
      .subscribe((res) => {
        switch (res.statusMessage) {
          case CreateEnvironmentOutputStatus.Ok:
            this.closeModal(true);
            break;
          default:
            this.errorMessage = `${errorMessagePrefix}.${res.statusMessage}`;
            break;
        }
      }, () => {
        this.errorMessage = `${errorMessagePrefix}.Unknown`;
      });
  }

  private updateEnvironment(name: string, description: string, isProduction: boolean, isMobile: boolean, isDesktop: boolean, isWeb: boolean, databaseName: string, desktopDatabaseVersion: string): void {
    const errorMessagePrefix = 'Organization.UnitEnvironment.Errors';
    this.licenseOrganizationService.updateEnvironment(this.licenseIdentifier, {
      id: this.environmentId,
      name,
      description,
      isProduction,
      isMobile,
      isDesktop,
      isWeb,
      databaseName,
      organizationUnitId: this.unitId,
      desktopDatabaseVersion
    }).pipe(first())
      .subscribe((res) => {
        switch (res.statusMessage) {
          case UpdateEnvironmentOutputStatus.Ok:
            this.closeModal(true);
            break;
          default:
            this.errorMessage = `${errorMessagePrefix}.${res.statusMessage}`;
            break;
        }
      }, () => {
        this.errorMessage = `${errorMessagePrefix}.Unknown`;
      });
  }

  public closeModal(data = false): void {
    this.dialogRef.close(data);
  }
}
