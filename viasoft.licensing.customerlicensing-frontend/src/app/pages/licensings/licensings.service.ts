import { Injectable } from '@angular/core';

import { Subject } from 'rxjs';
import { LicenseUsageInRealTimeServiceProxy } from '../../../client/customer-licensing';
import { GetAllLicenseUsageInRealTimeInput } from '../../common/inputs/get-all-licenseUsageInRealTime.input';
import { LicensingsGetAll } from '../../common/inputs/licensings-get-all.input';

@Injectable()
export class LicensingsService {

  advancedFilterSubject: Subject<string> = new Subject();
  private tenantId: string;
  private licensedTenantId: string;

  constructor(private licenseUsageInRealTime: LicenseUsageInRealTimeServiceProxy) { }

  getLicenseUsageInRealtime(input: GetAllLicenseUsageInRealTimeInput) {
    return this.licenseUsageInRealTime.getAllLicensesFromTenantId(
      input.licensingIdentifier,
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount
    );
  }

  setTenantId(tenantId: string): void {
    this.tenantId = tenantId;
  }

  getTenantId(): string {
    return this.tenantId;
  }

  setLicensedTenantId(licensedTenantId: string): void {
    this.licensedTenantId = licensedTenantId;
  }

  getLicensedTenantId(): string {
    return this.licensedTenantId;
  }

  getTenantInfoFromId(tenantId: string) {
    return this.licenseUsageInRealTime.getTenantInfoFromId(tenantId);
  }

  notifyAddSubject(input: LicensingsGetAll) {
    this.advancedFilterSubject.next(input.advancedFilter);
  }

  notifyClearSubject() {
    this.advancedFilterSubject.next(null);
  }
}
