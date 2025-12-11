import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductType } from 'src/app/common/enums/ProductType';
import { BASE_PATH } from 'src/client/customer-licensing';
import { CreateNamedUserInput } from '../model/CreateNamedUserInput';
import { NamedUserProductLicenseOutput } from '../model/NamedUserProductLicenseOutput';
import { RemoveNamedUserFromProductOutput } from '../model/RemoveNamedUserFromProductOutput';
import { UpdateNamedUsersFromProductOutput } from '../model/UpdateNamedUsersFromProductOutput';

@Injectable({
  providedIn: 'root'
})
export class NamedUserServiceProxy {

  constructor(
    private readonly httpClient: HttpClient,
    @Inject(BASE_PATH) private basePath: string
  ) { }

  create(licensedTenantId: string, licensedBundleId: string, input: CreateNamedUserInput): Observable<NamedUserProductLicenseOutput> {
    const route = `${this.basePath}/licensing/customer-licensing/licenses/${licensedTenantId}/products/${licensedBundleId}/named-user`;
    return this.httpClient.post<NamedUserProductLicenseOutput>(route, input);
  }

  update(licensedTenantId: string, licensedBundleId: string, namedUserEmail: string,input: CreateNamedUserInput): Observable<UpdateNamedUsersFromProductOutput> {
    const route = `${this.basePath}/licensing/customer-licensing/licenses/${licensedTenantId}/products/${licensedBundleId}/named-user/${namedUserEmail}`;
    return this.httpClient.put<UpdateNamedUsersFromProductOutput>(route, input);
  }

  delete(licensedTenantId: string, licensedBundle: string, namedUserEmail: string, productType: ProductType): Observable<RemoveNamedUserFromProductOutput> {
    let params = new HttpParams();
    if(productType){
      params = params.set('ProductType', productType);
    }
    const route = `${this.basePath}/licensing/customer-licensing/licenses/${licensedTenantId}/products/${licensedBundle}/named-user/${namedUserEmail}`;
    return this.httpClient.delete<RemoveNamedUserFromProductOutput>(route, {params});
  }

}
