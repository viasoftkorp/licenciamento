import {Inject, Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {BASE_PATH} from "@viasoft/licensing-management/clients/licensing-management";
import {Observable} from "rxjs";
import {ensureTrailingSlash} from "@viasoft/http";
import {
  LicensedTenantSettingsOutput
} from "@viasoft/licensing-management/clients/licensing-management/model/licensedTenantSettingsOutput";

@Injectable({
  providedIn: 'root'
})
export class LicensedTenantSettingsServiceProxy {
  constructor(
    private httpClient: HttpClient,
    @Inject(BASE_PATH) private basePath: string
  ) {}

  public get(identifier: string): Observable<LicensedTenantSettingsOutput> {
    return this.httpClient.get<LicensedTenantSettingsOutput>(
      `${ensureTrailingSlash(this.basePath)}licensing/licensing-management/licenses/${encodeURIComponent(String(identifier))}/license-server-settings`,
    );
  }

  public update(identifier: string, useSimpleHardwareId: boolean): Observable<LicensedTenantSettingsOutput> {
    return this.httpClient.post<LicensedTenantSettingsOutput>(
      `${ensureTrailingSlash(this.basePath)}licensing/licensing-management/licenses/${encodeURIComponent(String(identifier))}/license-server-settings`,
      {useSimpleHardwareId}
    );
  }
}
