import { NgModule } from '@angular/core';
import {VsCommonModule} from "@viasoft/common";

import {LICENSING_ENVIRONMENT_I18N_PT} from "./i18n/licensing-info-i18n-pt.const";
import {LICENSING_ENVIRONMENT_I18N_EN} from "./i18n/licensing-info-i18n-en.const";
import {LicensingsEnvironmentComponent} from "./licensings-environment.component";
import {LicensingEnvironmentOrganizationalComponent} from "./licensing-environment-organizational/licensing-environment-organizational.component";
import {TabsViewTemplateModule} from "@viasoft/view-template";
import {AddOrganizationUnitModalComponent} from "./modals/add-organization-unit-modal/add-organization-unit-modal.component";
import {AddOrganizationEnvironmentModalComponent} from "./modals/add-organization-environment-modal/add-organization-environment-modal.component";
import {LicensingsEnvironmentService} from "./licensings-environment.service";
import {LicenseOrganizationService} from "./modals/license-organization.service";
import {
  VsButtonModule,
  VsCheckboxModule,
  VsGridModule,
  VsIconModule,
  VsInputModule,
  VsLabelModule,
  VsLayoutModule,
  VsNoResultModule,
  VsSpinnerModule
} from "@viasoft/components";

@NgModule({
  declarations: [
    LicensingsEnvironmentComponent,
    LicensingEnvironmentOrganizationalComponent,
    AddOrganizationUnitModalComponent,
    AddOrganizationEnvironmentModalComponent
  ],
  imports: [
    VsCommonModule.forChild({
      translates: {
        pt: LICENSING_ENVIRONMENT_I18N_PT,
        en: LICENSING_ENVIRONMENT_I18N_EN
      }
    }),
    VsButtonModule,
    TabsViewTemplateModule,
    VsIconModule,
    VsLabelModule,
    VsGridModule,
    VsNoResultModule,
    VsLayoutModule,
    VsSpinnerModule,
    VsInputModule,
    VsCheckboxModule
  ],
  providers: [
    LicensingsEnvironmentService,
    LicenseOrganizationService
  ]
})
export class LicensingEnvironmentModule {

}
