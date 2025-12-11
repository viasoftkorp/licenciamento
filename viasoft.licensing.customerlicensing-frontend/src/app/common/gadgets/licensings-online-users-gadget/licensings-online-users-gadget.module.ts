import { NgModule } from '@angular/core';
import { VsButtonModule } from '@viasoft/components/button';
import { VsLabelModule } from '@viasoft/components/label';
import { VsGadgetModule } from '@viasoft/dashboard';
import { LicensingsInfoCardsGadgetModule } from '../licensings-info-cards-gadget/licensings-info-cards-gadget.module';
import { VsCommonModule } from '@viasoft/common';
import { LicensingsOnlineUsersGadgetComponent } from './licensings-online-users-gadget.component';
import { pt } from '../../../i18n/pt';
import { en } from '../../../i18n/en';


@NgModule({
  declarations: [
    LicensingsOnlineUsersGadgetComponent
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
    LicensingsOnlineUsersGadgetComponent
  ]
})
export class LicensingsOnlineUsersGadgetModule { }
