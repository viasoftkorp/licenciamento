import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { VsAppCoreModule } from '@viasoft/app-core';
import { VS_BACKEND_URL } from '@viasoft/client-core';
import { VsAuthService } from "@viasoft/common";
import { API_GATEWAY, VsHttpModule, VsUiErrorSettings, VS_UI_ERROR_SETTINGS } from '@viasoft/http';
import { VsNavigationViewComponent, VsNavigationViewModule } from '@viasoft/navigation';
import { AppRoutingModule } from 'apps/licensedtenant-dev/src/app/app-routing.module';
import { LICENSED_TENANT_DEV_I18N_EN } from 'apps/licensedtenant-dev/src/app/i18n/consts/licensedtenant-dev-i18n-en.const';
import { LICENSED_TENANT_DEV_I18N_PT } from 'apps/licensedtenant-dev/src/app/i18n/consts/licensedtenant-dev-i18n-pt.const';
import { AppConsts } from 'apps/licensedtenant-dev/src/app/tokens/consts/app-consts.const';
import { NAVIGATION_MENU_ITEMS } from 'apps/licensedtenant-dev/src/app/tokens/consts/navigation.const';
import { environment } from 'apps/licensedtenant-dev/src/environments/environment';

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    VsHttpModule.forRoot({
      environment,
      errorDialogSettings: {
        shouldShowErrorDialog: (error) => [401, 404, 500, 503].includes(error.status)
      }
    }),
    VsAppCoreModule.forRoot({
      apiPrefix: 'Licensing/LicensingManagement',
      portalConfig: {
        appId: 'LICENSED_TENANT_DEV',
        appName: 'licensing-licensingmanagement-licensedtenant-dev',
        domain: 'Customized',
        navbarTitle: 'LicensedTenantDev.Navigation.Title'
      },
      translates: {
        en: LICENSED_TENANT_DEV_I18N_EN,
        pt: LICENSED_TENANT_DEV_I18N_PT
      },
      environment,
      navigation: NAVIGATION_MENU_ITEMS,
    }),
    VsNavigationViewModule,
    AppRoutingModule
  ],
  providers: [
    { provide: API_GATEWAY, useFactory: () => AppConsts.apiGateway() },
    { provide: VS_BACKEND_URL, useFactory: () => AppConsts.apiGateway() },
    { provide: VS_UI_ERROR_SETTINGS, useValue: { maxErrorModalCount: 1 } as VsUiErrorSettings }
  ],
  bootstrap: [VsNavigationViewComponent]
})
export class AppModule {
  constructor(authService: VsAuthService) {
    authService.runInitialLoginSequence(true).then();
  }
}
