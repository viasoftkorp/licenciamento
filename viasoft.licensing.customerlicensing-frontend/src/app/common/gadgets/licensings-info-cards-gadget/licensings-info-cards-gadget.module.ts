import { NgModule } from '@angular/core';
import { VsCommonModule } from '@viasoft/common';
import { VsButtonModule } from '@viasoft/components/button';
import { VsLabelModule } from '@viasoft/components/label';
import { VsGadgetModule } from '@viasoft/dashboard';
import { pt } from '../../../i18n/pt';
import { en } from '../../../i18n/en';
import { LicensingsInfoCardsGadgetComponent } from './licensings-info-cards-gadget.component';



@NgModule({
  declarations: [LicensingsInfoCardsGadgetComponent],
  imports: [
    VsCommonModule.forChild({
      translates: {
        pt, en
      }
    }),
    VsLabelModule,
    VsButtonModule,
    VsGadgetModule
  ],
  exports: [LicensingsInfoCardsGadgetComponent]
})
export class LicensingsInfoCardsGadgetModule {}
