import { Component, OnInit, OnDestroy } from '@angular/core';
import { RequestFromCheckBoxGrid } from '@viasoft/licensing-management/app/tokens/utils/grid-check-box-validation';
import { LicenseTenantViewService } from '@viasoft/licensing-management/app/tokens/services/licensed-view.service';
import { AppViewService } from '@viasoft/licensing-management/app/tokens/services/app-view.service';
import { VsAuthorizationService, VsSubscriptionManager } from '@viasoft/common';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { BatchOperationsLicenseNumberSelectComponent } from '@viasoft/licensing-management/app/tokens/license-batch-operations/modals/batch-operations-license-number/bundles-select/batch-operations-license-number-select.component';
import { concatMap, mergeMap } from 'rxjs/operators';
import { BatchOperationsInput } from '@viasoft/licensing-management/clients/licensing-management/model/batchOperationsInput';
import { of } from 'rxjs';
import { LicenseBatchOperationsService } from '@viasoft/licensing-management/app/tokens/license-batch-operations/license-batch-operations.service';
import { BatchOperationsLoadingComponent } from '@viasoft/licensing-management/app/tokens/license-batch-operations/modals/batch-operations-loading/batch-operations-loading.component';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';

@Component({
  selector: 'app-app-in-licenses-batch-operations',
  templateUrl: './app-in-licenses-batch-operations.component.html',
  styleUrls: ['./app-in-licenses-batch-operations.component.scss']
})
export class AppInLicensesBatchOperationsComponent implements OnInit, OnDestroy {

  public requestApp: RequestFromCheckBoxGrid = {
    allSelected: false,
    hasItemSelected: false,
    selectedIds: [],
    unSelectedIds: [],
    currentGet: undefined
  };

  public requestLicense: RequestFromCheckBoxGrid = {
    allSelected: false,
    hasItemSelected: false,
    selectedIds: [],
    unSelectedIds: [],
    currentGet: undefined
  };
  hasPermission = false;
  private subs = new VsSubscriptionManager();

  constructor(private licenseTenantViewService: LicenseTenantViewService, private appViewService: AppViewService,
    private licenseBatchOperationsService: LicenseBatchOperationsService, private dialog: MatDialog,
    private authorizationService: VsAuthorizationService) {
  }

  ngOnInit() {
    this.getAuthorizations().then((res) => {
      this.hasPermission = res;
      this.appsSubs();
      this.licensedSubs();
    })
  }

  ngOnDestroy() {
    this.subs.clear();
  }

  clearGridStates() {
    this.licenseTenantViewService.clearStates();
    this.appViewService.clearStates();
  }

  appsSubs() {
    this.subs.add('appsGridCheckBox', this.appViewService.getGridCheckBoxForResquest().subscribe((res: RequestFromCheckBoxGrid) => {
      this.requestApp = res;
    }));
  }

  licensedSubs() {
    this.subs.add('licensesGridCheckBox', this.licenseTenantViewService.getGridCheckBoxForResquest().subscribe((res: RequestFromCheckBoxGrid) => {
      this.requestLicense = res;
    }));
  }

  validationDisabledButton() {
    if (!this.requestApp.hasItemSelected || !this.requestLicense.hasItemSelected) {
      return true;
    }
    return false;
  }

  save() {
    let modalLoading: MatDialogRef<BatchOperationsLoadingComponent>;
    this.subs.add('afterCloseSubs', this.dialog.open(BatchOperationsLicenseNumberSelectComponent, { maxWidth: '40vw' }).afterClosed()
      .pipe(
        concatMap((res) => {
          if (res) {
            const input: BatchOperationsInput = {
              idsToInsert: {
                allSelected: this.requestApp.allSelected,
                ids: this.requestApp.selectedIds,
                unselectedList: this.requestApp.unSelectedIds,
                advancedFilter: this.requestApp.currentGet ? this.requestApp.currentGet.advancedFilter : undefined
              },
              idsWhereTheyWillBeInserted: {
                allSelected: this.requestLicense.allSelected,
                ids: this.requestLicense.selectedIds,
                unselectedList: this.requestLicense.unSelectedIds,
                advancedFilter: this.requestLicense.currentGet ? this.requestLicense.currentGet.advancedFilter : undefined
              },
              numberOfLicenses: res
            }
            return of(input);
          }
          return of(undefined);
        })
      )
      .pipe(
        mergeMap((res) => {
          if (res) {
            modalLoading = this.dialog.open(BatchOperationsLoadingComponent);
            modalLoading.disableClose = true;
            return this.licenseBatchOperationsService.insertAppsInLicenses(res);
          }
          return of(undefined);
        })
      )
      .subscribe(
        () => { },
        () => {
          this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation(true);
        },
        () => {
          this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation();
        }
      )
    )
  }

  getAuthorizations() {
    return this.authorizationService.isGranted(Policies.BatchOperationInsertAppsInLicenses);
  }

}
