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
import { filter, map, switchMap, tap } from 'rxjs/operators';

import { DomainsService } from '../../../tokens/services/domains.service';
import { AppDetailComponent } from '../app-detail/app-detail.component';
import { AppsFormControlService } from '../apps-form-control.service';
import { AppsService } from '../apps.service';

@Component({
  selector: 'app-app-grid',
  templateUrl: './app-grid.component.html',
  styleUrls: ['./app-grid.component.scss']
})
export class AppGridComponent implements OnInit, OnDestroy {

  subs: Array<Subscription> = [];
  grid: VsGridOptions;
  listOfDomains: Array<string> = [];
  isAlreadyConfigured = false;
  policiesForGrid = {
    delete: false,
    update: false
  };
  finishAuthorizationRequest = false;

  constructor(
    private appGridService: AppsService,
    private appsService: AppsFormControlService,
    private notification: MessageService,
    private vsDialog: VsDialog,
    private domainsService: DomainsService,
    private readonly authorizationService: VsAuthorizationService
  ) {
    this.setAuthorizations();
  }

  ngOnInit() {
    this.subs.push(this.appsService.gridRefreshSubject.subscribe(
      () => {
        this.grid.refresh();
      }
    ));
    this.subs.push(this.getDomains().subscribe(() => { }));
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  configureGrid() {
    this.grid = new VsGridOptions();
    this.grid.enableQuickFilter = false;

    this.grid.columns = [
      new VsGridSimpleColumn({
        headerName: 'apps.name',
        field: 'name',
        width: 100,
      }),
      new VsGridSimpleColumn({
        headerName: 'apps.identifier',
        field: 'identifier',
        width: 100,
      }),
      new VsGridSimpleColumn({
        headerName: 'apps.domains.domain',
        field: 'domain',
        width: 50,
        disableFilter: true,
        translate: true,
        format: (value) => {
          if (this.listOfDomains[value] !== undefined) {
            return 'apps.domains.' + this.listOfDomains[value];
          }
        }
      }),
      new VsGridCheckboxColumn({
        headerName: 'apps.active',
        field: 'isActive',
        disabled: true,
        width: 50,
        disableFilter: true
      }),
      new VsGridCheckboxColumn({
        headerName: 'apps.default',
        field: 'isDefault',
        disabled: true,
        width: 50,
        sorting: {
          disable: true
        },
        disableFilter: true
      }),
      new VsGridSimpleColumn({
        headerName: 'apps.software',
        field: 'softwareName',
        width: 100,
        sorting: {
          disable: true
        },
        disableFilter: true
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

  get(input: VsGridGetInput): Observable<any> {
    return this.appGridService.getAll(
      input
    )
      .pipe(
        map((list) =>
          new VsGridGetResult(list.items, list.totalCount)
        )
      );
  }

  edit(data: any, hasUpdatePermission: boolean) {
    this.vsDialog.open(AppDetailComponent, { id: data.id, hasUpdatePermission }, { maxWidth: '20vw' });
  }

  delete(data: any) {
    if (data.id) {
      this.subs.push(this.notification.confirm(
        'common.notification.action_irreversible',
        'common.notification.confirm_deletion|name:' + data.name
      )
        .pipe(
          filter((result) => Boolean(result)),
          switchMap(() => this.appGridService.delete(data.id))
        )
        .subscribe(finalValue => {
          finalValue = finalValue as SoftwareDeleteOutput;
          if (!finalValue.success && finalValue.errors.length > 0 && finalValue.errors[0].errorCodeReason === 'UsedByOtherRegister') {
            this.notification.error(
              'common.error.app_is_being_used',
              'common.error.could_not_delete|name:' + data.name
            );
          }
          this.grid.refresh();
        }));
    }
  }

  getDomains() {
    return this.domainsService.getDomains().pipe(
      tap((value) => {
        for (const key in value) {
          if (key) {
            this.listOfDomains.push(key);
          }
        }
      }
      ));
  }

  setAuthorizations(): void {
    this.authorizationService
      .isGrantedMap([Policies.UpdateApp, Policies.DeleteApp])
      .then((result: Map<string, boolean>) => {
        this.policiesForGrid = {
          update: result.get(Policies.UpdateApp),
          delete: result.get(Policies.DeleteApp)
        };
        this.finishAuthorizationRequest = true;
        this.configureGrid();
      }
      );
  }

}

