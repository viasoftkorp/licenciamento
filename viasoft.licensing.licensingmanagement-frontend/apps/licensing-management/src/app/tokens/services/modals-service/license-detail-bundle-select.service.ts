import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { VsGridCheckboxColumn, VsGridSimpleColumn, VsSelectModalComponent } from '@viasoft/components';
import { IVsSelectModalData } from '@viasoft/components/select-modal/src/select-modal.component';
import { BundlesService } from '@viasoft/licensing-management/app/pages/bundles/bundles.service';
import { BundleCreateOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class LicenseDetailBundleSelectService {

  constructor(private dialog: MatDialog, private bundleService: BundlesService) { }

  public openDialog(config?: any): Observable<BundleCreateOutput> {

     /* End of domain-specific code. */
    return this.dialog.open(
      VsSelectModalComponent,
      {
        data: {

          icon: 'bars',
          title: 'products.products',

          service: config && config.service
                      ? config.service
                      : this.bundleService,

          gridOptions: {
            columns: [

              new VsGridSimpleColumn({
                headerName: 'products.name',
                field: 'name',
                width: 100
              }),
              new VsGridSimpleColumn({
                headerName: 'products.identifier',
                field: 'identifier',
                width: 100
              }),
              new VsGridSimpleColumn({
                headerName: 'products.software',
                field: 'softwareName',
                width: 100,
                sorting: {
                  disable: true
                }
              }),
              new VsGridCheckboxColumn({
                headerName: 'products.custom',
                field: 'isCustom',
                disabled: true,
                width: 100
              })
            ],
            enableFilter: false,
            enableQuickFilter: false
          },

          filterOptions: {

            fields: [
              {
                condition: 'contains',
                field: 'name',
                type: 'string'
              },
              {
                condition: 'contains',
                field: 'identifier',
                type: 'string'
              }
            ]
          },
        } as IVsSelectModalData
      }
    ).afterClosed().pipe(
        tap(
        () => {
          this.bundleService.getAllBundlesMinusLicensedBundlesFlag = false;
          this.bundleService.licensedTenantId = null;
        }
      )
    );
  }
}
