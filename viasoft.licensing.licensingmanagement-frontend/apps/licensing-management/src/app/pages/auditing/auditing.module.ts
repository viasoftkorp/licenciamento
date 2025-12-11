import { NgModule } from '@angular/core';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';

import { VsCommonModule } from '@viasoft/common';
import { VsGridModule } from '@viasoft/components/grid';
import { AuditingComponent } from './auditing.component';
import { AuditingRoutingModule } from './auditing-routing.module';
import {
  AuditingSeeMoreComponent
} from "@viasoft/licensing-management/app/tokens/modals/auditing-see-more/auditing-see-more.component";
import {
  VsButtonModule, VsDialogModule,
  VsFormModule,
  VsInputModule,
  VsLabelModule,
  VsLayoutModule,
  VsTextareaModule
} from "@viasoft/components";
import {ReactiveFormsModule} from "@angular/forms";
import {DatePipe} from "@angular/common";


@NgModule({
  declarations: [
    AuditingComponent,
    AuditingSeeMoreComponent
  ],
  imports: [
    VsCommonModule.forChild(),
    VsGridModule,
    AuditingRoutingModule,
    MatDatepickerModule,
    MatNativeDateModule,
    VsLayoutModule,
    VsLabelModule,
    ReactiveFormsModule,
    VsInputModule,
    VsButtonModule,
    VsTextareaModule,
    VsFormModule,
    VsDialogModule
  ],
  providers: [DatePipe]
})
export class AuditingModule { }
