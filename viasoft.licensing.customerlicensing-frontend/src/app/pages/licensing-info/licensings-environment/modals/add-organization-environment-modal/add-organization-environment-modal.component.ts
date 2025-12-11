import { Component, Inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { AddOrganizationUnitModalComponent } from '../add-organization-unit-modal/add-organization-unit-modal.component';
import { DatabaseRequiredOnDesktopTypeValidator } from './database-required-on-desktop.validator';
import {MessageService, VsSubscriptionManager} from "@viasoft/common";
import {LicenseOrganizationService} from "../license-organization.service";
import {OrganizationUnitEnvironment} from "../../../../../../client/customer-licensing/model/organizationUnitEnvironment";
import {CreateEnvironmentOutputStatus} from "../../../../../../client/customer-licensing/model/createEnvironmentOutputStatus";

@Component({
  selector: 'app-add-organization-environment-modal',
  templateUrl: './add-organization-environment-modal.component.html',
  styleUrls: ['./add-organization-environment-modal.component.scss']
})

export class AddOrganizationEnvironmentModalComponent implements OnInit {
  @ViewChild('updateDbNameWarnMessage') protected updateDbWarnMessage: TemplateRef<any>;
  @ViewChild('updateDbVersionWarnMessage') protected updateDbVersionWarnMessage: TemplateRef<any>;
  @ViewChild('updateDbNameActionTemplate') protected updateDbNameActionTemplate: TemplateRef<any>;
  @ViewChild('updateDbVersionActionTemplate') protected updateDbVersionActionTemplate: TemplateRef<any>;
  
  public form: FormGroup;
  public errorMessage: string;
  public isUpdateDbNameChecked: boolean = false;
  public isUpdateDbVersionChecked: boolean = false;
  private canUpdateDbName: boolean = false;
  private canUpdateDbVersion: boolean = false;
  private isUpdating: boolean;
  private subs: VsSubscriptionManager = new VsSubscriptionManager();

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
    private licenseOrganizationService: LicenseOrganizationService,
    private readonly messageService: MessageService,
    @Inject(MAT_DIALOG_DATA) private data: { unitId?: string; environment?: OrganizationUnitEnvironment; licenseIdentifier?: string; },
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

  public ngOnDestroy(): void {
    this.subs.clear();
  }

  private createForm(env?: OrganizationUnitEnvironment) {
    this.form = this.fb.group({
      name: [env?.name || '', [Validators.required]],
      description: [env?.description || '', []],
      isProduction: [env?.isProduction],
      isMobile: [env?.isMobile],
      isDesktop: [env?.isDesktop],
      isWeb: [env?.isWeb],
      databaseName: [env?.databaseName || '', [Validators.pattern(/^[a-zA-Z0-9\s\-_.]+$/)]],
      databaseNameUpdateConfirm: [false],
      desktopDatabaseVersion: [env?.desktopDatabaseVersion || '', [Validators.pattern(/^\d{4}\.\d{1,2}\.\d{1,2}$/)]],
      databaseVersionUpdateConfirm: [false]
    }, {
      validators: DatabaseRequiredOnDesktopTypeValidator
    });

    this.subs.add('updateDb-name', this.form.get('databaseNameUpdateConfirm').valueChanges.subscribe(v => this.isUpdateDbNameChecked = v));
    this.subs.add('updateDb-version', this.form.get('databaseVersionUpdateConfirm').valueChanges.subscribe(v => this.isUpdateDbVersionChecked = v));
  }

  public dbNameUpdateWarning(): void {
    if(this.isUpdating && this.data.environment.isDesktop && !this.canUpdateDbName){
      this.subs.add('warn-updateDb-name', this.messageService.warn(this.updateDbWarnMessage,
       'Environment.Organization.AddEnvironmentModal.Warnings.Warning', false, this.updateDbNameActionTemplate).subscribe(() => {
        if(!this.canUpdateDbName){
          this.closeModal();
        }
      }));
    }
  }

  public releaseDbNameUpdate(){
    this.canUpdateDbName = this.isUpdateDbNameChecked;
  }

  public dbVersionUpdateWarning(): void {
    if(this.isUpdating && this.data.environment.isDesktop && !this.canUpdateDbVersion){
      this.subs.add('warn-updateDb-version', this.messageService.warn(this.updateDbVersionWarnMessage,
       'Environment.Organization.AddEnvironmentModal.Warnings.Warning', false, this.updateDbVersionActionTemplate).subscribe(() => {
        if(!this.canUpdateDbVersion){
          this.closeModal();
        }
      }));
    }
  }

  public releaseDbVersionUpdate(){
    this.canUpdateDbVersion = this.isUpdateDbVersionChecked;
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
    const errorMessagePrefix = 'Environment.Organization.AddEnvironmentModal.Errors';
    this.subs.add('create-environment',
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
      })
        .subscribe(response => {
            if(response.statusMessage == CreateEnvironmentOutputStatus.Ok) {
              this.closeModal(true);
            }
          }, (errors) => {
            this.errorMessage = `${errorMessagePrefix}.${errors.error.statusMessage}`
          }
        )
    )

  }

  private updateEnvironment(name: string, description: string, isProduction: boolean, isMobile: boolean, isDesktop: boolean, isWeb: boolean, databaseName: string, desktopDatabaseVersion: string): void {
    const errorMessagePrefix = 'Environment.Organization.AddEnvironmentModal.Errors';
    this.subs.add('update-environment',
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
      })
        .subscribe(response => {
            if(response.statusMessage == CreateEnvironmentOutputStatus.Ok) {
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
