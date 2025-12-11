import { NgModule } from '@angular/core';
import { AccountsComponent } from './accounts.component';
import { VsCommonModule } from '@viasoft/common';
import { AccountsRoutingModule } from './accounts-routing.module';
import { VsButtonModule, VsGridModule, VsDialogModule, VsInputModule, VsSelectModule, VsIconModule, VsLabelModule, VsLayoutModule, VsFormModule } from '@viasoft/components';
import { TabsViewTemplateModule } from '@viasoft/view-template';
import { AccountsGridComponent } from './accounts-grid/accounts-grid.component';
import { AccountsService } from './accounts.service';
import { AccountsDetailComponent } from './accounts-detail/accounts-detail.component';
import { AccountsFormControlService } from './accounts-form-control.service';
import { AccountsCnpjValidationService } from './accounts-cnpj-validation.service';
import { AccountsZipCodeValidationService } from './accounts-zip-code-validation.service';

@NgModule({
    declarations: [AccountsComponent, AccountsGridComponent, AccountsDetailComponent],
    imports: [
        VsCommonModule.forChild(),
        AccountsRoutingModule,
        VsButtonModule,
        TabsViewTemplateModule,
        VsGridModule,
        VsDialogModule,
        VsInputModule,
        VsSelectModule,
        VsIconModule,
        VsLabelModule,
        VsLayoutModule,
        VsFormModule
    ],
    exports: [AccountsComponent, AccountsGridComponent],
    providers: [AccountsService,
        AccountsFormControlService,
        AccountsCnpjValidationService,
        AccountsZipCodeValidationService
    ]
})
export class AccountsModule { }
