import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable, Optional } from '@angular/core';
import { API_GATEWAY } from '@viasoft/http';
import { Observable } from 'rxjs';
import { GetAllUsersInput } from '../../interfaces/get-all-users-input.interface';

@Injectable()
export class AddNamedUserService {

  constructor(
    private readonly httpClient: HttpClient,
    @Optional() @Inject(API_GATEWAY) protected basePath: string
  ) { }

  public getAllUsers(input: GetAllUsersInput, identifier: string): Observable<any> {
    const route = `${this.basePath}licensing/licensing-management/licenses/${identifier}/users`;

    let params = new HttpParams();
    if (input.filter) {
      const advancedFilter = `{"condition":"or","rules":[{"field":"email","operator":"contains","type":"string","value":"${input.filter}"}]}`;
      params = params.set('AdvancedFilter', advancedFilter as any) ?? params;
    }
    if (input.skipCount) {
      params = params.set('SkipCount', input.skipCount as any) ?? params;
    }
    if (input.maxResultCount) {
      params = params.set('MaxResultCount', input.maxResultCount as any) ?? params;
    }

    return this.httpClient.get<any>(route, {params});
  }
}
