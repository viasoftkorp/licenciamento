import { Component, OnInit, OnDestroy } from '@angular/core';
import { BundleViewService } from '@viasoft/licensing-management/app/tokens/services/bundle-view.service';
import { LicenseTenantViewService } from '@viasoft/licensing-management/app/tokens/services/licensed-view.service';
import { of } from 'rxjs';
import { RequestFromCheckBoxGrid } from '@viasoft/licensing-management/app/tokens/utils/grid-check-box-validation';
import { LicenseBatchOperationsService } from '@viasoft/licensing-management/app/tokens/license-batch-operations/license-batch-operations.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { BatchOperationsLicenseNumberSelectComponent } from '@viasoft/licensing-management/app/tokens/license-batch-operations/modals/batch-operations-license-number/bundles-select/batch-operations-license-number-select.component';
import { VsAuthorizationService, VsSubscriptionManager } from '@viasoft/common';
import { concatMap, mergeMap } from 'rxjs/operators';
import { BatchOperationsInput } from '@viasoft/licensing-management/clients/licensing-management/model/batchOperationsInput';
import { BatchOperationsLoadingComponent } from '@viasoft/licensing-management/app/tokens/license-batch-operations/modals/batch-operations-loading/batch-operations-loading.component';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';

@Component({
  selector: 'app-bundle-batch-operations',
  templateUrl: './bundle-batch-operations.component.html',
  styleUrls: ['./bundle-batch-operations.component.scss']
})
export class BundleBatchOperationsComponent implements OnInit, OnDestroy {

  public requestBundle: RequestFromCheckBoxGrid = {
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

  constructor(private bundleViewService: BundleViewService, private licenseTenantViewService: LicenseTenantViewService,
    private licenseBatchOperationsService: LicenseBatchOperationsService, private dialog: MatDialog, private authorizationService: VsAuthorizationService) { }

  ngOnInit() {
    this.getAuthorizations().then((res) => {
      this.hasPermission = res;
      this.licensedSubs();
      this.bundlesSubs();
    })
  }

  ngOnDestroy() {
    this.bundleViewService.clearStates();
    this.licenseTenantViewService.clearStates();
    this.subs.clear();
  }

  bundlesSubs() {
    this.subs.add('bundlesGridCheckBox', this.bundleViewService.getGridCheckBoxForResquest().subscribe((res: RequestFromCheckBoxGrid) => {
      this.requestBundle = res;
    }));
  }

  licensedSubs() {
    this.subs.add('licensesGridCheckBox', this.licenseTenantViewService.getGridCheckBoxForResquest().subscribe((res: RequestFromCheckBoxGrid) => {
      this.requestLicense = res;
    }));
  }

  save() {
    this.openLicenseNumberModal();
  }

  openLicenseNumberModal() {
    let modalLoading: MatDialogRef<BatchOperationsLoadingComponent>;
    this.subs.add('afterCloseSubs', this.dialog.open(BatchOperationsLicenseNumberSelectComponent, { maxWidth: '40vw' }).afterClosed()
      .pipe(
        concatMap((res) => {
          if (res) {
            const input: BatchOperationsInput = {
              idsToInsert: {
                allSelected: this.requestBundle.allSelected,
                ids: this.requestBundle.selectedIds,
                unselectedList: this.requestBundle.unSelectedIds,
                advancedFilter: this.requestBundle.currentGet ? this.requestBundle.currentGet.advancedFilter : undefined
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
      ).pipe(
        mergeMap((res) => {
          if (res) {
            modalLoading = this.dialog.open(BatchOperationsLoadingComponent);
            modalLoading.disableClose = true;
            return this.licenseBatchOperationsService.insertBundlesInLicenses(res);
          }
          return of(undefined);
        })
      ).subscribe({
        next: () => { },
        error: () => {
          this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation(true);
        },
        complete: () => {
          this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation();
        }
      })
    );
  }

  getAuthorizations() {
    return this.authorizationService.isGranted(Policies.BatchOperationInsertBundlesInLicenses);
  }

}
