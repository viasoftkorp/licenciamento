import { Injectable } from '@angular/core';
import { LicensedTenantViewServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';
import { LicensedTenantViewGetAllInput } from '@viasoft/licensing-management/app/tokens/inputs/licensedTenantView-get-all.input';
import { GridCheckBoxValidation } from '../utils/grid-check-box-validation';

@Injectable({
  providedIn: 'root'
})
export class LicenseTenantViewService extends GridCheckBoxValidation {

  constructor(private licensedTenantViewServiceProxy: LicensedTenantViewServiceProxy) {
    super();
  }

  getAll(input: LicensedTenantViewGetAllInput) {
    return this.licensedTenantViewServiceProxy.getAll(
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount
    );
  }

}
