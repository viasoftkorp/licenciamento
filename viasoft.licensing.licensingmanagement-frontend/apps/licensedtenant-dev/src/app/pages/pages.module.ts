import { NgModule } from '@angular/core';
import { VsCommonModule } from '@viasoft/common';
import { PagesRoutingModule } from 'apps/licensedtenant-dev/src/app/pages/pages-routing.module';

@NgModule({
  declarations: [],
  imports: [VsCommonModule.forChild(), PagesRoutingModule]
})
export class PagesModule {}
