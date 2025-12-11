import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { VsCommonModule } from '@viasoft/common';
import { VsButtonModule } from '@viasoft/components/button';
import { VsDialogModule } from '@viasoft/components/dialog';
import { VsFormModule } from '@viasoft/components/form';
import { VsGridModule } from '@viasoft/components/grid';
import { VsIconModule } from '@viasoft/components/icon';
import { VsInputModule } from '@viasoft/components/input';
import { VsLabelModule } from '@viasoft/components/label';
import { VsLayoutModule } from '@viasoft/components/layout';
import { VsSearchModule } from '@viasoft/components/search';
import { FileQuotaLicensedAppSelectComponent } from '../modals/file-quota-licensed-app-select/file-quota-licensed-app-select.component';
import { FileProviderConfigurationModalComponent } from './file-provider-configuration-modal/file-provider-configuration-modal.component';
import { FileProviderConfigurationComponent } from './file-provider-configuration.component';
import { FileProviderConfigurationService } from './file-provider-configuration.service';

@NgModule({
  declarations: [
    FileProviderConfigurationComponent,
    FileProviderConfigurationModalComponent,
    FileQuotaLicensedAppSelectComponent
  ],
  imports: [
    VsCommonModule.forChild(),
    FormsModule,
    ReactiveFormsModule,
    VsInputModule,
    VsDialogModule,
    VsSearchModule,
    VsGridModule,
    VsFormModule,
    VsLayoutModule,
    VsButtonModule,
    VsIconModule,
    VsLabelModule
  ],
  providers: [
    FileProviderConfigurationService
  ],
  exports: [
    FileProviderConfigurationComponent,
    FileProviderConfigurationModalComponent
  ]
})
export class FileProviderConfigurationModule { }
