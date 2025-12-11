import { Injectable } from '@angular/core';
import { BundleServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';
import { AppServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';
import { BundleUpdateInput } from '@viasoft/licensing-management/clients/licensing-management';
import { BundleCreateInput } from '@viasoft/licensing-management/clients/licensing-management';
import { BundledAppCreateInput, BundledAppDeleteInput, BundleCreateOutput, BundleUpdateOutput, BundleDeleteOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { IVsBaseCrudService } from '@viasoft/common';
import { BundleGetAllInput } from '@viasoft/licensing-management/app/tokens/inputs/bundle-get-all.input';

@Injectable({
  providedIn: 'root'
})

export class BundlesService implements IVsBaseCrudService<
BundleCreateInput,
BundleCreateOutput,
BundleUpdateInput,
BundleUpdateOutput,
BundleDeleteOutput,
BundleCreateOutput,
BundleGetAllInput
> {

  getAllBundlesMinusLicensedBundlesFlag: boolean;
  licensedTenantId: string;

  constructor(private bundles: BundleServiceProxy, private apps: AppServiceProxy) { }

  getAll(
    input: BundleGetAllInput
  ) {
    if (this.getAllBundlesMinusLicensedBundlesFlag) {
      return this.getAllBundlesMinusLicensedBundles(this.licensedTenantId,
        input.filter,
        input.advancedFilter,
        input.sorting,
        input.skipCount,
        input.maxResultCount);
    }
    return this.bundles.getAll(this.licensedTenantId,
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount);
  }

  getById(id: string) {
    return this.bundles.getById(id);
  }

  create(bundle: BundleCreateInput) {
    return this.bundles.create(bundle);
  }

  update(bundle: BundleUpdateInput) {
    return this.bundles.update(bundle);
  }

  delete(id: string) {
    return this.bundles._delete(id);
  }

  getAllAppsInBundle(
    bundleId: string,
    filter: string,
    advancedFilter: string,
    sorting: string,
    skipCount: number,
    maxResultCount: number
  ) {
    return this.apps.getAllAppsInBundle(
      bundleId,
      filter,
      advancedFilter,
      sorting,
      skipCount,
      maxResultCount);
  }

  addAppToBundle(bundle: BundledAppCreateInput) {
    return this.bundles.addAppToBundle(bundle);
  }

  removeAppFromBundle(bundle: BundledAppDeleteInput) {
    return this.bundles.removeAppFromBundle(bundle);
  }

  getAllBundlesMinusLicensedBundles(
    licensedTenantId: string,
    filter: string,
    advancedFilter: string,
    sorting: string,
    skipCount: number,
    maxResultCount: number
    ) {
    return this.bundles.getAllBundlesMinusLicensedBundles(licensedTenantId, filter, advancedFilter, sorting, skipCount, maxResultCount);
  }

}
