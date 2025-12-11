import { NgModule } from '@angular/core';
import { PagesRoutingModule } from './pages-routing.module';
import { VsCommonModule } from '@viasoft/common';
import { AddNamedUserModalComponent } from './tokens/modals/named-user/add-named-user-modal.component';
import { FormsModule } from '@angular/forms';
import { AddNamedUserService } from './tokens/modals/services/add-named-user.service';
import { AUTHORIZATION_PROVIDER, VsAuthorizationModule } from '@viasoft/authorization-management';
import { AuthorizationService } from './tokens/modals/services/authorization/authorization.service';
import { TabsViewTemplateModule } from '@viasoft/view-template';
import {
  VsIconModule,
  VsLabelModule,
  VsButtonModule,
  VsTabGroupModule,
  VsAutocompleteModule,
  VsDialogModule,
  VsFormModule,
  VsSearchInputModule,
  VsLayoutModule, VsInputModule, VsSpinnerModule, VsCheckboxModule
} from '@viasoft/components';

@NgModule({
  declarations: [
    AddNamedUserModalComponent,
  ],
  imports: [
    VsCommonModule,
    PagesRoutingModule,
    VsIconModule,
    VsLabelModule,
    VsButtonModule,
    VsTabGroupModule,
    TabsViewTemplateModule,
    VsAutocompleteModule,
    FormsModule,
    VsDialogModule,
    VsFormModule,
    VsButtonModule,
    VsSearchInputModule,
    VsAuthorizationModule.forRoot(),
    VsLayoutModule,
    VsInputModule,
    VsSpinnerModule,
    VsCheckboxModule
  ],
  providers: [
    AddNamedUserService,
    { provide: AUTHORIZATION_PROVIDER, useClass: AuthorizationService }
  ]
})
export class PagesModule { }
