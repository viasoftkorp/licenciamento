import { NgModule } from '@angular/core';
import {Routes , RouterModule } from '@angular/router';
import { LicensingInfoComponent } from './licensing-info.component';
import {LicensingsEnvironmentComponent} from "./licensings-environment/licensings-environment.component";
import {IAuthorizationData, VsAuthorizationGuard} from "@viasoft/common";
import {Policies} from "../../authorizations/policies.class";

const routes: Routes = [
    {
        path: '',
        component: LicensingInfoComponent
    },
    {
      path: 'organizations',
      component:LicensingsEnvironmentComponent,
      canActivate: [VsAuthorizationGuard],
      data: {
        authBackRoute: '',
        permission: [Policies.ReadEnvironments]
      } as IAuthorizationData
    },
    {
        path: 'product/:productType/:id',
        loadChildren: () => import('./product/product.module').then(m => m.ProductModule)
    },
    { path: '**', redirectTo: '' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
  })
  export class LicensingInfoRoutingModule { }
