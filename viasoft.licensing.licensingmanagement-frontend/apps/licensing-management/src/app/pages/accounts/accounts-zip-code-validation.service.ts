import { Injectable } from '@angular/core';
import { ZipCodeServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';

@Injectable()
export class AccountsZipCodeValidationService {

  constructor(private zipCodeServiceProxy: ZipCodeServiceProxy) { }

  GetAddressByZipcode(zipCode: string) {
    return this.zipCodeServiceProxy.getAddressByZipcode(zipCode);
  }
}
