import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import {
  VsGridGetInput,
  VsGridGetResult,
  VsGridNumberColumn,
  VsGridOptions,
  VsGridSimpleColumn,
} from '@viasoft/components/grid';
import { LicensingFormCurrentTab } from '@viasoft/licensing-management/app/tokens/enum/licensing-form-current-tab.enum';
import {
  FileProviderConfigurationModalComponent,
} from '@viasoft/licensing-management/app/tokens/file-provider-configuration/file-provider-configuration-modal/file-provider-configuration-modal.component';
import {
  FileProviderConfigurationService,
} from '@viasoft/licensing-management/app/tokens/file-provider-configuration/file-provider-configuration.service';
import { FileAppQuotaView, FileTenantQuota } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { LicensingsFormControlServices } from '../../licensings-form-control.service';
import { LicensingsService } from '../../licensings.service';
import { VsSubscriptionManager } from '@viasoft/common';

@Component({
  selector: 'app-license-detail-file-quota',
  templateUrl: './license-detail-file-quota.component.html',
  styleUrls: ['./license-detail-file-quota.component.scss']
})
export class LicenseDetailFileQuotaComponent implements OnInit, OnDestroy {
  private subs = new VsSubscriptionManager();
  public gridOptions: VsGridOptions;
  public form: FormGroup;
  public fileConfigurationErrorDuringSave: boolean;
  public hasFinishedFileConfigurationRequest: boolean;
  public fileProviderConfiguration: FileTenantQuota;

  constructor(
    private dialog: MatDialog,
    private licensingsService: LicensingsService,
    private licensingsFormControlServices: LicensingsFormControlServices,
    private fileProviderConfigurationService: FileProviderConfigurationService
  ) {
    this.licensingsService.setCurrentTab(LicensingFormCurrentTab.FileQuota);
  }

  ngOnInit(): void {
    this.configureGrid();
    this.getFileConfiguration();
    this.subs.add('gridRefresh', this.licensingsFormControlServices.gridRefresherSubject.subscribe(res => {
      if (this.gridOptions) {
        this.gridOptions.refresh();
      }
    }));
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  private configureGrid(): void {
    this.gridOptions = new VsGridOptions();
    this.gridOptions.enableQuickFilter = false;
    this.gridOptions.id = 'FEC02F3F-F659-4FA8-86BF-DA14F74A8011';
    this.gridOptions.sizeColumnsToFit = true;
    this.gridOptions.columns = [
      new VsGridSimpleColumn({
        headerName: 'file_provider.grid.appName',
        field: 'appName'
      }),
      new VsGridNumberColumn({
        headerName: 'file_provider.grid.quotaLimit',
        field: 'quotaLimit',
        disableFilter: true,
        format: (bytesValue) => {
          if (bytesValue < 0) {
            return -1;
          }
          let mbValue = String((Number(bytesValue) / 1024 / 1024).toFixed(4));
          // Remove trailing zeros
          while (mbValue && mbValue.includes('.') && (mbValue.endsWith('0') || mbValue.endsWith('.'))) {
            mbValue = mbValue.substr(0, mbValue.length - 1);
          }
          return `${mbValue} Mb`;
        }
      }),
    ];
    this.gridOptions.get = (input: VsGridGetInput) => this.get(input);
    this.gridOptions.edit = (i, data) => this.editFileQuota(data);
    this.gridOptions.select = (i, data) => this.editFileQuota(data);
    this.gridOptions.delete = (i, data) => this.deleteFileQuota(data);
    this.gridOptions.deleteBehaviours.enableAutoDeleteConfirm = true;
  }

  private get(input: VsGridGetInput): Observable<any> {
    const licensedTenantId = this.licensingsService.getLicensedTenantId();
    return this.fileProviderConfigurationService.getAll(licensedTenantId, input.filter,
      input.advancedFilter, input.sorting, input.skipCount, input.maxResultCount)
      .pipe(map(res => {
        return new VsGridGetResult(res.items, res.totalCount);
      }));
  }

  public editFileQuota(data: FileAppQuotaView): void {
    if (!data || !data.id) {
      return;
    }

    this.subs.add('configModal',
      this.dialog.open(FileProviderConfigurationModalComponent, {
        data: {
          appId: data.appId
        }
      }).afterClosed().subscribe(() => {
        this.gridOptions.refresh();
      })
    );
  }

  private deleteFileQuota(data: FileAppQuotaView): void {
    this.subs.add('delete',
      this.fileProviderConfigurationService.deleteAppQuotaConfiguration(data.licensedTenantId, data.appId)
      .subscribe(res => {
        this.gridOptions.refresh();
      })
    );
  }

  private getFileConfiguration(): void {
    if (!this.licensingsService.getLicensedTenantId()) {
      this.hasFinishedFileConfigurationRequest = true;
      return;
    }
    this.subs.add('getConfiguration',
      this.fileProviderConfigurationService.getTenantConfiguration(this.licensingsService.getLicensedTenantId())
        .subscribe(res => {
          if (res) {
            this.fileProviderConfiguration = res;
          }
          this.hasFinishedFileConfigurationRequest = true;
        })
    );
  }

  public fileProviderFormChanged(event: FormGroup): void {
    this.form = event;
  }

  public insertOrUpdateFileConfiguration(): void {
    if (!this.form || !this.form.dirty) {
      return;
    }
    this.fileConfigurationErrorDuringSave = false;
    const id: string = this.form.get('id').value;
    const licenseTenantId: string = this.licensingsService.getLicensedTenantId();
    const quotaLimit: number = this.form.get('quotaLimit').value;
    this.form.markAsPristine();
    this.subs.add('insertOrUpdateFileConfiguration',
      this.fileProviderConfigurationService.insertOrUpdateTenantConfiguration(id, licenseTenantId, quotaLimit)
        .subscribe(
          () => { },
          () => { this.fileConfigurationErrorDuringSave = true; }
        )
    );
  }
}
