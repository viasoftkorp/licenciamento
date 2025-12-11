import { Injectable } from '@angular/core';
import {
  CreateEnvironmentOutput,
  CreateOrganizationUnitOutput,
  CreateOrUpdateEnvironmentInput,
  CreateOrUpdateOrganizationUnitInput,
  LicensingOrganizationServiceProxy,
  OrganizationUnit,
  OrganizationUnitEnvironmentOutput,
  OrganizationUnitEnvironmentPagedResultDto,
  OrganizationUnitPagedResultDto,
  UpdateEnvironmentOutput,
  UpdateOrganizationUnitOutput,
} from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LicenseOrganizationService {
  constructor(private organizationServiceProxy: LicensingOrganizationServiceProxy) { }

  public getUnit(licensedTenantIdentifier: string, id: string): Observable<OrganizationUnit> {
    return this.organizationServiceProxy.getOrganizationUnit(licensedTenantIdentifier, id);
  }

  public getUnitsByOrganization(licensedTenantIdentifier: string, organizationId?: string, filter?: string, advancedFilter?: string, sorting?: string, skipCount?: number, maxResultCount?: number): Observable<OrganizationUnitPagedResultDto> {
    return this.organizationServiceProxy.getUnitsByOrganization(licensedTenantIdentifier, organizationId, filter, advancedFilter, sorting, skipCount, maxResultCount);
  }

  public createUnit(licensedTenantIdentifier: string, input: CreateOrUpdateOrganizationUnitInput): Observable<CreateOrganizationUnitOutput> {
    return this.organizationServiceProxy.createOrganizationUnit(licensedTenantIdentifier, input);
  }

  public updateUnit(licensedTenantIdentifier: string, input: CreateOrUpdateOrganizationUnitInput): Observable<UpdateOrganizationUnitOutput> {
    return this.organizationServiceProxy.updateOrganizationUnit(licensedTenantIdentifier, input.id, input);
  }

  public activateUnit(licensedTenantIdentifier: string, id: string): Observable<void> {
    return this.organizationServiceProxy.activateOrganizationUnit(licensedTenantIdentifier, id);
  }

  public deactivateUnit(licensedTenantIdentifier: string, id: string): Observable<void> {
    return this.organizationServiceProxy.deactivateOrganizationUnit(licensedTenantIdentifier, id);
  }

  public getEnvironment(licensedTenantIdentifier: string, id: string): Observable<OrganizationUnitEnvironmentOutput> {
    return this.organizationServiceProxy.getOrganizationEnvironment(licensedTenantIdentifier, id);
  }

  public getEnvironmentByUnit(licensedTenantIdentifier: string, unitId: string, activeOnly?: boolean, desktopOnly?: boolean, webOnly?: boolean, productionOnly?: boolean, filter?: string, advancedFilter?: string, sorting?: string, skipCount?: number, maxResultCount?: number): Observable<OrganizationUnitEnvironmentPagedResultDto> {
    return this.organizationServiceProxy.getEnvironmentByUnitId(licensedTenantIdentifier, unitId, unitId, activeOnly, desktopOnly, webOnly, productionOnly, filter, advancedFilter, sorting, skipCount, maxResultCount);
  }

  public updateEnvironment(licensedTenantIdentifier: string, input: CreateOrUpdateEnvironmentInput): Observable<UpdateEnvironmentOutput> {
    return this.organizationServiceProxy.updateOrganizationEnvironment(licensedTenantIdentifier, input.id, input);
  }

  public activateEnvironment(licensedTenantIdentifier: string, id: string): Observable<void> {
    return this.organizationServiceProxy.activateOrganizationEnvironment(licensedTenantIdentifier, id);
  }

  public deactivateEnvironment(licensedTenantIdentifier: string, id: string): Observable<void> {
    return this.organizationServiceProxy.deactivateOrganizationEnvironment(licensedTenantIdentifier, id);
  }

  public createEnvironment(licensedTenantIdentifier: string, input: CreateOrUpdateEnvironmentInput): Observable<CreateEnvironmentOutput> {
    return this.organizationServiceProxy.createOrganizationEnvironment(licensedTenantIdentifier, input);
  }
}
