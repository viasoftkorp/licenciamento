import { NgModule } from '@angular/core';
import { LicenseGridBatchOperationsComponent } from './grids/license-grid/license-grid.component';
import { VsCommonModule } from '@viasoft/common';
import { BundleGridBatchOperationsComponent } from './grids/bundle-grid/bundle-grid.component';
import { VsIconModule } from '@viasoft/components/icon';
import { VsHeaderModule } from '@viasoft/components/header';
import { VsLabelModule } from '@viasoft/components/label';
import { AppGridBatchOperationsComponent } from './grids/app-grid/app-grid.component';
import { BatchOperationsLicenseNumberSelectComponent } from './modals/batch-operations-license-number/bundles-select/batch-operations-license-number-select.component';
import { VsButtonModule, VsInputModule, VsSpinnerModule, VsGridModule, VsDialogModule } from '@viasoft/components';
import { BatchOperationsLoadingComponent } from './modals/batch-operations-loading/batch-operations-loading.component';

@NgModule({
  declarations: [LicenseGridBatchOperationsComponent, BundleGridBatchOperationsComponent,
     AppGridBatchOperationsComponent, BatchOperationsLicenseNumberSelectComponent,
      BatchOperationsLoadingComponent],
  imports: [
    VsCommonModule.forChild(),
    VsGridModule,
    VsHeaderModule,
    VsLabelModule,
    VsIconModule,
    VsButtonModule,
    VsInputModule,
    VsSpinnerModule,
    VsDialogModule
  ],
  exports: [LicenseGridBatchOperationsComponent, BundleGridBatchOperationsComponent,
     AppGridBatchOperationsComponent, BatchOperationsLicenseNumberSelectComponent,
      BatchOperationsLoadingComponent]
})
export class BatchOperationsModule { }
