import { NgModule } from '@angular/core';
import {Routes , RouterModule } from '@angular/router';
import { authorizationRoutes } from '@viasoft/authorization-management';
import {IAuthorizationData, VsAuthorizationGuard} from "@viasoft/common";
import {Policies} from "../authorizations/policies.class";

const routes: Routes = [
    ...authorizationRoutes,
    {
        path: 'licensing-info',
        loadChildren: () => import('./licensing-info/licensing-info.module').then(m => m.LicensingInfoModule),
    },

    {
      path: 'settings',
      loadChildren: () => import('./settings/settings.module').then(m => m.SettingsModule),
      canActivate: [VsAuthorizationGuard],
      canActivateChild: [VsAuthorizationGuard],
      data:  {
        authBackRoute: '',
        permission: [Policies.ReadSettings]
      } as IAuthorizationData
    },
    {
        path: '',
        loadChildren: () => import('./licensings/licensings.module').then(m => m.LicensingsModule)
    },
    {
        path: '**',
        redirectTo: ''
    }

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
  })
  export class PagesRoutingModule { }
