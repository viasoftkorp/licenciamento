import { Component, OnDestroy, OnInit } from '@angular/core';
import { MessageService, VsAuthorizationService } from '@viasoft/common';
import {
  VsDialog,
  VsGridCheckboxColumn,
  VsGridGetInput,
  VsGridGetResult,
  VsGridOptions,
  VsGridSimpleColumn,
} from '@viasoft/components';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';
import { SoftwareDeleteOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable, Subscription } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';

import { SoftwareGetAllInput } from '../../../tokens/inputs/software-get-all.input';
import { SoftwareDetailComponent } from '../software-detail/software-detail.component';
import { SoftwaresFormControlService } from '../softwares-form-control.service';
import { SoftwaresService } from '../softwares.service';

@Component({
  selector: 'app-software-grid',
  templateUrl: './software-grid.component.html',
  styleUrls: ['./software-grid.component.scss']
})
export class SoftwareGridComponent implements OnInit, OnDestroy {

  grid: VsGridOptions;
  id: string;
  subs: Array<Subscription> = [];
  policiesForGrid = {
    delete: false,
    update: false
  };
  finishAuthorizationRequest = false;

  constructor(
    private softwareFormControlService: SoftwaresFormControlService,
    private notification: MessageService,
    private softwareService: SoftwaresService,
    private vsDialog: VsDialog,
    private readonly authorizationService: VsAuthorizationService) {
    this.setAuthorizations()
  }

  ngOnInit() {
    this.subs.push(this.softwareFormControlService.gridRefreshSubject.subscribe(
      () => {
        this.grid.refresh();
      }
    ));
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  configureGrid() {
    this.grid = new VsGridOptions();
    this.grid.enableQuickFilter = false;

    this.grid.columns = [
      new VsGridSimpleColumn({
        headerName: 'softwares.name',
        field: 'name',
        width: 100,
      }),
      new VsGridSimpleColumn({
        headerName: 'softwares.identifier',
        field: 'identifier',
        width: 100,
      }),
      new VsGridCheckboxColumn({
        headerName: 'softwares.active',
        field: 'isActive',
        disabled: true,
        width: 50,
      })
    ];

    if (this.policiesForGrid.update) {
      this.grid.edit = (i, data) => this.edit(data, true);
      this.grid.select = (i, data) => this.edit(data, true);
    } else {
      this.grid.select = (i, data) => this.edit(data, false);
    }

    if (this.policiesForGrid.delete) {
      this.grid.delete = (i, data) => this.delete(data);
    }
    this.grid.get = (input: VsGridGetInput) => this.get(input);
  }

  get(input: SoftwareGetAllInput): Observable<any> {
    return this.softwareService.getAll(
      input
    )
      .pipe(
        map((list) =>
          new VsGridGetResult(list.items, list.totalCount)
        )
      );
  }

  edit(data: any, hasUpdatePermission: boolean) {
    this.vsDialog.open(SoftwareDetailComponent, { id: data.id, hasUpdatePermission }, { maxWidth: "20vw"});
  }

  delete(data: any) {
    if (data.id) {
      this.subs.push(this.notification.confirm(
        'common.notification.action_irreversible',
        'common.notification.confirm_deletion|name:' + data.name
      )
        .pipe(
          filter(result => Boolean(result)),
          switchMap(() => this.softwareService.delete(data.id))
        )
        .subscribe(finalValue => {
          finalValue = finalValue as SoftwareDeleteOutput;
          if (!finalValue.success && finalValue.errors.length > 0 && finalValue.errors[0].errorCodeReason === 'UsedByOtherRegister') {
            this.notification.error(
              'common.error.software_is_being_used',
              'common.error.could_not_delete|name:' + data.name
            );
          }
          this.grid.refresh();
        }));
    }
  }

  setAuthorizations(): void {
    this.authorizationService
      .isGrantedMap([Policies.UpdateSoftware, Policies.DeleteSoftware])
      .then((result: Map<string, boolean>) => {
        this.policiesForGrid = {
          update: result.get(Policies.UpdateSoftware),
          delete: result.get(Policies.DeleteSoftware)
        };
        this.finishAuthorizationRequest = true;
        this.configureGrid();
      }
      );
  }
}
