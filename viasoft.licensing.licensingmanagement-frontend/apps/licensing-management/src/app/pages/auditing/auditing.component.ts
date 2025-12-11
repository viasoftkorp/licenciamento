import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import {AuditingLogOutput} from "@viasoft/licensing-management/clients/licensing-management";
import {AuditingSeeMoreComponent} from "@viasoft/licensing-management/app/tokens/modals/auditing-see-more/auditing-see-more.component";
import {
  VsDialog,
  VsGridDateTimeColumn,
  VsGridGetInput,
  VsGridGetResult,
  VsGridOptions,
  VsGridSimpleColumn,
} from '@viasoft/components';
import { AuditingService } from './auditing.service';


@Component({
  selector: 'app-auditing',
  templateUrl: './auditing.component.html',
  styleUrls: ['./auditing.component.scss']
})

export class AuditingComponent implements OnInit {

  public auditingGrid: VsGridOptions;

  constructor(
    private auditingService: AuditingService,
    private vsDialog: VsDialog
  ) { }

  ngOnInit() {
    this.configureGrid();
  }

  configureGrid() {
    this.auditingGrid = new VsGridOptions();
    this.auditingGrid.enableQuickFilter = false;
    this.auditingGrid.columns = [
      new VsGridSimpleColumn({
        headerName: 'Auditing.UserName',
        field: 'userName',
        width: 70
      }),
      new VsGridDateTimeColumn({
        headerName: 'Auditing.DateTime',
        field: 'dateTime',
        width: 80
      }),
      new VsGridSimpleColumn({
        headerName: 'Auditing.Type',
        field: 'type',
        width: 80,
        disableFilter: true,
        translate: true,
        format: (value) => {
          switch(value) {
            case 0 :
              return 'Auditing.BatchAction'
          }
        }
      }),
      new VsGridSimpleColumn({
        headerName: 'Auditing.ActionName',
        field: 'actionName',
        width: 160
      }),
      new VsGridSimpleColumn({
        headerName: 'Auditing.Details',
        field: 'details'
      }),
    ];
    this.auditingGrid.actions = [
      {
        icon: 'magnifying-glass',
        tooltip: 'Auditing.Modal.AuditingSeeMore',
        callback: (_, auditing) => this.seeMoreAuditing(auditing),
      },
    ]
    this.auditingGrid.sizeColumnsToFit = false;
    this.auditingGrid.get = (input: VsGridGetInput) => this.get(input);
    this.auditingGrid.select = (_, auditing: AuditingLogOutput) => this.seeMoreAuditing(auditing);
  }

  private get(input: VsGridGetInput): Observable<any> {
    return this.auditingService.getAll(input)
    .pipe(
      map((list: any) => {
        return new VsGridGetResult(list.items, list.totalCount)
      })
    );
  }

  private seeMoreAuditing(auditing: AuditingLogOutput) {
   this.vsDialog.open(AuditingSeeMoreComponent, {
     userName: auditing.userName,
     dateTime: auditing.dateTime,
     type: auditing.type,
     actionName: auditing.actionName,
     details: auditing.details
   })
     .afterClosed();
  }

}
