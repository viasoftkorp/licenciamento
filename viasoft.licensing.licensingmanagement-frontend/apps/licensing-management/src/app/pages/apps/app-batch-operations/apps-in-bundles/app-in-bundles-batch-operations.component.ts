import { Component, OnInit, OnDestroy } from '@angular/core';
import { RequestFromCheckBoxGrid } from '@viasoft/licensing-management/app/tokens/utils/grid-check-box-validation';
import { AppViewService } from '@viasoft/licensing-management/app/tokens/services/app-view.service';
import { MessageService, VsAuthorizationService, VsSubscriptionManager } from '@viasoft/common';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { concatMap, mergeMap, tap, switchMap } from 'rxjs/operators';
import { BatchOperationsInput } from '@viasoft/licensing-management/clients/licensing-management/model/batchOperationsInput';
import { of } from 'rxjs';
import { LicenseBatchOperationsService } from '@viasoft/licensing-management/app/tokens/license-batch-operations/license-batch-operations.service';
import { BatchOperationsLoadingComponent } from '@viasoft/licensing-management/app/tokens/license-batch-operations/modals/batch-operations-loading/batch-operations-loading.component';
import { BundleViewService } from '@viasoft/licensing-management/app/tokens/services/bundle-view.service';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';

@Component({
  selector: 'app-app-in-bundles-batch-operations',
  templateUrl: './app-in-bundles-batch-operations.component.html',
  styleUrls: ['./app-in-bundles-batch-operations.component.scss']
})
export class AppInBundlesBatchOperationsComponent implements OnInit, OnDestroy {

  public requestApp: RequestFromCheckBoxGrid = {
    allSelected: false,
    hasItemSelected: false,
    selectedIds: [],
    unSelectedIds: [],
    currentGet: undefined
  };

  public requestBundles: RequestFromCheckBoxGrid = {
    allSelected: false,
    hasItemSelected: false,
    selectedIds: [],
    unSelectedIds: [],
    currentGet: undefined
  };
  hasPermission = false;
  private subs = new VsSubscriptionManager();

  constructor(private appViewService: AppViewService,
    private bundleViewService: BundleViewService,
    private licenseBatchOperationsService: LicenseBatchOperationsService,
    private dialog: MatDialog,
    private notification: MessageService,
    private authorizationService: VsAuthorizationService) {
  }

  ngOnInit() {
    this.getAuthorizations().then((res) => {
      this.hasPermission = res;
      this.appsSubs();
      this.bundleSubs();
    });
  }

  ngOnDestroy() {
    this.subs.clear();
  }

  clearGridStates() {
    this.appViewService.clearStates();
    this.bundleViewService.clearStates();
  }

  appsSubs() {
    this.subs.add('appsGridCheckBox', this.appViewService.getGridCheckBoxForResquest().subscribe((res: RequestFromCheckBoxGrid) => {
      this.requestApp = res;
    }));
  }

  bundleSubs() {
    this.subs.add('bundlesGridCheckBox', this.bundleViewService.getGridCheckBoxForResquest().subscribe((res: RequestFromCheckBoxGrid) => {
      this.requestBundles = res;
    }));
  }

  validationDisabledButton() {
    if (!this.requestApp.hasItemSelected || !this.requestBundles.hasItemSelected) {
      return true;
    }
    return false;
  }

  save() {
    const input: BatchOperationsInput = {
      idsToInsert: {
        allSelected: this.requestApp.allSelected,
        ids: this.requestApp.selectedIds,
        unselectedList: this.requestApp.unSelectedIds,
        advancedFilter: this.requestApp.currentGet ? this.requestApp.currentGet.advancedFilter : undefined
      },
      idsWhereTheyWillBeInserted: {
        allSelected: this.requestBundles.allSelected,
        ids: this.requestBundles.selectedIds,
        unselectedList: this.requestBundles.unSelectedIds,
        advancedFilter: this.requestBundles.currentGet ? this.requestBundles.currentGet.advancedFilter : undefined
      },
      numberOfLicenses: 0
    }
    let modalLoading: MatDialogRef<BatchOperationsLoadingComponent>;
    let currentResult;
    modalLoading = this.dialog.open(BatchOperationsLoadingComponent);
    modalLoading.disableClose = true;
    this.subs.add('insertAppsInBundles',
      this.licenseBatchOperationsService.insertAppsInBundles(input).pipe(
        tap(result => {
          currentResult = result;
        }),
        switchMap(() => {
          return of(this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation());
        }),
        concatMap(() => {
          if (currentResult.length > 0) {
            modalLoading.close();
            return this.notification.confirm('products.addAppInLincense');
          }
          return of(undefined);
        }),
        mergeMap((res) => {
          if (res) {
            modalLoading = this.dialog.open(BatchOperationsLoadingComponent);
            modalLoading.disableClose = true;
            return this.licenseBatchOperationsService.insertAppsFromBundlesInLicenses(currentResult);
          }
          return of(undefined);
        })
      ).subscribe(
        () => { },
        () => {
          this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation(true);
        },
        () => {
          this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation();
        }
      )
    );
  }

  getAuthorizations() {
    return this.authorizationService.isGranted(Policies.BatchOperationInsertAppsInBundles);
  }

}
