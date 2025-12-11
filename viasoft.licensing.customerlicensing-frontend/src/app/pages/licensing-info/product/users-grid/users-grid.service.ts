import { Injectable } from '@angular/core';
import { VsGridGetInput } from '@viasoft/components';
import { Observable, Subject } from 'rxjs';
import { ProductType } from 'src/app/common/enums/ProductType';
import { LicenseUserBehaviourOutputPagedResultDto, LicenseUserBehaviourServiceProxy } from 'src/client/customer-licensing';
import { licenseUserBehaviourNamedOfflineOutputPagedResultDto } from 'src/client/customer-licensing/model/licenseUserBehaviourNamedOfflineOutputPagedResultDto';
import { licenseUserBehaviourNamedOnlineOutputPagedResultDto } from 'src/client/customer-licensing/model/licenseUserBehaviourNamedOnlineOutputPagedResultDto';

@Injectable()
export class UsersGridService {

  public refreshProductInfo: Subject<any> = new Subject<any>();

  constructor(private licenseUserBehaviourService: LicenseUserBehaviourServiceProxy) { }

  public getUserBehaviourFloating(input: VsGridGetInput, licensingIdentifier: string, bundleIdentifier: string, productType: ProductType): Observable<LicenseUserBehaviourOutputPagedResultDto> {
    return this.licenseUserBehaviourService.getUserBehaviourFloating(input, licensingIdentifier, bundleIdentifier, productType);
  }

  public getUserBehaviourNamedOnline(input: VsGridGetInput, licensedTenant: string, licensingIdentifier: string, bundleIdentifier: string, productId: string, productType: ProductType): Observable<licenseUserBehaviourNamedOnlineOutputPagedResultDto> {
    return this.licenseUserBehaviourService.getUserBehaviourFromBundleNamedOnline(input, licensedTenant, licensingIdentifier, bundleIdentifier, productId, productType);
  }

  public getUserBehaviourNamedOffline(input: VsGridGetInput, licensingTenantId: string, licensingIdentifier: string, bundleIdentifier: string,  productId: string, productType: ProductType): Observable<licenseUserBehaviourNamedOfflineOutputPagedResultDto> {
    return this.licenseUserBehaviourService.getUserBehaviourFromBundleNamedOffline(input, licensingTenantId, licensingIdentifier, bundleIdentifier, productId, productType);
  }
}
