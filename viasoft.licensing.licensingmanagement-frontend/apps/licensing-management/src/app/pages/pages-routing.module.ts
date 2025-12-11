import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authorizationRoutes } from '@viasoft/authorization-management';

const routes: Routes = [
  ...authorizationRoutes,
    {
      path: 'softwares',
      loadChildren: () => import('./softwares/softwares.module').then(m => m.SoftwaresModule)
    },
    {
      path: 'licensings',
      loadChildren: () => import('./licensings/licensings.module').then(m => m.LicensingsModule)
    },
    {
      path: 'products',
      loadChildren: () => import('./bundles/bundles.module').then(m => m.BundlesModule)
    },
    {
      path: 'apps',
      loadChildren: () => import('./apps/apps.module').then(m => m.AppsModule)
    },
    {
      path: 'accounts',
      loadChildren: () => import('./accounts/accounts.module').then(m => m.AccountsModule)
    },
    {
      path: 'auditing',
      loadChildren: () => import('./auditing/auditing.module').then(m => m.AuditingModule)
    },
    {
      path: '',
      redirectTo: 'licensings',
      pathMatch: 'full'
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PagesRoutingModule { }
