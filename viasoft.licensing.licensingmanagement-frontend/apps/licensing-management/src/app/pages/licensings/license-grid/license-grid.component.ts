import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';

import { MessageService, VsAuthorizationService } from '@viasoft/common';
import {VsGridGetInput, VsGridGetResult, VsGridOptions, VsGridSimpleColumn} from '@viasoft/components';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';
import {LicensedTenantViewGetAllInput,} from '@viasoft/licensing-management/app/tokens/inputs/licensedTenantView-get-all.input';
import { LicenseTenantDeleteOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { LicensedTenantViewService } from '../licensed-tenant-view.service';
import { LicensingsFormControlServices } from '../licensings-form-control.service';
import { LicensingsService } from '../licensings.service';

@Component({
  selector: 'app-license-grid',
  templateUrl: './license-grid.component.html',
  styleUrls: ['./license-grid.component.scss']
})

export class LicenseGridComponent implements OnInit, OnDestroy {

  public grid: VsGridOptions;
  public finishAuthorizationRequest = false;
  private subs: Array<Subscription> = [];
  private policies = {
    delete: false,
    update: false
  };

  constructor(
    private licenses: LicensingsFormControlServices,
    private router: Router,
    private notification: MessageService,
    private licenseService: LicensingsService,
    private licensedTenantViewService: LicensedTenantViewService,
    private authorizationService: VsAuthorizationService) {
    this.setAuthorizations();
  }

  ngOnInit() {
    this.subs.push(this.licenses.licensingsSubject.subscribe(
      () => {
        this.router.navigate(['/licensings/new']);
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
        headerName: 'TenantId',
        field: 'identifier',
        width: 150,
        sorting: {
          disable: true
        }
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.accountId',
        field: 'accountCompanyName',
        width: 140
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.cnpjCpf',
        field: 'licensedCnpjs',
        width: 100
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.administratorEmail',
        field: 'administratorEmail',
        width: 145
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.hardwareId',
        field: 'hardwareId',
        width: 145
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.status',
        field: 'status',
        width: 85,
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
      })
    ];

    if (this.policies.update) {
      this.grid.edit = (i, data) => this.edit(data);
      this.grid.select = (i, data) => this.edit(data);
    } else {
      this.grid.select = (i, data) => this.edit(data);
    }

    if (this.policies.delete) {
      this.grid.delete = (i, data) => this.delete(data);
    }
    this.grid.get = (input: VsGridGetInput) => this.get(input);
  }

  get(input: LicensedTenantViewGetAllInput): Observable<any> {
    return this.licensedTenantViewService.getAll(
      input
    )
      .pipe(
        map((list) =>
          new VsGridGetResult(list.items, list.totalCount)
        )
      );
  }

  edit(data: any) {
    this.router.navigate(['/licensings', data.licensedTenantId]);
  }

  delete(data: any) {
    if (data.licensedTenantId) {

      this.subs.push(this.notification.confirm(
        'common.notification.action_irreversible',
        'common.notification.confirm_deletion|name:' + data.identifier
      )
        .pipe(
          filter((result) => Boolean(result)),
          switchMap(() => this.licenseService.delete(data.licensedTenantId))
        )
        .subscribe(r => {
          r = r as LicenseTenantDeleteOutput;
          if (!r.success && r.errors.length > 0 && r.errors[0].errorCodeReason === 'UsedByOtherRegister') {
            this.notification.error(
              'common.error.software_is_being_used',
              'common.error.could_not_delete|name:' + data.customerCode
            );
          }
          this.grid.refresh();
        }));
    }
  }

  setAuthorizations(): void {
    this.authorizationService
      .isGrantedMap([Policies.UpdateLicense, Policies.DeleteLicense])
      .then((result: Map<string, boolean>) => {
        this.policies = {
          update: result.get(Policies.UpdateLicense),
          //delete: result.get(Policies.DeleteLicense)
          delete: false
        };
        this.finishAuthorizationRequest = true;
        this.configureGrid();
      }
      );
  }

}

