import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HttpClientModule } from '@angular/common/http';
import {VsCommonModule} from '@viasoft/common';
import {
  VsTabGroupModule,
  VsLayoutModule,
  VsFormModule,
  VsLabelModule,
  VsInputModule,
  VsButtonModule,
  VsSelectModule,
  VsIconModule,
} from '@viasoft/components';
import {AuthorizationModule} from "@viasoft/common";
import {SettingsRoutingModule} from "./settings-routing.module";
import {SettingsComponent} from "./settings.component";
import {InfrastructureSettingsComponent } from './infrastructure-settings/infrastructure-settings.component';
import {pt} from "./i18n/pt";
import {en} from "./i18n/en";
import {TabsViewTemplateModule} from "@viasoft/view-template";
import {SharedService} from "./shared.service";


@NgModule({
  declarations: [SettingsComponent, InfrastructureSettingsComponent],
  imports: [
    VsCommonModule.forChild({
      translates: {
        pt: pt,
        en: en
      }
    }),
    SettingsRoutingModule,
    VsTabGroupModule,
    VsButtonModule,
    TabsViewTemplateModule,
    VsLayoutModule,
    VsFormModule,
    VsInputModule,
    VsSelectModule,
    VsIconModule,
    VsLabelModule,
  ],
  providers: [SharedService]
})
export class SettingsModule { }
