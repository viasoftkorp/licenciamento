import { Injectable } from '@angular/core';
import { CnpjServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';

@Injectable()
export class AccountsCnpjValidationService {

  constructor(private cnpjServiceProxy: CnpjServiceProxy) { }

  GetCompanyByCnpj(cnpj: string, id: string) {
    return this.cnpjServiceProxy.getCompanyByCnpj(cnpj, id);
  }
}
