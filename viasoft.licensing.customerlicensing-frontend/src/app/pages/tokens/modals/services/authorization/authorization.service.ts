import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthorizationServiceProxy } from 'src/client/authorization/authorizationServiceProxy';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {

  constructor(private authorizationService: AuthorizationServiceProxy) { }

  private cache: Observable<string[]>;

  getPermissionsForCurrentApp() {
    if (!this.cache) {
      this.cache = this.authorizationService.GetPolicies();
    }
    return this.cache;
  }
}
