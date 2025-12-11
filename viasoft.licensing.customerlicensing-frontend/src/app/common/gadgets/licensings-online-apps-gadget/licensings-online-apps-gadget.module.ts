import { NgModule } from '@angular/core';
import { LicensingsInfoCardsGadgetModule } from '../licensings-info-cards-gadget/licensings-info-cards-gadget.module';
import { VsCommonModule } from '@viasoft/common';
import { VsButtonModule } from '@viasoft/components/button';
import { VsLabelModule } from '@viasoft/components/label';
import { VsGadgetModule } from '@viasoft/dashboard';
import { pt } from '../../../i18n/pt';
import { en } from '../../../i18n/en';
import { LicensingsOnlineAppsGadgetComponent } from './licensings-online-apps-gadget.component';



@NgModule({
  declarations: [
    LicensingsOnlineAppsGadgetComponent
  ],
  imports: [
    VsCommonModule.forChild({
      translates: {
        pt, en
      }
    }),
    VsLabelModule,
    VsButtonModule,
    VsGadgetModule,
    LicensingsInfoCardsGadgetModule
  ],
  exports: [
    LicensingsOnlineAppsGadgetComponent
  ]
})
export class LicensingsOnlineAppsGadgetModule {}
