import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductType } from 'src/app/common/enums/ProductType';
import { NamedUserServiceProxy } from 'src/client/customer-licensing/api/NamedUserServiceProxy';
import { CreateNamedUserInput } from 'src/client/customer-licensing/model/CreateNamedUserInput';
import { NamedUserProductLicenseOutput } from 'src/client/customer-licensing/model/NamedUserProductLicenseOutput';
import { RemoveNamedUserFromProductOutput } from 'src/client/customer-licensing/model/RemoveNamedUserFromProductOutput';
import { UpdateNamedUsersFromProductOutput } from 'src/client/customer-licensing/model/UpdateNamedUsersFromProductOutput';

@Injectable()
export class NamedUserService {

  constructor(private serviceProxy: NamedUserServiceProxy) { }

  public create(licensedTenantId: string, licensedBundle: string, input: CreateNamedUserInput): Observable<NamedUserProductLicenseOutput> {
    return this.serviceProxy.create(licensedTenantId, licensedBundle, input);
  }

  public update(licensedTenantId: string, licensedBundle: string, namedUserEmail: string, input: CreateNamedUserInput): Observable<UpdateNamedUsersFromProductOutput> {
    return this.serviceProxy.update(licensedTenantId, licensedBundle, namedUserEmail, input);
  }

  public delete(licensedTenantId: string, licensedBundle: string, namedUserEmail: string, productType: ProductType): Observable<RemoveNamedUserFromProductOutput> {
    return this.serviceProxy.delete(licensedTenantId, licensedBundle, namedUserEmail, productType);
  }

}
