import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppGridComponent } from './app-grid/app-grid.component';
import { AppsRoutingModule } from './apps-routing.module';
import { VsCommonModule } from '@viasoft/common';
import { ReactiveFormsModule } from '@angular/forms';
import {
  VsButtonModule,
  VsCheckboxModule,
  VsGridModule,
  VsHeaderModule,
  VsIconModule,
  VsInputModule,
  VsLabelModule,
  VsSearchInputModule,
  VsDialogModule,
  VsAutocompleteModule
} from '@viasoft/components';
import { AppDetailComponent } from './app-detail/app-detail.component';
import { MatDialogModule } from '@angular/material/dialog';
import { AppsComponent } from './apps.component';
import { TabsViewTemplateModule } from '@viasoft/view-template';
import { AppsFormControlService } from './apps-form-control.service';
import { BatchOperationsModule } from '@viasoft/licensing-management/app/tokens/license-batch-operations/batch-operations.module';
import { AppInBundlesBatchOperationsComponent } from './app-batch-operations/apps-in-bundles/app-in-bundles-batch-operations.component';
import { AppInLicensesBatchOperationsComponent } from './app-batch-operations/apps-in-licenses/app-in-licenses-batch-operations.component';

@NgModule({
  declarations: [AppsComponent, AppDetailComponent, AppGridComponent, AppInBundlesBatchOperationsComponent, AppInLicensesBatchOperationsComponent],
  imports: [
    CommonModule,
    AppsRoutingModule,
    VsCommonModule.forChild(),
    MatDialogModule,
    VsSearchInputModule,
    ReactiveFormsModule,
    VsHeaderModule,
    VsLabelModule,
    VsButtonModule,
    VsIconModule,
    VsGridModule,
    VsInputModule,
    VsCheckboxModule,
    TabsViewTemplateModule,
    VsAutocompleteModule,
    VsDialogModule,
    BatchOperationsModule
  ],
  exports: [AppsComponent, AppDetailComponent, AppGridComponent],
  providers: [AppsFormControlService]
})
export class AppsModule { }
