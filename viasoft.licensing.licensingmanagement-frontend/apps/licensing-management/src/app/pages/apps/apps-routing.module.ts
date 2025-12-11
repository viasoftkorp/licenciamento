import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppGridComponent } from './app-grid/app-grid.component';
import { AppDetailComponent } from './app-detail/app-detail.component';
import { AppsComponent } from './apps.component';
import { VsAuthorizationGuard, IAuthorizationData } from '@viasoft/common';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';
import { AppInBundlesBatchOperationsComponent } from './app-batch-operations/apps-in-bundles/app-in-bundles-batch-operations.component';
import { AppInLicensesBatchOperationsComponent } from './app-batch-operations/apps-in-licenses/app-in-licenses-batch-operations.component';

const routes: Routes = [
  {
    path: 'batch-operations-licenses', component: AppInLicensesBatchOperationsComponent, canActivate: [VsAuthorizationGuard], data: {
      authBackRoute: '',
      permission: [Policies.BatchOperationInsertAppsInLicenses],
    } as IAuthorizationData
  },
  {
    path: 'batch-operations-products', component: AppInBundlesBatchOperationsComponent, canActivate: [VsAuthorizationGuard], data: {
      authBackRoute: '',
      permission: [Policies.BatchOperationInsertAppsInBundles],
    } as IAuthorizationData
  },
  {
    path: '', component: AppsComponent, children: [
      { path: 'new', component: AppDetailComponent },
      { path: ':id', component: AppDetailComponent },
      { path: '', component: AppGridComponent },
      { path: '**', redirectTo: '' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppsRoutingModule { }
