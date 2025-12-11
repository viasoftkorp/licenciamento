import { Injectable } from '@angular/core';
import { LicensedTenantViewServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';
import { LicensedTenantViewGetAllInput } from '@viasoft/licensing-management/app/tokens/inputs/licensedTenantView-get-all.input';

@Injectable()
export class LicensedTenantViewService {

  constructor(private licensedTenantViewServiceProxy: LicensedTenantViewServiceProxy) { }

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
