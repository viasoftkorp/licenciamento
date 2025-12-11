import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsageSearchTenantFilterSelectModalDevComponent } from './usage-search-tenant-filter-select-modal-dev.component';

const routes: Routes = [
  {
    path: '',
    component: UsageSearchTenantFilterSelectModalDevComponent
  }
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class CustomUsageSearchTenantFilterSelectModalDevRoutingModule { }
