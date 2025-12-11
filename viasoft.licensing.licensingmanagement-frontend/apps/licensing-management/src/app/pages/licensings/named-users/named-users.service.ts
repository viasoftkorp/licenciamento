import { Injectable } from '@angular/core';
import { VsGridGetInput } from '@viasoft/components/grid';
import { CreateNamedUserInput } from '@viasoft/licensing-management/app/tokens/interfaces/create-named-user-input.interface';
import { UpdateNamedUserInput } from '@viasoft/licensing-management/app/tokens/interfaces/update-named-user-input.interface';
import { Observable } from 'rxjs';

@Injectable()
export abstract class NamedUsersService {

  abstract get(licensedTenantId: string, licensedEntityId: string, input: VsGridGetInput): Observable<any>;
  abstract create(licensedTenantId: string, licensedEntityId: string, input: CreateNamedUserInput): Observable<any>;
  abstract update(licensedTenantId: string, licensedEntityId: string, namedUserId: string, input: UpdateNamedUserInput): Observable<any>;
  abstract delete(licensedTenantId: string, licensedEntityId: string, namedUserId: string): Observable<any>;
}
