import {Injectable} from '@angular/core';
import {Observable, Subject} from "rxjs";

import {VsGridOptions} from "@viasoft/components";
import {VsJwtProviderService} from "@viasoft/http";
import {LicenseUsageInRealTimeServiceProxy, TenantInfoOutput} from "../../../../client/customer-licensing";
import {OrganizationUnitPagedResultDto} from "../../../../client/customer-licensing/model/organizationUnitPagedResultDto";
import {LicensingsEnvironmentServiceProxy } from "../../../../client/customer-licensing/api/licensings-environmentServiceProxy";


@Injectable()

export class LicensingsEnvironmentService {

  public updatingUnitGrid = new Subject<void>();

  constructor(
    private jwt: VsJwtProviderService,
    private licenseUsageInRealTime: LicenseUsageInRealTimeServiceProxy,
    private licensingsEnvironmentServiceProxy: LicensingsEnvironmentServiceProxy
  ) {}

  public updateUnitGridAfterAdd (): void {
    this.updatingUnitGrid.next();
  }

  public getTenantId(): string {
    return this.jwt.getTenantIdFromJwt();
  }

  private getTenantInfoFromId(tenantId: string): Observable<TenantInfoOutput> {
    return this.licenseUsageInRealTime.getTenantInfoFromId(tenantId);
  }

  public getLicensedTenantId(): Observable<TenantInfoOutput> {
    return this.getTenantInfoFromId(this.getTenantId());
  }

  public getUnitForGrid<GetUnitForGridInput>(organizationId, sorting?, filter?,advancedFilter?, skipCount?, maxResultCount?): Observable<OrganizationUnitPagedResultDto> {
    return this.licensingsEnvironmentServiceProxy.getUnitForGrid<GetUnitForGridInput>(organizationId, sorting, filter, advancedFilter, skipCount, maxResultCount);
  }

  public activateUnit(unitId): Observable<any> {
    return this.licensingsEnvironmentServiceProxy.activateUnit(unitId);
  }
  public deactivateUnit(unitId): Observable<any> {
    return this.licensingsEnvironmentServiceProxy.deactivateUnit(unitId);
  }

  public activateEnvironment(licensedTenantId, id): Observable<any> {
   return this.licensingsEnvironmentServiceProxy.activateEnvironment(licensedTenantId, id);
  }

  public deactivateEnvironment(licensedTenantId, id): Observable<any> {
   return this.licensingsEnvironmentServiceProxy.deactivateEnvironment(licensedTenantId, id);
  }

  public getEnvironmentForGrid<getEnvironmentForGridInput>(identifier, unitId,  activeOnly?, desktopOnly?, webOnly?, productionOnly?, filter?, advancedFilter?, sorting?, skipCount?, maxResultCount?) {
    return this.licensingsEnvironmentServiceProxy.getEnvironmentForGrid<getEnvironmentForGridInput>(identifier, unitId, activeOnly, desktopOnly, webOnly, productionOnly, filter, advancedFilter, sorting, skipCount, maxResultCount);
  }

}
