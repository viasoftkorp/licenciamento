import { NgModule, ModuleWithProviders, SkipSelf, Optional } from '@angular/core';
import { Configuration } from './configuration';
import { HttpClient } from '@angular/common/http';


import { LicenseUsageInRealTimeServiceProxy } from './api/licenseUsageInRealTimeServiceProxy';
import { LicenseUsageStatisticsServiceProxy } from './api/licenseUsageStatisticsServiceProxy';
import { LicenseUserBehaviourServiceProxy } from './api/licenseUserBehaviourServiceProxy';
import { AuthorizationServiceProxy } from '../authorization/authorizationServiceProxy';

@NgModule({
  imports:      [],
  declarations: [],
  exports:      [],
  providers: [
    LicenseUsageInRealTimeServiceProxy,
    LicenseUsageStatisticsServiceProxy,
    LicenseUserBehaviourServiceProxy ]
})
export class ApiModule {
    public static forRoot(configurationFactory: () => Configuration): ModuleWithProviders<ApiModule> {
        return {
            ngModule: ApiModule,
            providers: [ { provide: Configuration, useFactory: configurationFactory } ]
        };
    }

    constructor( @Optional() @SkipSelf() parentModule: ApiModule,
                 @Optional() http: HttpClient) {
        if (parentModule) {
            throw new Error('ApiModule is already loaded. Import in your base AppModule only.');
        }
        if (!http) {
            throw new Error('You need to import the HttpClientModule in your AppModule! \n' +
            'See also https://github.com/angular/angular/issues/20575');
        }
    }
}
