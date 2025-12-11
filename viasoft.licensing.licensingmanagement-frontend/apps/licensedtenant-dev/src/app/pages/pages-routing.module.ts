import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'usage-search-tenant-filter-modal',
    loadChildren: () => import('apps/licensedtenant-dev/src/app/pages/usage-search-tenant-filter-select-modal-dev/usage-search-tenant-filter-select-modal-dev.module').then(m => m.CustomUsageSearchTenantFilterSelectModalDevModule)
  },
  {
    path: '**',
    redirectTo: 'usage-search-tenant-filter-modal'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PagesRoutingModule {}
