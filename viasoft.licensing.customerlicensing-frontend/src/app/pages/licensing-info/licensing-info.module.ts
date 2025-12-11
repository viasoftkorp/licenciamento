import { NgModule } from '@angular/core';
import { LicensingInfoComponent } from './licensing-info.component';
import { ProductGridComponent } from './product-grid/product-grid.component';
import { LicensingInfoRoutingModule } from './licensing-info-routing.module';
import { VsCommonModule } from '@viasoft/common';
import { LICENSING_INFO_I18N_PT } from './i18n/consts/licensing-info-i18n-pt.const';
import { LICENSING_INFO_I18N_EN } from './i18n/consts/licensing-info-i18n-en.const';
import {
  VsMaskResolverService,
  VsButtonModule,
  VsGridModule,
  VsHeaderModule,
  VsIconModule,
  VsLabelModule,
  VsLayoutModule
} from '@viasoft/components';
import { TabsViewTemplateModule } from '@viasoft/view-template';
import { LicensingsService } from '../licensings/licensings.service';
import { LicensingsOnlineUsersGadgetModule } from 'src/app/common/gadgets/licensings-online-users-gadget/licensings-online-users-gadget.module';
import { LicensingsOnlineAppsGadgetModule } from 'src/app/common/gadgets/licensings-online-apps-gadget/licensings-online-apps-gadget.module';
import { ProductService } from './product-grid/product.service';
import { StatusCellComponent } from './product-grid/status-cell/status-cell.component';
import { IMaskModule } from 'angular-imask';
import { NumberOfUsedLicensesCellComponent } from './product-grid/number-of-licenses-cell/number-of-used-licenses-cell.component';
import { ProductNameCellComponent } from './product-grid/product-name-cell/product-name-cell.component';
import {LicensingEnvironmentModule} from "./licensings-environment/licensing-environment.module";

@NgModule({
  declarations: [
    LicensingInfoComponent,
    ProductGridComponent,
    StatusCellComponent,
    NumberOfUsedLicensesCellComponent,
    ProductNameCellComponent,
  ],
  imports: [
    VsCommonModule.forChild({
      translates: {
        pt: LICENSING_INFO_I18N_PT,
        en: LICENSING_INFO_I18N_EN
      }
    }),
    TabsViewTemplateModule,
    VsGridModule,
    LicensingsOnlineAppsGadgetModule,
    LicensingsOnlineUsersGadgetModule,
    VsLayoutModule,
    VsHeaderModule,
    VsLabelModule,
    VsIconModule,
    LicensingInfoRoutingModule,
    IMaskModule,
    VsButtonModule,
    LicensingEnvironmentModule
  ],
  providers: [
    LicensingsService,
    ProductService,
    VsMaskResolverService
  ],
})
export class LicensingInfoModule { }
