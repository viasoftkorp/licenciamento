import { Injectable } from '@angular/core';

import { LicensedTenantBatchOperationsServiceProxy, BundledAppCreateInput, BundledAppDeleteInput, LicensedBundleApp } from '@viasoft/licensing-management/clients/licensing-management';
import { BatchOperationsInput } from '@viasoft/licensing-management/clients/licensing-management/model/batchOperationsInput';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LicenseBatchOperationsService {

  private errorInBatchOperation: Subject<boolean> = new Subject();

  constructor(private licensedTenantBatchOperationsServiceProxy: LicensedTenantBatchOperationsServiceProxy) { }

  public insertNewAppFromBundleInLicenses(newAppAddedInBundleInput: BundledAppCreateInput) {
    return this.licensedTenantBatchOperationsServiceProxy.insertNewAppFromBundleInLicenses(newAppAddedInBundleInput);
  }

  public insertBundlesInLicenses(batchOperationsInput: BatchOperationsInput) {
    return this.licensedTenantBatchOperationsServiceProxy.insertBundlesInLicenses(batchOperationsInput);
  }

  public insertAppsInLicenses(batchOperationsInput: BatchOperationsInput) {
    return this.licensedTenantBatchOperationsServiceProxy.insertAppsInLicenses(batchOperationsInput);
  }

  public insertAppsInBundles(batchOperationsInput: BatchOperationsInput) {
    return this.licensedTenantBatchOperationsServiceProxy.insertAppsInBundles(batchOperationsInput);
  }

  public insertAppsFromBundlesInLicenses(licensedBundlesApps: Array<LicensedBundleApp>) {
    return this.licensedTenantBatchOperationsServiceProxy.insertAppsFromBundlesInLicenses(licensedBundlesApps);
  }

  public removeAppFromBundleInLicenses(removeAppInBundleInput: BundledAppDeleteInput) {
    return this.licensedTenantBatchOperationsServiceProxy.removeAppFromBundleInLicenses(removeAppInBundleInput);
  }

  public emitErrorOrFinishInBatchOperation(hasError = false) {
    this.errorInBatchOperation.next(hasError);
  }

  public getErrorOrFinishInBatchOperation() {
    return this.errorInBatchOperation;
  }

}
