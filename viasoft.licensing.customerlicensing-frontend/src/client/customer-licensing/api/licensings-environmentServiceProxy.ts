import {Inject, Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient, HttpParams} from "@angular/common/http";

import {API_GATEWAY, ensureTrailingSlash} from "@viasoft/http"
import {OrganizationUnitPagedResultDto} from "../model/organizationUnitPagedResultDto";
import {OrganizationUnitEnvironmentPagedResultDto} from "../model/organizationUnitEnvironmentPagedResultDto";


@Injectable({
  providedIn: 'root'
})

export class LicensingsEnvironmentServiceProxy {

  private BASE_URL: string = ensureTrailingSlash(this.apiGateway) + 'licensing/customer-licensing/organizations/units/';

  constructor(
    private httpClient: HttpClient,
    @Inject(API_GATEWAY) private apiGateway: string
  ) {
  }

  public activateUnit(unitId): Observable<any> {
    return this.httpClient.post<any>(this.BASE_URL + unitId + '/activate', null);
  }

  public deactivateUnit(unitId): Observable<any> {
    return this.httpClient.post<any>(this.BASE_URL + unitId + '/deactivate', null);
  }

  public activateEnvironment(licensedTenantId, id): Observable<any> {
    return this.httpClient.post<any>(this.BASE_URL + licensedTenantId + '/environments/' + id + '/activate', null);
  }

  public deactivateEnvironment(licensedTenantId, id): Observable<any> {
    return this.httpClient.post<any>(this.BASE_URL + licensedTenantId + '/environments/' + id + '/deactivate', null);
  }


  private addToHttpParams(httpParams: HttpParams, value: any, key?: string): HttpParams {
    if (typeof value === "object" && value instanceof Date === false) {
      httpParams = this.addToHttpParamsRecursive(httpParams, value);
    } else {
      httpParams = this.addToHttpParamsRecursive(httpParams, value, key);
    }
    return httpParams;
  }

  private addToHttpParamsRecursive(httpParams: HttpParams, value?: any, key?: string): HttpParams {
    if (value == null) {
      return httpParams;
    }

    if (typeof value === "object") {
      if (Array.isArray(value)) {
        (value as any[]).forEach(elem => httpParams = this.addToHttpParamsRecursive(httpParams, elem, key));
      } else if (value instanceof Date) {
        if (key != null) {
          httpParams = httpParams.append(key,
            (value as Date).toISOString().substr(0, 10));
        } else {
          throw Error("key may not be null if value is Date");
        }
      } else {
        Object.keys(value).forEach(k => httpParams = this.addToHttpParamsRecursive(
          httpParams, value[k], key != null ? `${key}.${k}` : k));
      }
    } else if (key != null) {
      httpParams = httpParams.append(key, value);
    } else {
      throw Error("key may not be null if value is not object or array");
    }
    return httpParams;
  }

  public getUnitForGrid<GetUnitForGridInput>(organizationId: string, sorting?: boolean, filter?: boolean, advancedFilter?: string, skipCount?: boolean, maxResultCount?: any): Observable<OrganizationUnitPagedResultDto> {
    let queryParameters = new HttpParams();

    if (sorting !== undefined && sorting !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>sorting, 'Sorting');
    }

    if (filter !== undefined && filter !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>filter, 'Filter');
    }

    if (advancedFilter !== undefined && advancedFilter !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>advancedFilter, 'AdvancedFilter');
    }

    if (skipCount !== undefined && skipCount !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>skipCount, 'SkipCount');
    }
    if (maxResultCount !== undefined && maxResultCount !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>maxResultCount, 'MaxResultCount');
    }

    return this.httpClient.get<OrganizationUnitPagedResultDto>(ensureTrailingSlash(this.apiGateway) + 'licensing/customer-licensing/organizations/' + organizationId + '/units',
      {params: queryParameters}
    )
  }


  public getEnvironmentForGrid<getEnvironmentForGridInput>(identifier: string, unitId: string, activeOnly?: boolean, desktopOnly?: boolean, webOnly?: boolean, productionOnly?: boolean, filter?: string, advancedFilter?: string, sorting?: string, skipCount?: boolean, maxResultCount?: any): Observable<OrganizationUnitEnvironmentPagedResultDto> {
    if (identifier === null || identifier === undefined) {
      throw new Error('Required parameter identifier was null or undefined when calling getEnvironmentByUnitId.');
    }
    if (unitId === null || unitId === undefined) {
      throw new Error('Required parameter unitId was null or undefined when calling getEnvironmentByUnitId.');
    }

    let queryParameters = new HttpParams();
    queryParameters = this.addToHttpParams(queryParameters,
      <any>unitId, 'UnitId');

    if (activeOnly !== undefined && activeOnly !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>activeOnly, 'ActiveOnly');
    }
    if (desktopOnly !== undefined && desktopOnly !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>desktopOnly, 'DesktopOnly');
    }
    if (webOnly !== undefined && webOnly !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>webOnly, 'WebOnly');
    }
    if (productionOnly !== undefined && productionOnly !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>productionOnly, 'ProductionOnly');
    }

    if (filter !== undefined && filter !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>filter, 'Filter');
    }
    if (advancedFilter !== undefined && advancedFilter !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>advancedFilter, 'AdvancedFilter');
    }

    if (sorting !== undefined && sorting !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>sorting, 'Sorting');
    }

    if (skipCount !== undefined && skipCount !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>skipCount, 'SkipCount');
    }
    if (maxResultCount !== undefined && maxResultCount !== null) {
      queryParameters = this.addToHttpParams(queryParameters,
        <any>maxResultCount, 'MaxResultCount');
    }

    return this.httpClient.get<OrganizationUnitEnvironmentPagedResultDto>(this.BASE_URL + unitId + '/environments',
      {
        params: queryParameters
      }
    )

  }

}
