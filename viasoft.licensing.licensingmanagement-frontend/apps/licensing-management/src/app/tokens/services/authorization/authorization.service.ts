import { Injectable } from '@angular/core';
import { IAuthorizationProvider } from '@viasoft/authorization-management';
import { Observable } from 'rxjs';
import { AuthorizationServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService implements IAuthorizationProvider {

  private cache: Observable<string[]>;

  constructor(private authorizationService: AuthorizationServiceProxy) { }

  getPermissionsForCurrentApp() {
    if (!this.cache) {
      this.cache = this.authorizationService.getAuthorizations();
    }
    return this.cache;
  }

}
