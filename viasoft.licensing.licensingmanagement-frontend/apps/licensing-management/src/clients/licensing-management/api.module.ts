import { NgModule, ModuleWithProviders, SkipSelf, Optional } from '@angular/core';
import { Configuration } from './configuration';
import { HttpClient } from '@angular/common/http';


import { AccountServiceProxy } from './api/accountServiceProxy';
import { AppServiceProxy } from './api/appServiceProxy';
import { AppQuotaServiceProxy } from './api/appQuotaServiceProxy';
import { AuditingLogServiceProxy } from './api/auditingLogServiceProxy';
import { AuthorizationServiceProxy } from './api/authorizationServiceProxy';
import { BundleServiceProxy } from './api/bundleServiceProxy';
import { CnpjServiceProxy } from './api/cnpjServiceProxy';
import { DomainServiceProxy } from './api/domainServiceProxy';
import { HostTenantServiceProxy } from './api/hostTenantServiceProxy';
import { InfrastructureConfigurationServiceProxy } from './api/infrastructureConfigurationServiceProxy';
import { LicenseServerServiceProxy } from './api/licenseServerServiceProxy';
import { LicensedTenantBatchOperationsServiceProxy } from './api/licensedTenantBatchOperationsServiceProxy';
import { LicensedTenantViewServiceProxy } from './api/licensedTenantViewServiceProxy';
import { LicensingServiceProxy } from './api/licensingServiceProxy';
import { LicensingManagementStatisticsServiceProxy } from './api/licensingManagementStatisticsServiceProxy';
import { LicensingOrganizationServiceProxy } from './api/licensingOrganizationServiceProxy';
import { SoftwareServiceProxy } from './api/softwareServiceProxy';
import { TenantInfoServiceProxy } from './api/tenantInfoServiceProxy';
import { TenantQuotaServiceProxy } from './api/tenantQuotaServiceProxy';
import { ZipCodeServiceProxy } from './api/zipCodeServiceProxy';
import { ProductServiceProxy } from './api/productServiceProxy';

@NgModule({
  providers: [
    AccountServiceProxy,
    AppServiceProxy,
    AppQuotaServiceProxy,
    AuditingLogServiceProxy,
    AuthorizationServiceProxy,
    BundleServiceProxy,
    CnpjServiceProxy,
    DomainServiceProxy,
    HostTenantServiceProxy,
    InfrastructureConfigurationServiceProxy,
    LicenseServerServiceProxy,
    LicensedTenantBatchOperationsServiceProxy,
    LicensedTenantViewServiceProxy,
    LicensingServiceProxy,
    LicensingManagementStatisticsServiceProxy,
    LicensingOrganizationServiceProxy,
    ProductServiceProxy,
    SoftwareServiceProxy,
    TenantInfoServiceProxy,
    TenantQuotaServiceProxy,
    ZipCodeServiceProxy ]
})
export class ApiModule {}
