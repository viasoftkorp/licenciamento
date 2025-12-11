import { Injectable } from '@angular/core';
import { AccountServiceProxy, AccountCreateInput, AccountUpdateInput, AccountCreateOutput, AccountUpdateOutput, AccountDeleteOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { AccountGetAllInput } from '@viasoft/licensing-management/app/tokens/inputs/account-get-all.input';
import { IVsBaseCrudService } from '@viasoft/common';

@Injectable({
  providedIn: 'root'
})
export class AccountsService implements IVsBaseCrudService<
  AccountCreateInput,
  AccountCreateOutput,
  AccountUpdateInput,
  AccountUpdateOutput,
  AccountDeleteOutput,
  AccountCreateOutput,
  AccountGetAllInput
> {

  onlyActiveAccounts: boolean;

  constructor(private accountServiceProxy: AccountServiceProxy) { }

  getAll(input: AccountGetAllInput) {
    return this.accountServiceProxy.getAll(
      this.onlyActiveAccounts,
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount
    );
  }

  getById(id: string) {
    return this.accountServiceProxy.getById(id);
  }

  create(account: AccountCreateInput) {
    return this.accountServiceProxy.create(account);
  }

  update(account: AccountUpdateInput) {
    return this.accountServiceProxy.update(account);
  }

  delete(id: string) {
    return this.accountServiceProxy._delete(id);
  }

  updateAccountsWithCrmService() {
    return this.accountServiceProxy.updateAccountsWithCrmService();
  }
}
