import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  AppQuotaServiceProxy,
  FileAppQuotaInput,
  FileAppQuotaView,
  FileAppQuotaViewPagedResultDto,
  FileTenantQuota,
  FileTenantQuotaInput,
  TenantQuotaServiceProxy,
} from '@viasoft/licensing-management/clients/licensing-management';

@Injectable()
export class FileProviderConfigurationService {
  constructor(
    private tenantQuotaServiceProxy: TenantQuotaServiceProxy,
    private appQuotaServiceProxy: AppQuotaServiceProxy
  ) { }

  public insertOrUpdateTenantConfiguration(id: string, licenseTenantId: string, quotaLimit: number)
    : Observable<any> {
    const input: FileTenantQuotaInput = {
      id,
      licenseTenantId,
      quotaLimit
    };
    return this.tenantQuotaServiceProxy.addOrUpdateTenantQuota(input);
  }

  public insertOrUpdateAppConfiguration(input: FileAppQuotaInput): Observable<any> {
    return this.appQuotaServiceProxy.addOrUpdateAppQuota(input);
  }

  public insertAppQuota(input: FileAppQuotaInput): Observable<any> {
    return this.appQuotaServiceProxy.insert(input);
  }

  public updateAppQuota(input: FileAppQuotaInput): Observable<any> {
    return this.appQuotaServiceProxy.update(input);
  }

  public getAll(licensedTenantId?: string, filter?: string, advancedFilter?: string, sorting?: string, skipCount?: number, maxResultCount?: number)
    : Observable<FileAppQuotaViewPagedResultDto> {
    return this.appQuotaServiceProxy.getAll(licensedTenantId, filter, advancedFilter, sorting, skipCount, maxResultCount);
  }

  public getLicensedAppsForQuotaConfiguration(licensedTenantId?: string, filter?: string, advancedFilter?: string, sorting?: string, skipCount?: number, maxResultCount?: number)
    : Observable<FileAppQuotaViewPagedResultDto> {
    return this.appQuotaServiceProxy.getLicensedAppsForQuotaConfiguration(licensedTenantId, filter, advancedFilter, sorting, skipCount, maxResultCount);
  }

  public deleteAppQuotaConfiguration(licensedTenantId: string, appId: string): Observable<FileAppQuotaViewPagedResultDto> {
    return this.appQuotaServiceProxy.deleteAppQuota(licensedTenantId, appId);
  }

  public getTenantConfiguration(licenseTenantIdentifier?: string): Observable<FileTenantQuota> {
    return this.tenantQuotaServiceProxy.getTenantQuota(licenseTenantIdentifier);
  }

  public getAppConfiguration(licenseTenantId?: string, appId?: string): Observable<FileAppQuotaView> {
    return this.appQuotaServiceProxy.getAppQuota(licenseTenantId, appId);
  }
}
