import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { VsGridCheckboxColumn, VsGridSimpleColumn } from '@viasoft/components';
import { VsSelectModalComponent } from '@viasoft/components/select-modal';
import { IVsSelectModalData } from '@viasoft/components/select-modal/src/select-modal.component';
import { SoftwaresService } from '@viasoft/licensing-management/app/pages/softwares/softwares.service';
import { SoftwareCreateOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SoftwareSelectService {

  constructor(
    private dialog: MatDialog,
    private softwaresService: SoftwaresService
  ) { }

  public openDialog(config?: any): Observable<SoftwareCreateOutput> {

    // Doing this way, because there's still no method for booleans in the filters.
    if (config && config.onlyActiveSoftwares) {
      this.softwaresService.onlyActiveSoftwares = config.onlyActiveSoftwares;
   }

    return this.dialog.open(
      VsSelectModalComponent,
      {
        data: {
          icon: 'boxes-stacked',
          title: 'softwares.softwares',

          service: config && config.service
                     ? config.service
                     : this.softwaresService,

          gridOptions: {
            columns: [
              new VsGridSimpleColumn({
                headerName: 'softwares.name',
                field: 'name',
                width: 100
              }),
              new VsGridSimpleColumn({
                headerName: 'softwares.identifier',
                field: 'identifier',
                width: 100
              }),
              new VsGridCheckboxColumn({
                headerName: 'softwares.active',
                field: 'isActive',
                width: 50
              }),
            ]
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
          this.softwaresService.onlyActiveSoftwares = false;
        }
      )
    );
  }


}
