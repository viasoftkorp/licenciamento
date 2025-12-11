import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BASE_PATH } from 'src/client/customer-licensing';
import { GetAllUsersInput } from 'src/client/customer-licensing/model/GetAllUsersInput';

@Injectable({
  providedIn: 'root'
})
export class AddNamedUserServiceProxy {

  constructor(
    private readonly httpClient: HttpClient,
    @Inject(BASE_PATH) private basePath: string
  ) { }

  public getAllUsers(input: GetAllUsersInput, licensingIdentifier: string): Observable<any> {
    const route = `${this.basePath}/licensing/customer-licensing/licenses/${licensingIdentifier}/users`;

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
