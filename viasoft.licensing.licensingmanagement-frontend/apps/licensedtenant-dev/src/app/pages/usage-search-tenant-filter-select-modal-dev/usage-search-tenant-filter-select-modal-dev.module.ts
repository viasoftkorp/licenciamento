import { NgModule } from '@angular/core';
import { UsageSearchTenantFilterSelectModalModule } from 'libs/licensing-licensingmanagement-licensedtenant/src/public-api';
import { UsageSearchTenantFilterSelectModalDevComponent } from './usage-search-tenant-filter-select-modal-dev.component';
import { CustomUsageSearchTenantFilterSelectModalDevRoutingModule } from './usage-search-tenant-filter-select-modal-dev-routing.module';
import { VsButtonModule } from '@viasoft/components/button';
import { VsIconModule } from '@viasoft/components/icon';

@NgModule({
  declarations: [UsageSearchTenantFilterSelectModalDevComponent],
  imports: [
    UsageSearchTenantFilterSelectModalModule,
    CustomUsageSearchTenantFilterSelectModalDevRoutingModule,
    VsButtonModule,
    VsIconModule
  ]
})
export class CustomUsageSearchTenantFilterSelectModalDevModule { }
