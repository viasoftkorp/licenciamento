import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {LicensingEnvironmentOrganizationalServiceProxy} from "../../../../../client/customer-licensing/api/licensingEnvironmentOrganizationalServiceProxy";
import {OrganizationUnit} from "../../../../../client/customer-licensing/model/organizationUnit";
import {CreateOrUpdateOrganizationUnitInput} from "../../../../../client/customer-licensing/model/createOrUpdateOrganizationUnitInput";
import {CreateOrganizationUnitOutput} from "../../../../../client/customer-licensing/model/createOrganizationUnitOutput";
import {UpdateOrganizationUnitOutput} from "../../../../../client/customer-licensing/model/updateOrganizationUnitOutput";
import {CreateOrUpdateEnvironmentInput} from "../../../../../client/customer-licensing/model/createOrUpdateEnvironmentInput";
import {CreateEnvironmentOutput} from "../../../../../client/customer-licensing/model/createEnvironmentOutput";
import {UpdateEnvironmentOutput} from "../../../../../client/customer-licensing/model/updateEnvironmentOutput";


@Injectable()

export class LicenseOrganizationService {

  constructor(
    private licensingEnvironmentOrganizationalService: LicensingEnvironmentOrganizationalServiceProxy
  ) { }

  public getUnit(licensedTenantIdentifier: string, id: string): Observable<OrganizationUnit> {
    return this.licensingEnvironmentOrganizationalService.getOrganizationUnit(licensedTenantIdentifier, id);
  }

  public createUnit(orgId: string, input: CreateOrUpdateOrganizationUnitInput): Observable<CreateOrganizationUnitOutput> {
    return this.licensingEnvironmentOrganizationalService.createOrganizationUnit(orgId, input);
  }

  public updateUnit( input: CreateOrUpdateOrganizationUnitInput): Observable<UpdateOrganizationUnitOutput> {
    return this.licensingEnvironmentOrganizationalService.updateOrganizationUnit(input.id, input);
  }

  public createEnvironment(licensedTenantIdentifier: string, input: CreateOrUpdateEnvironmentInput): Observable<CreateEnvironmentOutput> {
    return this.licensingEnvironmentOrganizationalService.createEnvironment(licensedTenantIdentifier, input);
  }

  public updateEnvironment(licensedTenantIdentifier: string, input: CreateOrUpdateEnvironmentInput): Observable<UpdateEnvironmentOutput>  {
    return this.licensingEnvironmentOrganizationalService.updateEnvironment(licensedTenantIdentifier, input);
  }
}
