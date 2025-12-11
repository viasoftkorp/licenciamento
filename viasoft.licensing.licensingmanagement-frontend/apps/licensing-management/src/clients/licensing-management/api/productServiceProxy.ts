import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { VsGridGetInput } from "@viasoft/components";
import { ensureTrailingSlash } from "@viasoft/http";
import { Observable } from "rxjs";
import { LicensedProductOutputPagedResult } from "../model/LicensedProductOutputPagedResult";
import { BASE_PATH } from "../variables";

@Injectable()
export class ProductServiceProxy {
    constructor(private httpClient: HttpClient,
        @Inject(BASE_PATH) private basePath: string) {
    }

    public getAll(input: VsGridGetInput, licensedTenantId: string): Observable<LicensedProductOutputPagedResult> {
        let params = new HttpParams();

        if (input.filter) {
          params = params.set('Filter', input.filter);
        }
        if (input.advancedFilter) {
          params = params.set('AdvancedFilter', input.advancedFilter);
        }
        if (input.sorting) {
          params = params.set('Sorting', input.sorting);
        }
        if (input.skipCount) {
          params = params.set('SkipCount', input.skipCount.toString());
        }
        if (input.maxResultCount) {
          params = params.set('MaxResultCount', input.maxResultCount.toString());
        }
        return this.httpClient.get<LicensedProductOutputPagedResult>(`${ensureTrailingSlash(this.basePath)}licensing/licensing-management/licenses/${licensedTenantId}/products`,
          {params});
    }
}