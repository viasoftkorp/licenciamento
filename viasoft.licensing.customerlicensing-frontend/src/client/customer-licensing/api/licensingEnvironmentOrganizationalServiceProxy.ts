import {Inject, Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {API_GATEWAY, ensureTrailingSlash} from "@viasoft/http";
import {Observable} from "rxjs";
import {CreateOrUpdateEnvironmentInput} from "../model/createOrUpdateEnvironmentInput";
import {CreateEnvironmentOutput} from "../model/createEnvironmentOutput";
import {UpdateEnvironmentOutput} from "../model/updateEnvironmentOutput";
import {CreateOrUpdateOrganizationUnitInput} from "../model/createOrUpdateOrganizationUnitInput";
import {CreateOrganizationUnitOutput} from "../model/createOrganizationUnitOutput";
import {OrganizationUnit} from "../model/organizationUnit";
import {UpdateOrganizationUnitOutput} from "../model/updateOrganizationUnitOutput";


@Injectable(
  {providedIn: 'root'}
)

export class LicensingEnvironmentOrganizationalServiceProxy {

  private BASE_PATH: string = ensureTrailingSlash(this.apiGateway) + "licensing/customer-licensing/organizations/";

  constructor(
    @Inject(API_GATEWAY) private apiGateway: string,
    private httpClient: HttpClient
  ) {
  }

  public createOrganizationUnit(identifier: string, createOrUpdateOrganizationUnitInput?: CreateOrUpdateOrganizationUnitInput): Observable<CreateOrganizationUnitOutput> {
    return this.httpClient.post<CreateOrganizationUnitOutput>( this.BASE_PATH + identifier + '/units',
      createOrUpdateOrganizationUnitInput)
  }

  public getOrganizationUnit(identifier: string, id: string): Observable<OrganizationUnit> {
    return this.httpClient.get<OrganizationUnit>(this.BASE_PATH + 'units/' + id)
  }

  public updateOrganizationUnit(id: string, createOrUpdateOrganizationUnitInput?: CreateOrUpdateOrganizationUnitInput): Observable<UpdateOrganizationUnitOutput> {
    return this.httpClient.put<UpdateOrganizationUnitOutput>(this.BASE_PATH + 'units/' + id,
      createOrUpdateOrganizationUnitInput)
  }

  public createEnvironment(licenseIdentifier: string, createOrUpdateEnvironmentInput: CreateOrUpdateEnvironmentInput): Observable<CreateEnvironmentOutput> {
    return this.httpClient.post<CreateEnvironmentOutput>(this.BASE_PATH + 'units/' + createOrUpdateEnvironmentInput.organizationUnitId + '/environments',
      createOrUpdateEnvironmentInput)
  }

  public updateEnvironment(licenseIdentifier: string, createOrUpdateEnvironmentInput: CreateOrUpdateEnvironmentInput): Observable<UpdateEnvironmentOutput> {
    return this.httpClient.put<UpdateEnvironmentOutput>(this.BASE_PATH + 'units/' + createOrUpdateEnvironmentInput.organizationUnitId + '/environments/' + createOrUpdateEnvironmentInput.id,
      createOrUpdateEnvironmentInput)
  }

}
