import { Injectable} from '@angular/core';

import {VsJwtProviderService} from "@viasoft/http";
import {LicenseUsageInRealTimeServiceProxy} from "../../../../client/customer-licensing";
import { DeployVersions} from "./infrastructure-settings.interface";
import {
  SettingsInfrastructureServiceProxy
} from "../../../../client/API-client/model/settingsInfrastructureServiceProxy";


@Injectable({
  providedIn: 'root'
})
export class InfrastructureSettingsService {

  constructor(
    private jwt: VsJwtProviderService,
    private licenseUsageInRealTime: LicenseUsageInRealTimeServiceProxy,
    private infrastructureSettingsProxy: SettingsInfrastructureServiceProxy
  ) {}

  private getTenantId() {
    return this.jwt.getTenantIdFromJwt();
  }

  private getTenantInfoFromId(tenantId: string) {
    return this.licenseUsageInRealTime.getTenantInfoFromId(tenantId);
  }

  public getLicensedTenantId() {
    return this.getTenantInfoFromId(this.getTenantId())
  }

  public getGatewayAndVersions() {
    return this.infrastructureSettingsProxy.getGatewayAndVersions(this.getTenantId())
  }

  public getVersionsOfDropdown (deployVersions: DeployVersions[]) {
    let versions =[];
    for (let i=0; i<deployVersions.length; i++) {
      versions.push({
        value: deployVersions[i].version,
        name: `${deployVersions[i].version}`})
    }
    return versions;
  }

  public updateGateway(licensedTenant, gateway) {
    let body = {
      LicensedTenantId: licensedTenant,
      gatewayAddress: gateway
    }
    return this.infrastructureSettingsProxy.updateGateway(body)

  }

  public copyToClipboardImplementation(text: any): void {
   this.infrastructureSettingsProxy.copyToClipboardImplementation(text);
  }

  public copyToClipboardUpdate(text: any): void {
    this.infrastructureSettingsProxy.copyToClipboardUpdate(text);
  }

  public copyToClipboardUninstall(text: any): void {
    this.infrastructureSettingsProxy.copyToClipboardUninstall(text);
  }
}
