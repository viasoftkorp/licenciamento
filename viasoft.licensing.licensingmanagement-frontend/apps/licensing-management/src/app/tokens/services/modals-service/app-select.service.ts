import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { VsGridCheckboxColumn, VsGridSimpleColumn } from '@viasoft/components';
import { VsSelectModalComponent } from '@viasoft/components/select-modal';
import { IVsSelectModalData } from '@viasoft/components/select-modal/src/select-modal.component';
import { AppsService } from '@viasoft/licensing-management/app/pages/apps/apps.service';
import { AppCreateOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AppSelectService {

  bundleSoftwareId: string;
  listOfDomains: Array<string>;

  constructor(
    private dialog: MatDialog,
    private appService: AppsService
  ) { }

  public openDialog(config?: any): Observable<AppCreateOutput> {
   if (config && config.softwareId) {
      this.appService.softwareIdFilterBy = config.softwareId;
      this.listOfDomains = config.listOfDomains;
   } else if (config && config.licensedApps) {
     this.appService.licensedApps = config.licensedApps;
     this.listOfDomains = config.listOfDomains;
   }

    /* End of domain-specific code. */
   return this.dialog.open(
      VsSelectModalComponent,
      {
        data: {

          icon: 'server',
          title: 'apps.apps',

          service: config && config.service
                     ? config.service
                     : this.appService,

          gridOptions: {
            columns: [
              new VsGridSimpleColumn({
                headerName: 'apps.name',
                field: 'name'
              }),
              new VsGridSimpleColumn({
                headerName: 'apps.identifier',
                field: 'identifier'
              }),
              new VsGridSimpleColumn({
                headerName: 'apps.domains.domain',
                field: 'domain',
                disableFilter: true,
                translate: true,
                format: (value) => {
                  if (this.listOfDomains[value] !== undefined) {
                    return 'apps.domains.' + this.listOfDomains[value];
                  }
                }
              }),
              new VsGridSimpleColumn({
                headerName: 'apps.software',
                field: 'softwareName',
                sorting: {
                  disable: true
                }
              }),
              new VsGridCheckboxColumn({
                headerName: 'apps.active',
                field: 'isActive',
                width: 80
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
    ).afterClosed().pipe(tap(() => {
      this.appService.softwareIdFilterBy = undefined;
      this.appService.licensedApps = [];
      this.listOfDomains = [];
    }));
  }
}
