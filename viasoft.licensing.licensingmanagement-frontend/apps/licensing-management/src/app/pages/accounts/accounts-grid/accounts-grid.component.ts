import { Component, OnDestroy, OnInit } from '@angular/core';
import { MessageService, VsAuthorizationService } from '@viasoft/common';
import {
  VsDialog,
  VsGridCpfOrCnpjColumn,
  VsGridGetInput,
  VsGridGetResult,
  VsGridOptions,
  VsGridPhoneColumn,
  VsGridSimpleColumn,
} from '@viasoft/components';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';
import { AccountDeleteOutput, OperationValidation } from '@viasoft/licensing-management/clients/licensing-management';
import { Subscription } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';

import { AccountsDetailComponent } from '../accounts-detail/accounts-detail.component';
import { AccountsFormControlService } from '../accounts-form-control.service';
import { AccountsService } from '../accounts.service';

@Component({
  selector: 'app-accounts-grid',
  templateUrl: './accounts-grid.component.html',
  styleUrls: ['./accounts-grid.component.scss']
})
export class AccountsGridComponent implements OnInit, OnDestroy {

  grid: VsGridOptions;
  subs: Array<Subscription> = [];
  policiesForGrid = {
    delete: false,
    update: false
  };
  finishAuthorizationRequest = false;

  constructor(private accountsService: AccountsService,
    private vsDialog: VsDialog,
    private accountsFormControlService: AccountsFormControlService,
    private notification: MessageService,
    private readonly authorizationService: VsAuthorizationService) {
    this.setAuthorizations();
  }

  ngOnInit() {
    this.subs.push(this.accountsFormControlService.refreshGridSubject.subscribe(
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
    this.grid.actions = [
      {
        icon: 'ban',
        callback: (_,unit) => this.get(unit),
        tooltip: 'auditing.seeMore'
      }
    ]

    this.grid.columns = [
      new VsGridSimpleColumn({
        headerName: 'accounts.details.companyName',
        field: 'companyName'
      }),
      new VsGridSimpleColumn({
        headerName: 'accounts.details.tradingName',
        field: 'tradingName'
      }),
      new VsGridCpfOrCnpjColumn({
        headerName: 'accounts.details.cnpjCpf',
        field: 'cnpjCpf'
      }),
      new VsGridSimpleColumn({
        headerName: 'accounts.details.email',
        field: 'email'
      }),
      new VsGridPhoneColumn({
        headerName: 'accounts.details.phone',
        field: 'phone'
      }),
      new VsGridSimpleColumn({
        headerName: 'accounts.details.accountStatus.status',
        field: 'status',
        disableFilter: true,
        sorting: {
          disable: true
        },
        translate: true,
        format: (value) => {
          if (value === 0) {
            return value = 'accounts.details.accountStatus.blocked';
          } else {
            return value = 'accounts.details.accountStatus.active';
          }
        }
      })
    ];
    this.grid.get = (input: VsGridGetInput) => this.get(input);

    if (this.policiesForGrid.update) {
      this.grid.edit = (i, data) => this.openSideModal(data, true);
      this.grid.select = (i, data) => this.openSideModal(data, true);
    } else {
      this.grid.select = (i, data) => this.openSideModal(data, false);
    }

    if (this.policiesForGrid.delete) {
      this.grid.delete = (i, data) => this.delete(data);
    }
  }

  get(input: VsGridGetInput) {
    return this.accountsService.getAll(input).pipe(
      map(
        (list) => new VsGridGetResult(list.items, list.totalCount)
      )
    );
  }

  openSideModal(data: any, hasUpdatePermission: boolean) {
    this.vsDialog.open(AccountsDetailComponent, {id: data.id, hasUpdatePermission} );
  }

  delete(data: any) {
    if (data.id) {
      this.subs.push(this.notification.confirm(
        'common.notification.action_irreversible',
        'common.notification.confirm_deletion|name:' + data.companyName
      )
        .pipe(
          filter((result) => Boolean(result)),
          switchMap(() => this.accountsService.delete(data.id))
        )
        .subscribe((finalValue: AccountDeleteOutput) => {
            if (!finalValue.success &&
              finalValue.errors.length > 0 &&
              finalValue.errors[0].errorCode === OperationValidation.UsedByOtherRegister) {
              this.notification.error(
                'common.error.account_is_being_used|name:' + finalValue.errors[0].message,
                'common.error.could_not_delete|name:' + data.companyName
              );
            }
            this.grid.refresh();
          }
        ));
    }
  }

  setAuthorizations(): void {
    this.authorizationService
      .isGrantedMap([Policies.UpdateAccount, Policies.DeleteAccount])
      .then((result: Map<string, boolean>) => {
        this.policiesForGrid = {
          update: result.get(Policies.UpdateAccount),
          delete: result.get(Policies.DeleteAccount)
        };
        this.finishAuthorizationRequest = true;
        this.configureGrid();
      }
      );
  }
}
