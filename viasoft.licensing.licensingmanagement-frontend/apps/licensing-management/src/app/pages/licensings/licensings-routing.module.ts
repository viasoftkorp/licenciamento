import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {
  LicenseDetailFileQuotaComponent,
} from './license-detail/license-detail-file-quota/license-detail-file-quota.component';
import {
  LicenseDetailInfrastructureConfigurationComponent,
} from './license-detail/license-detail-infrastructure-configuration/license-detail-infrastructure-configuration.component';
import {
  LicenseDetailOrganizationComponent,
} from './license-detail/license-detail-organization/license-detail-organization.component';
import { LicenseDetailProductsComponent } from './license-detail/license-detail-products/license-detail-products.component';
import { LicenseDetailComponent } from './license-detail/license-detail.component';
import { LicenseGridComponent } from './license-grid/license-grid.component';
import { LicensingsComponent } from './licensings.component';
import {
  LicenseDetailLicensesServerComponent
} from "@viasoft/licensing-management/app/pages/licensings/license-detail/license-detail-licenses-server/license-detail-licenses-server.component";

const routes: Routes = [
  {
    path: '', component: LicensingsComponent, children: [
      { path: 'new', component: LicenseDetailComponent },
      {
        path: ':id', component: LicenseDetailComponent, children: [
          { path: 'file-quota', component: LicenseDetailFileQuotaComponent },
          { path: 'infrastructure-configuration', component: LicenseDetailInfrastructureConfigurationComponent },
          { path: 'organization', component: LicenseDetailOrganizationComponent },
          { path: 'licenses-server', component: LicenseDetailLicensesServerComponent },
          { path: '', component: LicenseDetailProductsComponent, data: { activeChildren: 'defaultRoute' }},
          { path: '**', redirectTo: '' }
        ]
      },
      { path: '', component: LicenseGridComponent },
      { path: '**', redirectTo: '' }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LicensingsRoutingModule { }
