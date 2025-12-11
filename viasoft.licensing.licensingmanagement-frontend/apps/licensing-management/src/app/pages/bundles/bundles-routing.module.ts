import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BundlesComponent } from './bundles.component';
import { BundleGridComponent } from './bundle-grid/bundle-grid.component';
import { BundleDetailComponent } from './bundle-detail/bundle-detail.component';
import { BundleBatchOperationsComponent } from './bundle-batch-operations/bundle-batch-operations.component';
import { VsAuthorizationGuard, IAuthorizationData } from '@viasoft/common';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';

const routes: Routes = [
  {
    path: 'batch-operations', component: BundleBatchOperationsComponent, canActivate: [VsAuthorizationGuard], data: {
      authBackRoute: '',
      permission: [Policies.BatchOperationInsertBundlesInLicenses]
    } as IAuthorizationData
  },
  {
    path: '', component: BundlesComponent, children: [
      { path: 'new', component: BundleDetailComponent },
      { path: ':id', component: BundleDetailComponent },
      { path: '', component: BundleGridComponent },
      { path: '**', redirectTo: '' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BundlesRoutingModule { }
