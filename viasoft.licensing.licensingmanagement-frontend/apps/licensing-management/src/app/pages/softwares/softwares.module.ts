import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterModule, Routes } from '@angular/router';
import { VsCommonModule } from '@viasoft/common';
import {
    VsButtonModule,
    VsCheckboxModule,
    VsGridModule,
    VsHeaderModule,
    VsIconModule,
    VsInputModule,
    VsLabelModule,
    VsTabGroupModule,
    VsDialogModule,
} from '@viasoft/components';
import { TabsViewTemplateModule } from '@viasoft/view-template';

import { SoftwareGridComponent } from './software-grid/software-grid.component';
import { SoftwaresRoutingModule } from './softwares-routing.module';
import { SoftwareDetailComponent } from './software-detail/software-detail.component';
import { SoftwaresFormControlService } from './softwares-form-control.service';
import { SoftwaresComponent } from './softwares.component';

const routes: Routes = [
  { path: '**', component: SoftwareGridComponent},
  { path: 'apps/new', component: SoftwareDetailComponent},
  { path: ':id', component: SoftwareDetailComponent}
];

@NgModule({
  declarations: [SoftwaresComponent, SoftwareGridComponent , SoftwareDetailComponent],
  imports: [
    VsCommonModule.forChild(),
    SoftwaresRoutingModule,
    ReactiveFormsModule,

    VsHeaderModule,
    VsLabelModule,
    VsButtonModule,
    VsIconModule,
    VsGridModule,
    VsInputModule,
    VsCheckboxModule,
    MatSidenavModule,
    MatListModule,
    MatTabsModule,
    VsTabGroupModule,
    TabsViewTemplateModule,
    SoftwaresRoutingModule,
    VsDialogModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    SoftwaresComponent, SoftwareGridComponent, SoftwareDetailComponent
  ],
  providers: [SoftwaresFormControlService]
})
export class SoftwaresModule { }
