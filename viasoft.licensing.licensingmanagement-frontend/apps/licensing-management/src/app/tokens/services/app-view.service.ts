import { Injectable } from '@angular/core';
import { AppServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';
import { AppGetAllInput } from '../inputs/app-get-all.input';
import { GridCheckBoxValidation } from '../utils/grid-check-box-validation';

@Injectable({
  providedIn: 'root'
})
export class AppViewService extends GridCheckBoxValidation {

  constructor(private apps: AppServiceProxy) {
    super();
  }

  getAll(
    input: AppGetAllInput
  ) {
    return this.apps.getAll(
      undefined,
      undefined,
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount);
  }
}
