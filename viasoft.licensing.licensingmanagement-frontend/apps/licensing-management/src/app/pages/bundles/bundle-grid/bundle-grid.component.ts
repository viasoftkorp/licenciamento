import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService, VsAuthorizationService } from '@viasoft/common';
import {
  VsGridCheckboxColumn,
  VsGridGetInput,
  VsGridGetResult,
  VsGridOptions,
  VsGridSimpleColumn,
} from '@viasoft/components';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';
import { BundleDeleteOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable, Subscription } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';

import { BundlesFormControlService } from '../bundles-form-control.service';
import { BundlesService } from '../bundles.service';


@Component({
  selector: 'app-bundle-grid',
  templateUrl: './bundle-grid.component.html',
  styleUrls: ['./bundle-grid.component.scss']
})
export class BundleGridComponent implements OnInit, OnDestroy {

  grid: VsGridOptions;
  subs: Array<Subscription> = [];
  finishAuthorizationRequest = false;
  policiesForGrid = {
    delete: false,
    update: false
  };

  constructor(
    private bundlesService: BundlesFormControlService,
    private bundles: BundlesService,
    private router: Router,
    private notification: MessageService,
    private readonly authorizationService: VsAuthorizationService
  ) {
    this.setAuthorizations();
  }

  ngOnInit() {
    this.subs.push(this.bundlesService.bundlesSubject.subscribe(
      () => {
        this.router.navigate(['/products/new']);
      }
    ));
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe);
  }

  configureGrid() {
    this.grid = new VsGridOptions();

    this.grid.columns = [
      new VsGridSimpleColumn({
        headerName: 'products.name',
        field: 'name',
        width: 100,
      }),
      new VsGridSimpleColumn({
        headerName: 'products.identifier',
        field: 'identifier',
        width: 100,
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
        width: 100,
      }),
      new VsGridCheckboxColumn({
        headerName: 'products.active',
        field: 'isActive',
        disabled: true,
        width: 50,
      })
    ];

    this.grid.enableFilter = true;
    this.grid.enableQuickFilter = false;

    if (this.policiesForGrid.update) {
      this.grid.edit = (i, data) => this.edit(data);
      this.grid.select = (i, data) => this.edit(data);
    } else {
      this.grid.select = (i, data) => this.edit(data);
    }

    if (this.policiesForGrid.delete) {
      this.grid.delete = (i, data) => this.delete(data);
    }

    this.grid.get = (input: VsGridGetInput) => this.get(input);
  }

  get(input: VsGridGetInput): Observable<any> {
    return this.bundles.getAll(
      input
    )
      .pipe(
        map((list: any) =>
          new VsGridGetResult(list.items, list.totalCount)
        )
      );
  }

  edit(data: any) {
    this.router.navigate(['/products', data.id]);
  }

  delete(data: any) {
    if (data.id) {
      this.subs.push(this.notification.confirm(
        'common.notification.action_irreversible',
        'common.notification.confirm_deletion|name:' + data.name
      )
        .pipe(
          filter(result => Boolean(result)),
          switchMap(() => this.bundles.delete(data.id))
        )
        .subscribe(finalValue => {
          finalValue = finalValue as BundleDeleteOutput;
          if (!finalValue.success && finalValue.errors.length > 0 && finalValue.errors[0].errorCodeReason === 'UsedByOtherRegister') {
            this.notification.error(
              'common.error.product_is_being_used',
              'common.error.could_not_delete|name:' + data.name
            );
          }
          this.grid.refresh();
        }));
    }
  }

  setAuthorizations(): void {
    this.authorizationService
      .isGrantedMap([Policies.UpdateBundle, Policies.DeleteBundle])
      .then((result: Map<string, boolean>) => {
        this.policiesForGrid = {
          update: result.get(Policies.UpdateBundle),
          delete: result.get(Policies.DeleteBundle)
        };
        this.finishAuthorizationRequest = true;
        this.configureGrid();
      }
      );
  }
}
