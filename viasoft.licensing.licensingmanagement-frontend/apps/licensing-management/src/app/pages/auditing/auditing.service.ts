import { Injectable } from '@angular/core';
import { AuditingLogServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';
import { AuditingLogGetAllInput } from '@viasoft/licensing-management/app/tokens/inputs/auditing-get-all-input';

@Injectable({
  providedIn: 'root'
})
export class AuditingService {

  constructor(private auditingLog: AuditingLogServiceProxy) { }

  public getAll(input: AuditingLogGetAllInput) {
    return this.auditingLog.getAll(input.filter, input.advancedFilter, input.sorting, input.skipCount, input.maxResultCount);
  }

}
