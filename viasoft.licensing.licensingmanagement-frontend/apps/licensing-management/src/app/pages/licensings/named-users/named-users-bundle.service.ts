import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable, Optional } from '@angular/core';
import { API_GATEWAY } from '@viasoft/http';
import { CreateNamedUserInput } from '@viasoft/licensing-management/app/tokens/interfaces/create-named-user-input.interface';
import { GetNamedUserBundleOutput } from '@viasoft/licensing-management/app/tokens/interfaces/get-named-user-bundle-output.interface';
import { NamedUserBundleOutput } from '@viasoft/licensing-management/app/tokens/interfaces/named-user-bundle-output.interface';
import { RemoveNamedUserAppOutput } from '@viasoft/licensing-management/app/tokens/interfaces/remove-named-user-app-output.interface';
import { UpdateNamedUserInput } from '@viasoft/licensing-management/app/tokens/interfaces/update-named-user-input.interface';
import { UpdateNamedUserAppOutput } from '@viasoft/licensing-management/app/tokens/interfaces/update-named-user-app-output.interface';
import { Observable } from 'rxjs';
import { NamedUsersService } from './named-users.service';
import { VsGridGetInput } from '@viasoft/components/grid';

@Injectable()
export class NamedUsersBundleService implements NamedUsersService {

  constructor(
    private readonly httpClient: HttpClient,
    @Optional() @Inject(API_GATEWAY) protected basePath: string
  ) { }

  get(licensedTenantId: string, licensedEntityId: string, input: VsGridGetInput): Observable<any> {
    const route = `${this.basePath}licensing/licensing-management/licenses/${licensedTenantId}/licensed-bundles/${licensedEntityId}/named-user`;

    let params = new HttpParams();
    if (input.filter !== undefined && input.filter !== null && input.filter !== '') {
      params = params.set('Filter', input.filter);
    }
    if (input.advancedFilter !== undefined && input.advancedFilter !== null && input.advancedFilter !== '') {
      params = params.set('AdvancedFilter', input.advancedFilter);
    }
    if (input.sorting !== undefined && input.sorting !== null && input.sorting !== '') {
      params = params.set('Sorting', input.sorting);
    }
    if (input.skipCount !== undefined && input.skipCount !== null) {
      params = params.set('SkipCount', input.skipCount.toString());
    }
    if (input.maxResultCount !== undefined && input.maxResultCount !== null) {
      params = params.set('MaxResultCount', input.maxResultCount.toString());
    }

    return this.httpClient.get<GetNamedUserBundleOutput>(route, {params});
  }

  create(licensedTenantId: string, licensedEntityId: string, input: CreateNamedUserInput): Observable<any> {
    const route = `${this.basePath}licensing/licensing-management/licenses/${licensedTenantId}/licensed-bundles/${licensedEntityId}/named-user`;
    return this.httpClient.post<NamedUserBundleOutput>(route, input);
  }

  update(licensedTenantId: string, licensedEntityId: string, namedUserId: string, input: UpdateNamedUserInput): Observable<any> {
    const route = `${this.basePath}licensing/licensing-management/licenses/${licensedTenantId}/licensed-bundles/${licensedEntityId}/named-user/${namedUserId}`;
    return this.httpClient.put<UpdateNamedUserAppOutput>(route, input);
  }

  delete(licensedTenantId: string, licensedEntityId: string, namedUserId: string): Observable<any> {
    const route = `${this.basePath}licensing/licensing-management/licenses/${licensedTenantId}/licensed-bundles/${licensedEntityId}/named-user/${namedUserId}`;
    return this.httpClient.delete<RemoveNamedUserAppOutput>(route);
  }

}
