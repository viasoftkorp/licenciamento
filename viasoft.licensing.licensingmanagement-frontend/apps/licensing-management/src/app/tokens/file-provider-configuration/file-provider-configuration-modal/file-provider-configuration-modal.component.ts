import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { LicensingsService } from '@viasoft/licensing-management/app/pages/licensings/licensings.service';
import { BundledAndLooseAppOutput, FileAppQuotaInput } from '@viasoft/licensing-management/clients/licensing-management';

import {
  FileQuotaLicensedAppSelectComponent,
} from '../../modals/file-quota-licensed-app-select/file-quota-licensed-app-select.component';
import { FileProviderConfigurationService } from '../file-provider-configuration.service';
import { VsSubscriptionManager } from '@viasoft/common';

@Component({
  selector: 'app-file-provider-configuration-modal',
  templateUrl: './file-provider-configuration-modal.component.html',
  styleUrls: ['./file-provider-configuration-modal.component.scss']
})
export class FileProviderConfigurationModalComponent implements OnInit, OnDestroy {
  private subs = new VsSubscriptionManager();
  public form: FormGroup;
  public fileConfigurationErrorDuringSave: boolean;
  public hasFinishedFileConfigurationRequest: boolean;
  public fileProviderConfiguration: FileAppQuotaInput;
  public appId: string;
  public selectedApp: BundledAndLooseAppOutput;

  constructor(
    private fileProviderConfigurationService: FileProviderConfigurationService,
    private licensingsService: LicensingsService,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<FileProviderConfigurationModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: {
      appId: string;
      licensedTenantId: string;
    }
  ) {
    if (!this.data) {
      this.cancel();
    }
  }

  ngOnInit(): void {
    this.getFileConfiguration();
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  private getFileConfiguration(): void {
    this.appId = this.data ? this.data.appId : undefined;
    if (!this.appId) {
      this.hasFinishedFileConfigurationRequest = true;
      return;
    }
    this.subs.add('getConfiguration', this.fileProviderConfigurationService
      .getAppConfiguration(this.licensingsService.getLicensedTenantId(), this.appId).subscribe(res => {
        if (res) {
          this.fileProviderConfiguration = {
            quotaLimit: res.quotaLimit,
            appId: res.appId
          };
        } else {
          this.cancel();
        }
        this.hasFinishedFileConfigurationRequest = true;
      })
    );
  }

  public fileProviderFormChanged(event: FormGroup): void {
    this.form = event;
  }

  public insertOrUpdateFileConfiguration(): void {
    this.fileConfigurationErrorDuringSave = false;
    if (!this.form || !this.form.dirty) {
      return;
    }
    const id: string = this.form.get('id').value;
    const licenseTenantIdentifier: string = this.licensingsService.getLicensedTenantId();
    const quotaLimit: number = this.form.get('quotaLimit').value;
    const input: FileAppQuotaInput = {
      id,
      quotaLimit,
      appId: this.appId,
      licensedTenantId: licenseTenantIdentifier
    };
    const request = this.data.appId
      ? this.fileProviderConfigurationService.updateAppQuota(input)
      : this.fileProviderConfigurationService.insertAppQuota(input);

    this.subs.add('insertOrUpdateFileConfiguration', request
      .subscribe({
        next: () => {
          this.form.markAsPristine();
          this.cancel();
        },
        error: () => {
          this.fileConfigurationErrorDuringSave = true;
        }
      })
    );
  }
  public openAppSearch(): void {
    this.subs.add('openAppSearch',
      this.dialog.open(FileQuotaLicensedAppSelectComponent, {
        data: {
          licensedTenantId: this.data.licensedTenantId
        }
      }).afterClosed().subscribe((res: BundledAndLooseAppOutput) => {
        if (!res) { return; }
        this.selectedApp = res;
        this.appId = this.selectedApp.id;
      })
    );
  }

  public cancel(): void {
    this.dialogRef.close();
  }
}
