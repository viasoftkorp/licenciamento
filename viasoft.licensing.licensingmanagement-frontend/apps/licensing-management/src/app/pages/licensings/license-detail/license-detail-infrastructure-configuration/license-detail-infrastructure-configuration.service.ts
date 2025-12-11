import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { InfrastructureConfigurationUpdateInput } from '@viasoft/licensing-management/clients/licensing-management/model/infrastructureConfigurationUpdateInput';
import { InfrastructureConfigurationServiceProxy } from '@viasoft/licensing-management/clients/licensing-management/api/infrastructureConfigurationServiceProxy';
import { InfrastructureConfigurationCreateOutput } from '@viasoft/licensing-management/clients/licensing-management/model/infrastructureConfigurationCreateOutput';

@Injectable()
export class LicenseDetailInfrastructureConfigurationService {

  infrastructureConfigurationsSubject: Subject<InfrastructureConfigurationUpdateInput> =
  new Subject<InfrastructureConfigurationUpdateInput>();
  infrastructureConfigurationsFromLicensedTenant: Subject<InfrastructureConfigurationUpdateInput> =
  new Subject<InfrastructureConfigurationUpdateInput>();
  licensingIdentifierReadySubject: Subject<void> = new Subject<void>();
  updateGatewayAdressSubject: Subject<string> = new Subject<string>();
  initialValues: InfrastructureConfigurationUpdateInput = {
    desktopDatabaseName: null,
    gatewayAddress: null
  };

  constructor(private readonly infrastructureConfigurationServiceProxy: InfrastructureConfigurationServiceProxy) { }


  updateGatewayAdressValue(input: string){
    this.updateGatewayAdressSubject.next(input);
  }

  setInfrastructureConfigurationValues(input: InfrastructureConfigurationUpdateInput): void {
    this.infrastructureConfigurationsSubject.next(input);
  }

  setInfrastructureConfigurationsFromLicensedTenant(input: InfrastructureConfigurationUpdateInput): void {
    this.updateInitialValues(input);
    this.infrastructureConfigurationsFromLicensedTenant.next(input);
  }

  licensingIdentifierReady() {
    this.licensingIdentifierReadySubject.next();
  }

  updateInitialValues(input: InfrastructureConfigurationUpdateInput): void {
    this.initialValues = input;
  }

  updateGatewayInitialValue(input: string): void {
    this.initialValues.gatewayAddress = input;
  }

  resetInitialValues(): void {
    this.initialValues = {
      desktopDatabaseName: null,
      gatewayAddress: null
    };
  }

  updateDesktopDatabaseNameValue(input: string): void {
    this.initialValues.desktopDatabaseName = input;
  }

  getByTenantId(input: string): Observable<InfrastructureConfigurationCreateOutput> {
    return this.infrastructureConfigurationServiceProxy.getByTenantId(input);
  }

  update(input: InfrastructureConfigurationUpdateInput) {
    return this.infrastructureConfigurationServiceProxy.update(input);
  }

}
