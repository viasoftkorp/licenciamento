import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { VsGridGetInput, VsGridGetResult, VsGridOptions, VsGridSimpleColumn } from '@viasoft/components';
import { BundledAndLooseAppOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { FileProviderConfigurationService } from '../../file-provider-configuration/file-provider-configuration.service';

@Component({
  selector: 'app-file-quota-licensed-app-select',
  templateUrl: './file-quota-licensed-app-select.component.html',
  styleUrls: ['./file-quota-licensed-app-select.component.scss']
})
export class FileQuotaLicensedAppSelectComponent implements OnInit {
  public gridOptions: VsGridOptions;
  private searchFilter: string;

  constructor(
    private fileProviderConfigurationService: FileProviderConfigurationService,
    private dialogRef: MatDialogRef<FileQuotaLicensedAppSelectComponent>,
    @Inject(MAT_DIALOG_DATA) private data: { licensedTenantId: string; }
  ) {
    if (!this.data || !this.data.licensedTenantId) {
      this.dialogRef.close();
      return;
    }
  }

  ngOnInit(): void {
    this.gridOptions = new VsGridOptions();
    this.gridOptions.id = 'E9DC000D-C3EF-4B89-9645-84E68CCAA457';
    this.gridOptions.enableFilter = false;
    this.gridOptions.enableQuickFilter = false;
    this.gridOptions.enableSorting = false;
    this.gridOptions.columns = [
      new VsGridSimpleColumn({
        field: 'name',
        headerName: 'filequota.addApp.grid.appName'
      })
    ];
    this.gridOptions.get = (input: VsGridGetInput) => this.get(input);
    this.gridOptions.select = (index: number, data: BundledAndLooseAppOutput) => this.select(data);
  }

  private get(input: VsGridGetInput): Observable<any> {
    return this.fileProviderConfigurationService.getLicensedAppsForQuotaConfiguration(
      this.data.licensedTenantId,
      (this.searchFilter),
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount
    ).pipe(map(res => new VsGridGetResult(res.items, res.totalCount)));
  }

  private select(input: BundledAndLooseAppOutput): void {
    this.dialogRef.close(input);
  }

  public search(filter?: string): void {
    this.searchFilter = filter;
    this.gridOptions.refresh();
  }
}
