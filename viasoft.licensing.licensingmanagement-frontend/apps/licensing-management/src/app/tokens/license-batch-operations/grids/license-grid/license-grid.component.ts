import { Component, OnInit } from '@angular/core';
import { VsGridGetInput, VsGridGetResult, VsGridOptions, VsGridSimpleColumn } from '@viasoft/components/grid';
import { LicenseTenantViewService } from '@viasoft/licensing-management/app/tokens/services/licensed-view.service';
import { LicensedTenantViewOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-license-grid-batch-operation',
  templateUrl: './license-grid.component.html',
  styleUrls: ['./license-grid.component.scss']
})
export class LicenseGridBatchOperationsComponent implements OnInit {

  licenseGrid: VsGridOptions;

  constructor(private licensedTenantViewService: LicenseTenantViewService) { }

  ngOnInit() {
    this.configureGrid();
  }

  configureGrid() {
    this.licenseGrid = new VsGridOptions();
    this.licenseGrid.enableQuickFilter = false;

    this.licenseGrid.columns = [
      new VsGridSimpleColumn({
        headerName: 'TenantId',
        field: 'identifier',
        width: 160,
        disableFilter: true
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.accountId',
        field: 'accountCompanyName',
        width: 100
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.status',
        field: 'status',
        width: 100,
        disableFilter: true,
        translate: true,
        format: (value) => {
          switch (value) {
            case 1: {
              return 'licensings.blocked';
            }
            case 2: {
              return 'licensings.trial';
            }
            case 3: {
              return 'licensings.active';
            }
          }
        }
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.cnpjCpf',
        field: 'licensedCnpjs',
        width: 100
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.administratorEmail',
        field: 'administratorEmail',
        width: 160
      })
    ];

    this.licenseGrid.selectionMode = 'multiple';
    this.licenseGrid.get = (input: VsGridGetInput) => this.get(input);
    this.licenseGrid.select = (i: number, data: LicensedTenantViewOutput) => this.licenseSelected(data);
    this.licenseGrid.unselect = (i, data: LicensedTenantViewOutput) => this.licenseUnSelected(data);
    this.licenseGrid.selectAll = (state: boolean, currentGet: VsGridGetInput) => this.licenseSelectedAll(state, currentGet);
    this.licenseGrid.selectionCleared = () => {
      this.licensedTenantViewService.clearStates();
      this.licensedTenantViewService.emitCurrentState();
    };
  }

  get(input: VsGridGetInput): Observable<any> {
    return this.licensedTenantViewService.getAll(
      input
    )
      .pipe(
        map((list) => {
          return new VsGridGetResult(list.items, list.totalCount);
        }
        ),
        tap((list) => {
          this.licensedTenantViewService.totalCount = list.totalCount;
        })
      );
  }

  licenseSelected(data: LicensedTenantViewOutput) {
    this.licensedTenantViewService.selected(data.licensedTenantId);
  }

  licenseUnSelected(data: LicensedTenantViewOutput) {
    this.licensedTenantViewService.unSelected(data.licensedTenantId);
  }

  licenseSelectedAll(state: boolean, currentGet: VsGridGetInput) {
    this.licensedTenantViewService.allSelected(state, currentGet);
  }

}
