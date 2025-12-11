import { Injectable } from '@angular/core';
import {Observable, Subject} from "rxjs";
import {
  LicensedTenantSettingsOutput
} from "@viasoft/licensing-management/clients/licensing-management/model/licensedTenantSettingsOutput";
import {
  LicensedTenantSettingsServiceProxy
} from "@viasoft/licensing-management/clients/licensing-management/api/licensedTenantSettingsServiceProxy";

@Injectable()
export class LicenseDetailLicensesServerService {
  public licensingIdentifierReadySubject: Subject<void> = new Subject<void>();
  public licensedTenantSettingsValueChanged: Subject<any> = new Subject<any>();

  constructor(
    private readonly licensedTenantSettingsServiceProxy: LicensedTenantSettingsServiceProxy
  ) {}

  public licensingIdentifierReady(): void {
    this.licensingIdentifierReadySubject.next();
  }

  public getLicensedTenantSettings(identifier: string): Observable<LicensedTenantSettingsOutput> {
    return this.licensedTenantSettingsServiceProxy.get(identifier);
  }

  public updateLicensedTenantSettings(identifier: string, useSimpleHardwareId: boolean): Observable<LicensedTenantSettingsOutput> {
    return this.licensedTenantSettingsServiceProxy.update(identifier, useSimpleHardwareId);
  }
}
