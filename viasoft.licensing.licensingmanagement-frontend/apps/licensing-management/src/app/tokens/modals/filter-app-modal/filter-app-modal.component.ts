import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { JQQB_COND_OR, JQQB_OP_CONTAINS, JQQB_OP_EQUAL, MessageService, VsAuthorizationService, VsSubscriptionManager } from '@viasoft/common';
import { VsFilterGetItemsInput, VsFilterGetItemsOutput, VsFilterItem, VsGridGetInput, VsGridGetResult, VsGridOptions, VsGridSimpleColumn } from '@viasoft/components';
import { LicensedAppDeleteInput } from '@viasoft/licensing-licensingmanagement-licensedtenant';
import { LicensingsFormControlServices } from '@viasoft/licensing-management/app/pages/licensings/licensings-form-control.service';
import { LicensingsService } from '@viasoft/licensing-management/app/pages/licensings/licensings.service';
import { Observable, of } from 'rxjs';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Policies } from '../../classes/policies.class';
import { DomainsService } from '../../services/domains.service';

@Component({
  selector: 'app-filter-app-modal',
  templateUrl: './filter-app-modal.component.html',
  styleUrls: ['./filter-app-modal.component.scss']
})
export class FilterAppModalComponent implements OnInit {

  loading = true;
  grid: VsGridOptions;
  grandChildren: string;
  subs: VsSubscriptionManager = new VsSubscriptionManager();
  arrayOfLicensedApps: Array<string> = [];
  mapOfStatuses: Map<number, string>;
  listOfDomains: Array<string> = [];
  hasPoliciesForUpdateDetails = false;
  finishAuthorizationRequest = false;
  title: string;
  isDomainLoaded: boolean;

  constructor(
    private licensingsFormControl: LicensingsFormControlServices,
    private notification: MessageService,
    private licensingsService: LicensingsService,
    private domainsService: DomainsService,
    private readonly authorizationService: VsAuthorizationService,
    private translateService: TranslateService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.title = this.translateService.instant("apps.apps_in_product") + this.data.productName;
    this.getDomains();
  }

  ngOnInit() {
    this.getAuthorizations().then(result => {
      this.finishAuthorizationRequest = true;
      this.hasPoliciesForUpdateDetails = result;
      this.initComponentAfterGetAuthorizations();
    });
  }

  initComponentAfterGetAuthorizations() {
    this.mapOfStatuses = new Map();
    this.mapOfStatuses.set(0, 'licensings.blocked');
    this.mapOfStatuses.set(1, 'licensings.active');
    this.configureGrid();
    this.subs.add('grid-refresh', this.licensingsFormControl.gridRefresherSubject.subscribe(() => {
      this.grid.refresh();
    }));
    this.loading = false;
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  configureGrid() {
    this.grid = new VsGridOptions();
    this.grid.columns = [
      new VsGridSimpleColumn({
        headerName: 'apps.name',
        field: 'name',
        width: 260
      }),
      new VsGridSimpleColumn({
        headerName: 'apps.domains.domain',
        field: 'domain',
        translate: true,
        filterOptions: this.domainsService.getDomainsFilterOptions(this.listOfDomains),
        format: (value) => {
          if (this.listOfDomains[value] !== undefined) {
            return 'apps.domains.' + this.listOfDomains[value];
          }
        },
      }),
      new VsGridSimpleColumn({
        headerName: 'licensings.status',
        field: 'status',
        translate: true,
        filterOptions: {
          mode: 'selection',
          blockInput: true,
          operators: [JQQB_OP_EQUAL],
          getItems: () => this.getStatus(),
          getItemsFilterOperator: JQQB_OP_EQUAL,
        },
        format: (value) => {
          return this.mapOfStatuses.get(value);
        },
      }),
      new VsGridSimpleColumn({
        headerName: 'softwares.software',
        field: 'softwareName'
      })
    ];
    this.grid.enableFilter = true;
    this.grid.enableQuickFilter = false;
    this.grid.enableSorting = true;
    this.grid.sizeColumnsToFit = true;
    if (this.hasPoliciesForUpdateDetails) {
      this.grid.delete = (i, data) => this.delete(data);
    }
    this.grid.get = (input: VsGridGetInput) => this.get(input);
  }
  getStatus(): Observable<VsFilterGetItemsOutput> {
    return of({
      items: [
        { key: '0', value: 'licensings.blocked' } as VsFilterItem,
        { key: '1', value: 'licensings.active' } as VsFilterItem
      ],
      totalCount: 2
    } as VsFilterGetItemsOutput);
  }

  get(input: VsGridGetInput): Observable<any> {
    if (this.data.productId == null) {
      return of(
        new VsGridGetResult([], 0)
      );
    }
    return this.licensingsService.getAllLicensedAppsInBundle(
      this.licensingsService.licensedTenant.LicensedTenantId,
      this.data.productId,
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount
    )
      .pipe(
        map((list) =>
          new VsGridGetResult(list.items, list.totalCount)
        )
      );
  }

  delete(data: any) {
    if (data.id) {

      this.subs.add('notification-confirm', this.notification.confirm(
        'common.notification.action_irreversible',
        'common.notification.confirm_deletion|name:' + data.name
      )
        .pipe(
          filter((result) => Boolean(result)),
          switchMap(() => this.licensingsService.removeAppFromLicense({
            licensedTenantId: this.licensingsService.licensedTenant.LicensedTenantId,
            appId: data.id
          } as LicensedAppDeleteInput))
        )
        .subscribe(finalValue => {
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

  getDomains() {
    this.subs.add('get-domains', this.domainsService.getDomains().pipe(
      tap((value) => {
        for (const key in value) {
          if (key) {
            this.listOfDomains.push(key);
          }
        }
      }),
      tap(() => {
        this.isDomainLoaded = true;
      })).subscribe());
  }

  getAuthorizations() {
    return this.authorizationService.isGranted(Policies.UpdateLicense);
  }

}
