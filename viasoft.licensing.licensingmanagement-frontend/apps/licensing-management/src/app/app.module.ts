import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { VsAppCoreModule } from '@viasoft/app-core';
import { VS_BACKEND_URL } from '@viasoft/client-core';
import { MustBeLoggedAuthGuard } from '@viasoft/common';
import { API_GATEWAY, VS_UI_ERROR_SETTINGS, VsHttpModule, VsUiErrorSettings, ensureTrailingSlash } from '@viasoft/http';
import { ApiModule, BASE_PATH } from '@viasoft/licensing-management/clients/licensing-management';
import { environment } from '@viasoft/licensing-management/environments/environment';
import { VsNavigationViewComponent, VsNavigationViewModule } from '@viasoft/navigation';

import { AUTHORIZATION_PROVIDER, VsAuthorizationModule } from '@viasoft/authorization-management';
import { en } from './i18n/en';
import { pt } from './i18n/pt';
import { NAVIGATION_MENU_ITEMS } from './navigation-items.const';
import { AuthorizationService } from './tokens/services/authorization/authorization.service';

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        VsHttpModule.forRoot({
            environment: environment,
            isCompanyBased: false,
            errorDialogSettings: {
                shouldShowErrorDialog: (error) => [401, 404, 500, 503].includes(error.status)
            }
        }),
        VsAppCoreModule.forRoot({
            apiPrefix: 'Licensing/LicensingManagement',
            portalConfig: {
                appId: 'LS01',
                appName: 'license-management',
                domain: 'Customized',
                navbarTitle: 'Licenciamento'
            },
            translates: {
                pt, en
            },
            environment,
            navigation: NAVIGATION_MENU_ITEMS
        }),
        ApiModule,
        VsNavigationViewModule,
        VsAuthorizationModule.forRoot(),
        RouterModule.forRoot([
            {
                path: '',
                loadChildren: () => import('./pages/pages.module').then(m => m.PagesModule),
                canActivate: [MustBeLoggedAuthGuard]
            }
        ], {}),
    ],
    providers: [
        { provide: API_GATEWAY, useFactory: () => ensureTrailingSlash(environment['settings']['backendUrl']) },
        { provide: VS_BACKEND_URL, useFactory: () => ensureTrailingSlash(environment['settings']['backendUrl']) },
        { provide: BASE_PATH, useFactory: () => environment['settings']['backendUrl'] },
        { provide: AUTHORIZATION_PROVIDER, useClass: AuthorizationService },
        { provide: VS_UI_ERROR_SETTINGS, useValue: { maxErrorModalCount: 1 } as VsUiErrorSettings }
    ],
    bootstrap: [VsNavigationViewComponent]
})
export class AppModule { }
