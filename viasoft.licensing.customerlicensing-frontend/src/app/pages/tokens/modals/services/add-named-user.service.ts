import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AddNamedUserServiceProxy } from 'src/client/customer-licensing/api/addNamedUserServiceProxy';
import { GetAllUsersInput } from 'src/client/customer-licensing/model/GetAllUsersInput';

@Injectable()
export class AddNamedUserService {

  constructor(
    private readonly serviceProxy: AddNamedUserServiceProxy
  ) { }

  public getAllUsers(input: GetAllUsersInput, licensingIdentifier: string): Observable<any> {
    return this.serviceProxy.getAllUsers(input, licensingIdentifier);
  }
}
