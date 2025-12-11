import { NgModule } from '@angular/core';
import { VsCommonModule } from '@viasoft/common';

import { PagesRoutingModule } from './pages-routing.module';

@NgModule({
  imports: [
    VsCommonModule.forChild(),
    PagesRoutingModule,
  ]
})
export class PagesModule { }
