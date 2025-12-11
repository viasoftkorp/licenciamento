import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { VsCommonModule } from '@viasoft/common';
import {
  VsAutocompleteModule,
  VsButtonModule,
  VsCheckboxModule,
  VsDatepickerModule,
  VsDialogModule,
  VsFormModule,
  VsGridModule,
  VsHeaderModule,
  VsIconModule,
  VsInputModule,
  VsLabelModule,
  VsLayoutModule,
  VsNoResultModule,
  VsRadioGroupModule,
  VsSearchInputModule,
  VsSelectModule,
  VsSpinnerModule,
  VsTabGroupModule,
  VsTextareaModule,
} from '@viasoft/components';
import {
  FileProviderConfigurationModule,
} from '@viasoft/licensing-management/app/tokens/file-provider-configuration/file-provider-configuration.module';
import {
  LicenseConsumeInfoComponent,
} from '@viasoft/licensing-management/app/tokens/modals/license-consume-info/license-consume-info.component';
import {
  LicensedAppsNumberSelectComponent,
} from '@viasoft/licensing-management/app/tokens/modals/licensed-apps-number-select/licensed-apps-number-select.component';
import {
  LicensesNumberSelectComponent,
} from '@viasoft/licensing-management/app/tokens/modals/licenses-number-select/licenses-number-select.component';
import {
  LooseAppsNumberSelectComponent,
} from '@viasoft/licensing-management/app/tokens/modals/loose-apps-number-select/loose-apps-number-select.component';
import { TabsViewTemplateModule } from '@viasoft/view-template';
import { AddNamedUserComponent } from '../../tokens/modals/add-named-user/add-named-user.component';

import { MatMenuModule } from '@angular/material/menu';
import {
  LicenseDetailLicensesServerService
} from "@viasoft/licensing-management/app/pages/licensings/license-detail/license-detail-licenses-server/license-detail-licenses-server.service";
import {
  LicensedTenantSettingsInfoComponent
} from "@viasoft/licensing-management/app/tokens/modals/licensed-tenant-settings-info/licensed-tenant-settings-info.component";
import {
  AddOrganizationEnvironmentModalComponent,
} from '../../tokens/modals/add-organization-environment-modal/add-organization-environment-modal.component';
import {
  AddOrganizationUnitModalComponent,
} from '../../tokens/modals/add-organization-unit-modal/add-organization-unit-modal.component';
import { FilterAppModalComponent } from '../../tokens/modals/filter-app-modal/filter-app-modal.component';
import { AddNamedUserService } from '../../tokens/services/modals-service/add-named-user.service';
import { AddLicenseDropdownComponent } from './license-detail/add-license-dropdown/add-license-dropdown.component';
import {
  LicenseDetailFileQuotaComponent,
} from './license-detail/license-detail-file-quota/license-detail-file-quota.component';
import {
  LicenseDetailInfrastructureConfigurationComponent,
} from './license-detail/license-detail-infrastructure-configuration/license-detail-infrastructure-configuration.component';
import {
  LicenseDetailInfrastructureConfigurationService,
} from './license-detail/license-detail-infrastructure-configuration/license-detail-infrastructure-configuration.service';
import { LicenseDetailLicensesServerComponent } from './license-detail/license-detail-licenses-server/license-detail-licenses-server.component';
import {
  LicenseDetailOrganizationComponent,
} from './license-detail/license-detail-organization/license-detail-organization.component';
import { LicenseDetailProductsComponent } from './license-detail/license-detail-products/license-detail-products.component';
import { LicenseDetailComponent } from './license-detail/license-detail.component';
import { LicenseGridComponent } from './license-grid/license-grid.component';
import { LicensedTenantViewService } from './licensed-tenant-view.service';
import { LicensingsFormControlServices } from './licensings-form-control.service';
import { LicensingsRoutingModule } from './licensings-routing.module';
import { LicensingsComponent } from './licensings.component';
import { NamedUsersAppService } from './named-users/named-users-app.service';
import { NamedUsersBundleService } from './named-users/named-users-bundle.service';
import { NamedUsersComponent } from './named-users/named-users.component';

@NgModule({
  declarations: [LicensingsComponent,
    LicenseGridComponent,
    LicenseDetailComponent,
    LicensesNumberSelectComponent,
    LicensedAppsNumberSelectComponent,
    LooseAppsNumberSelectComponent,
    LicenseDetailFileQuotaComponent,
    LicenseConsumeInfoComponent,
    LicenseDetailInfrastructureConfigurationComponent,
    LicenseDetailOrganizationComponent,
    AddOrganizationUnitModalComponent,
    AddOrganizationEnvironmentModalComponent,
    NamedUsersComponent,
    AddNamedUserComponent,
    LicenseDetailProductsComponent,
    AddLicenseDropdownComponent,
    FilterAppModalComponent,
    LicenseDetailLicensesServerComponent,
    LicensedTenantSettingsInfoComponent
  ],
  imports: [
    VsCommonModule.forChild(),
    LicensingsRoutingModule,
    ReactiveFormsModule,
    TabsViewTemplateModule,
    VsButtonModule,
    VsCheckboxModule,
    VsDatepickerModule,
    VsHeaderModule,
    VsIconModule,
    VsInputModule,
    VsFormModule,
    VsGridModule,
    VsLabelModule,
    VsLayoutModule,
    VsNoResultModule,
    VsRadioGroupModule,
    VsSearchInputModule,
    VsSelectModule,
    VsSpinnerModule,
    VsTabGroupModule,
    VsTextareaModule,
    MatNativeDateModule,
    MatDatepickerModule,
    FileProviderConfigurationModule,
    VsDialogModule,
    VsAutocompleteModule,
    MatMenuModule,
  ],
  providers: [
    LicensingsFormControlServices,
    LicensedTenantViewService,
    LicenseDetailInfrastructureConfigurationService,
    LicenseDetailLicensesServerService,
    NamedUsersBundleService,
    NamedUsersAppService,
    AddNamedUserService
  ]
})
export class LicensingsModule { }
