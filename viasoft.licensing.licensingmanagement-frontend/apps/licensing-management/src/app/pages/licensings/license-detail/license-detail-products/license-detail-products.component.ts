import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { JQQB_OP_EQUAL, JQQB_OP_GREATER, JQQB_OP_LESS, MessageService, VsAuthorizationService, VsSubscriptionManager } from '@viasoft/common';
import { VsDialog, VsGridGetInput, VsGridGetResult, VsGridOptions, VsGridSimpleColumn, VsTableAction } from '@viasoft/components';
import { LicensedAppDeleteInput, LicensedBundleDeleteInput, LicenseTenantDeleteOutput } from '@viasoft/licensing-licensingmanagement-licensedtenant';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';
import { LicensingFormCurrentTab } from '@viasoft/licensing-management/app/tokens/enum/licensing-form-current-tab.enum';
import { LicensingModel } from '@viasoft/licensing-management/app/tokens/enum/licensing-model.enum';
import { NamedUserTypes } from '@viasoft/licensing-management/app/tokens/enum/named-user-types.enum';
import { ProductType } from '@viasoft/licensing-management/app/tokens/enum/ProductType';
import { NamedUsersDialogDataInput } from '@viasoft/licensing-management/app/tokens/interfaces/named-users-dialog-data-input.interface';
import { FilterAppModalComponent } from '@viasoft/licensing-management/app/tokens/modals/filter-app-modal/filter-app-modal.component';
import { LicensesNumberSelectComponent } from '@viasoft/licensing-management/app/tokens/modals/licenses-number-select/licenses-number-select.component';
import { LooseAppsNumberSelectComponent } from '@viasoft/licensing-management/app/tokens/modals/loose-apps-number-select/loose-apps-number-select.component';
import { LicensedProductOutput } from '@viasoft/licensing-management/clients/licensing-management/model/LicensedProductOutput';
import { Observable } from 'rxjs';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { LicensingsFormControlServices } from '../../licensings-form-control.service';
import { LicensingsService } from '../../licensings.service';
import { NamedUsersComponent } from '../../named-users/named-users.component';

@Component({
  selector: 'app-license-detail-products',
  templateUrl: './license-detail-products.component.html',
  styleUrls: ['./license-detail-products.component.scss']
})
export class LicenseDetailProductsComponent implements OnInit {

  grid: VsGridOptions;
  id: string;
  expirationDate: string;
  updatingLicensedProduct = false;
  subs: VsSubscriptionManager = new VsSubscriptionManager();
  hasPoliciesForUpdateDetails = false;
  finishAuthorizationRequest = false;

  constructor(private licenses: LicensingsService,
              private notification: MessageService,
              private licensingsFormControl: LicensingsFormControlServices,
              private dialog: MatDialog,
              private readonly authorizationService: VsAuthorizationService,
              private readonly vsDialog: VsDialog) {
  }

  ngOnInit() {
    this.getAuthorizations().then(result => {
      this.finishAuthorizationRequest = true;
      this.hasPoliciesForUpdateDetails = result;
      this.initComponentAfterGetAuthorizations();
    });
  }

  initComponentAfterGetAuthorizations() {
    this.id = this.licenses.licensedTenant.LicensedTenantId;
    this.licenses.setCurrentTab(LicensingFormCurrentTab.Product);
    this.subs.add('grid-refresher', this.licensingsFormControl.gridRefresherSubject.subscribe(() => {
      this.grid.refresh();
    }));
    this.configureGrid();
  }

  ngOnDestroy(): void {
    this.subs.clear();
    this.licenses.noCurrentTab();
  }

  configureGrid() {
    this.grid = new VsGridOptions();

    this.grid.columns = [
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
        headerName: 'licensings.numberOfLicenses',
        field: 'numberOfLicenses',
        width: 100,
        kind: 'integer',
        filterOptions: {
          defaultOperator: JQQB_OP_EQUAL,
          operators: [JQQB_OP_EQUAL, JQQB_OP_GREATER, JQQB_OP_LESS]
        }
      }),
      new VsGridSimpleColumn({
        headerName: 'softwares.software',
        field: 'softwareName',
        width: 100
      })
    ];
    this.grid.enableSorting = true;
    this.grid.enableFilter = true;
    this.grid.enableQuickFilter = false;
    if (this.hasPoliciesForUpdateDetails) {
      this.grid.delete = (i, data) => this.delete(data);
      this.grid.edit = (i, data) => this.update(data)
    }
    this.grid.get = (input: VsGridGetInput) => this.get(input);

    this.grid.actions = [
      <VsTableAction> {
        icon: 'server',
        callback: (i: number, data: LicensedProductOutput) => this.openAppsGrid(data),
        condition: (i: number, data: LicensedProductOutput) => data.productType == ProductType.LicensedBundle,
        tooltip: 'apps.apps'
      },
      <VsTableAction> {
        icon: 'user-tag',
        callback: (i: number, data: LicensedProductOutput) => this.manageNamedLicenses(data),
        condition: (i: number, data: LicensedProductOutput) => data.licensingModel === LicensingModel.Named,
        tooltip: 'licensings.manageNamedUsersTitle'
      },
    ]
  }

  get(input: VsGridGetInput): Observable<any> {
    return this.licenses.getAllLicensedProduct(input, this.id)
      .pipe(
        map((list: any) => new VsGridGetResult(list.items, list.totalCount))
      );
  }

  delete(data: any) {
    if (data.id) {
      if (data.productType == ProductType.LicensedBundle){
        this.subs.add('notification-confirm-bundle', this.notification.confirm(
          'common.notification.action_irreversible',
          'common.notification.confirm_deletion|name:' + data.name
        )
          .pipe(
            filter(result => Boolean(result)),
            switchMap(() => this.licenses.removeBundleFromLicense({
              licensedTenantId: this.id, bundleId: data.productId
            } as LicensedBundleDeleteInput))
          )
          .subscribe(finalValue => {
            finalValue = finalValue as LicenseTenantDeleteOutput;
            if (!finalValue.success && finalValue.errors.length > 0 && finalValue.errors[0].errorCodeReason === 'UsedByOtherRegister') {
              this.notification.error(
                'common.error.product_is_being_used',
                'common.error.could_not_delete|name:' + data.name
              );
            }
            this.grid.refresh();
          }));
      } else {
        this.subs.add('notification-confirm-app', this.notification.confirm(
          'common.notification.action_irreversible',
          'common.notification.confirm_deletion|name:' + data.name
        )
          .pipe(
            filter(
              (result) => Boolean(result)
            ),
            switchMap(
              () => this.licenses.removeAppFromLicense({
                licensedTenantId: this.licenses.licensedTenant.LicensedTenantId,
                appId: data.productId
              } as LicensedAppDeleteInput)
            )
          ).subscribe(finalValue => {
            if (!finalValue.success && finalValue.errors.length > 0 && finalValue.errors[0].errorCodeReason === 'UsedByOtherRegister') {
              this.notification.error(
                'common.error.product_is_being_used',
                'common.error.could_not_delete|name:' + data.name
              );
            }
            if (!finalValue.success && finalValue.errors.length > 0 &&
              finalValue.errors[0].errorCodeReason === 'CantRemoveFromLicenseDefaultApp') {
              this.notification.error(
                'common.error.cant_remove_default_app',
                'common.error.could_not_delete|name:' + data.name
              );
            }
            this.grid.refresh();
          }));
      }
    }
  }

  update(data: any) {
    if (data.productType == ProductType.LicensedBundle){
      this.updateLicensedBundle(data);
    } else {
      this.updateLicensedApp(data, this.hasPoliciesForUpdateDetails);
    }
  }

  manageNamedLicenses(data: LicensedProductOutput) {
    this.vsDialog.open(NamedUsersComponent, <NamedUsersDialogDataInput> {
        licensedEntityId: data.id,
        licensedTenantId: this.licenses.licensedTenant.LicensedTenantId,
        licensingMode: data.licensingMode,
        namedUserType: data.productType == ProductType.LicensedBundle ? NamedUserTypes.LicensedBundleNamedUser : NamedUserTypes.LicensedAppNamedUser,
        numberOfLicenses: data.numberOfLicenses,
        deviceId: this.licenses.licensedTenant.deviceId,
        licensedTenantIdentifier: this.licenses.licensedTenant.licensedTenantIdentifier
      }, {maxWidth: '35vw'});
  }

  openAppsGrid(data: LicensedProductOutput) {
    this.vsDialog.open(FilterAppModalComponent, { productId: data.productId, productName: data.name}); }

  updateLicensedBundle(data: LicensedProductOutput) {
    const modal = this.dialog.open(LicensesNumberSelectComponent, {
      data: {
        bundleId: data.productId,
        bundleName: data.name,
        numberOfLicenses: data.numberOfLicenses,
        licensingMode: data.licensingMode,
        licensingModel: data.licensingModel,
        expirationDateTime: data.expirationDateTime,
        status: data.status
      }
    });
    this.subs.add('update-licensed-bundle-modal', modal.afterClosed().subscribe(result => {
      if (result) {
        this.subs.add('update-licensed-bundle', this.licenses.updateBundleFromLicense(result).subscribe(() => {
          this.updatingLicensedProduct = true;
          this.grid.refresh();
          this.licensingsFormControl.nextFilterAppDetail({
            bundleId: data.productId,
            licensingModel: data.licensingModel
          });
        }));
      }
    }));
  }

  updateLicensedApp(data: any, hasUpdatePermission): void {
    const modal = this.dialog.open(LooseAppsNumberSelectComponent, {
      data: {
        name: data.name,
        appId: data.productId,
        numberOfLicenses: data.numberOfLicenses,
        hasUpdatePermission,
        licensingModel: data.licensingModel,
        licensingMode: data.licensingMode,
        expirationDateTime: data.expirationDateTime,
        status: data.status,
        isCreate: false
      }
    });
    this.subs.add('update-licensed-app-modal', modal.afterClosed().subscribe((value) => {
      if (value) {
        this.subs.add('update-licensed-app', this.licenses.updateLooseAppFromLicense(value).subscribe(() => {
          this.grid.refresh();
        }));
      }
    }));
  }

  getAuthorizations() {
    return this.authorizationService.isGranted(Policies.UpdateLicense);
  }
}
