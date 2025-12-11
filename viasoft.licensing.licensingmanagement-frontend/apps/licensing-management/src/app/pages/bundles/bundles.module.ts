import { NgModule } from '@angular/core';
import { BundlesComponent } from './bundles.component';
import { BundlesRoutingModule } from './bundles-routing.module';
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
  VsSearchInputModule
} from '@viasoft/components';
import { BundleDetailComponent } from './bundle-detail/bundle-detail.component';
import { MatDialogModule } from '@angular/material/dialog';
import { BundleGridComponent } from './bundle-grid/bundle-grid.component';
import { TabsViewTemplateModule } from '@viasoft/view-template';
import { BundlesFormControlService } from './bundles-form-control.service';
import { BundleBatchOperationsComponent } from './bundle-batch-operations/bundle-batch-operations.component';
import { BatchOperationsModule } from '@viasoft/licensing-management/app/tokens/license-batch-operations/batch-operations.module';

@NgModule({
  declarations: [BundlesComponent, BundleDetailComponent, BundleGridComponent, BundleBatchOperationsComponent],
  imports: [
    VsCommonModule.forChild(),
    BundlesRoutingModule,
    ReactiveFormsModule,
    VsHeaderModule,
    VsInputModule,
    VsLabelModule,
    VsButtonModule,
    VsIconModule,
    VsGridModule,
    VsInputModule,
    VsCheckboxModule,
    VsSearchInputModule,
    MatDialogModule,
    TabsViewTemplateModule,
    BatchOperationsModule
  ],
  providers: [BundlesFormControlService]
})
export class BundlesModule { }
