import { Injectable } from '@angular/core';
import { VsReadOnlyBaseService } from '@viasoft/common';
import { LicensedTenantGetAllClass } from '../../tokens/classes/licensedtenant-get-all.class';
import { Observable } from 'rxjs';
import { LicensedTenantViewOutput } from '../../clients/license-management/model/licensedTenantViewOutput';
import { LicensedTenantViewServiceProxy } from '../../clients/license-management/api/licensedTenantViewServiceProxy';
import { LicensedTenantViewOutputPagedResultDto } from '../../clients/license-management/model/licensedTenantViewOutputPagedResultDto';

@Injectable()
export class LicensedTenantViewService extends VsReadOnlyBaseService<
LicensedTenantViewOutput,
LicensedTenantGetAllClass
> {

  constructor(private readonly licensedTenantViewProxyService: LicensedTenantViewServiceProxy) {
    super();
  }

  proxyService = this.licensedTenantViewProxyService;

  getById() {
    return null;
  }

  getAll(input: LicensedTenantGetAllClass): Observable<LicensedTenantViewOutputPagedResultDto> {
    return this.licensedTenantViewProxyService.getAll(
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount
    );
  }
}
