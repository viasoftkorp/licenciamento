import { Injectable } from '@angular/core';
import { BundleGetAllInput } from '../inputs/bundle-get-all.input';
import { BundleServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';
import { GridCheckBoxValidation } from '../utils/grid-check-box-validation';

@Injectable({
  providedIn: 'root'
})
export class BundleViewService extends GridCheckBoxValidation {

  constructor(private bundles: BundleServiceProxy) {
    super();
  }

  getAll(
    input: BundleGetAllInput
  ) {
    return this.bundles.getAll(undefined,
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount);
  }

}
