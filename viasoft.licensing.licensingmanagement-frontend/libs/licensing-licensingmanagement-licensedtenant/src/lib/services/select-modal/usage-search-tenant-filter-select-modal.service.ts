import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { VsGridSimpleColumn } from '@viasoft/components/grid';
import { IVsSelectModalData, VsSelectModalComponent } from '@viasoft/components/select-modal';
import { Observable } from 'rxjs';

import { LicensedTenantViewService } from '../view/licensedtenant-view.service';

@Injectable()
export class UsageSearchTenantFilterSelectModalService {

  constructor(
    private readonly dialog: MatDialog,
    private readonly licenseTenantViewService: LicensedTenantViewService
  ) {}

  public openDialog(config?: any): Observable<any> {
    return this.dialog.open(
      VsSelectModalComponent,
      {
        data: {
          icon: 'rectangle-list',
          title: 'LicensedTenant.TenantFilterSelectModal.Title',

          service: this.licenseTenantViewService,

          gridOptions: {
            columns: [
              new VsGridSimpleColumn({
                headerName: 'LicensedTenant.TenantFilterSelectModal.Headers.LicensingIdentifier',
                field: 'identifier',
                width: 150,
                disableFilter: true
              }),
              new VsGridSimpleColumn({
                headerName: 'LicensedTenant.TenantFilterSelectModal.Headers.AccountName',
                field: 'accountCompanyName',
                width: 140
              })
            ],
            enableFilter: true
          },

          filterOptions: {
            fields: [
              {
                condition: 'contains',
                field: 'accountCompanyName',
                type: 'string'
              }
            ]
          }
        } as IVsSelectModalData
      }
    ).afterClosed();
  }
}
