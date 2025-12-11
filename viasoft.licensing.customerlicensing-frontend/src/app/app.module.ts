import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { VsAppCoreModule } from '@viasoft/app-core';
import { VS_BACKEND_URL } from '@viasoft/client-core';
import {MustBeLoggedAuthGuard, VsAuthService} from '@viasoft/common';
import { API_GATEWAY, VS_UI_ERROR_SETTINGS, VsHttpModule, VsUiErrorSettings } from '@viasoft/http';
import { VsNavbarService, VsNavigationViewComponent, VsNavigationViewModule } from '@viasoft/navigation';
import { ApiModule } from 'src/client/authorization/api.module';
import { environment } from 'src/environments/environment';
import{AuthorizationModule} from "@viasoft/common";

import { ApiModule as CustomerApiModule, BASE_PATH as LICENSING_PATH } from '../client/customer-licensing';
import { ApiModule as DashboardApiModule, BASE_PATH as DASHBOARD_PATH } from '../client/dashboard';
import { AppConsts } from './common/tokens/consts/app-consts.const';
import { en } from './i18n/en';
import { pt } from './i18n/pt';
import { SIDENAV_MENU_ITEMS } from './navigation-items.const';

@NgModule({
  declarations: [
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    VsHttpModule.forRoot({
      environment: environment,
      isCompanyBased: false
    }),
    VsAppCoreModule.forRoot({
      portalConfig: {
        appId: 'LS02',
        appName: 'customer-licensing',
        domain: 'Customized',
        navbarTitle: 'Monitoramento'
      },
      apiPrefix: 'licensing/customerLicensing',
      translates: {
        pt, en
      },
      environment
    }),
    AuthorizationModule.forRoot(),
    CustomerApiModule,
    DashboardApiModule,
    ApiModule,
    RouterModule.forRoot([
      { path: '', loadChildren: () => import('./pages/pages.module').then(m => m.PagesModule), canActivate: [MustBeLoggedAuthGuard] }
    ]),
    VsNavigationViewModule
  ],
  providers: [
    { provide: API_GATEWAY, useFactory: () => AppConsts.apiGateway() },
    { provide: VS_BACKEND_URL, useFactory: () => AppConsts.apiGateway() },
    { provide: LICENSING_PATH, useExisting: VS_BACKEND_URL },
    { provide: DASHBOARD_PATH, useExisting: VS_BACKEND_URL },
    { provide: VS_UI_ERROR_SETTINGS, useValue: { maxErrorModalCount: 1 } as VsUiErrorSettings },
  ],
  bootstrap: [VsNavigationViewComponent]
})
export class AppModule {
  constructor(navbarService: VsNavbarService, authService: VsAuthService) {
    navbarService.setNavigationMenu(SIDENAV_MENU_ITEMS);
  }
}
