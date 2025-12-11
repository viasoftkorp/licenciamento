import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { VsGridGetInput } from "@viasoft/components";
import { ensureTrailingSlash } from "@viasoft/http";
import { Observable } from "rxjs";
import { ProductType } from "src/app/common/enums/ProductType";
import { ProductOutput } from "../model/ProductOutput";
import { ProductOutputPagedResultDto } from "../model/ProductOutputPagedResultDto";
import { LicenseUsageOutput } from "../model/LicenseUsageOutput";
import { BASE_PATH } from "../variables";

@Injectable({
    providedIn: 'root'
})
export class ProductServiceProxy {
    constructor(private httpClient: HttpClient,
        @Inject(BASE_PATH) private basePath: string) {
    }

    public getAll(input: VsGridGetInput, licensedTenantId: string): Observable<ProductOutputPagedResultDto> {
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
        return this.httpClient.get<ProductOutputPagedResultDto>(`${ensureTrailingSlash(this.basePath)}licensing/customer-licensing/licenses/${licensedTenantId}/products`,
          {params});
    }

    public getById(licensedTenantId: string, identifier: string, productType: ProductType, licensingIdentifier: string): Observable<ProductOutput> {
      let params = new HttpParams();
      if(productType){
        params = params.set('ProductType', productType.toString());
      }
      if(licensingIdentifier){
        params = params.set('LicensingIdentifier', licensingIdentifier);
      }
      return this.httpClient.get<ProductOutput>(`${ensureTrailingSlash(this.basePath)}licensing/customer-licensing/licenses/${licensedTenantId}/products/${identifier}`,
        {params});
    }
    public getAllLicenseUsage(licensingIdentifier: string, bundleIdentifiers: string[], appIdentifiers: string[]): Observable<LicenseUsageOutput[]> {
      let params = new HttpParams();
      if (bundleIdentifiers) {
        bundleIdentifiers.forEach((bundleIdentifier) => {
          params = params.append('BundleIdentifiers', <string>bundleIdentifier);
        })
      }
      if (appIdentifiers) {
        appIdentifiers.forEach((appIdentifier) => {
          params = params.append('AppIdentifiers', <string>appIdentifier);
        })
      }
      return this.httpClient.get<LicenseUsageOutput[]>(`${ensureTrailingSlash(this.basePath)}licensing/customer-licensing/licenses/${licensingIdentifier}/products/license-usage`,
          {params});
    }
}