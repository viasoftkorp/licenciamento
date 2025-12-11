import { NgModule } from '@angular/core';
import { VsCommonModule } from '@viasoft/common';
import { 
  VsButtonModule,
  VsGridModule, 
  VsHeaderModule, 
  VsIconModule, 
  VsLabelModule, 
  VsLayoutModule 
} from '@viasoft/components';
import { TabsViewTemplateModule } from '@viasoft/view-template';
import { LicensingsOnlineUsersGadgetModule } from 'src/app/common/gadgets/licensings-online-users-gadget/licensings-online-users-gadget.module';
import { LicensingsOnlineAppsGadgetModule } from 'src/app/common/gadgets/licensings-online-apps-gadget/licensings-online-apps-gadget.module';
import { IMaskModule } from 'angular-imask';
import { ProductComponent } from './product.component';
import { ProductService } from '../product-grid/product.service';
import { ProductRoutingModule } from './product-routing.module';
import { UsersGridComponent } from './users-grid/users-grid.component';
import { ProgressBarComponent } from './progress-bar/progress-bar.component';
import { LicensingsService } from '../../licensings/licensings.service';
import { StatusCellComponent } from './users-grid/status-cell/status-cell.component';
import { NamedUserService } from './named-user.service';
import { UsersGridService } from './users-grid/users-grid.service';


@NgModule({
  declarations: [
    ProductComponent,
    UsersGridComponent,
    ProgressBarComponent,
    StatusCellComponent,
  ],
  imports: [
    VsCommonModule,
    TabsViewTemplateModule,
    VsGridModule,
    LicensingsOnlineAppsGadgetModule,
    LicensingsOnlineUsersGadgetModule,
    VsLayoutModule,
    VsHeaderModule,
    VsLabelModule,
    VsIconModule,
    IMaskModule,
    ProductRoutingModule,
    VsButtonModule
  ],
  providers: [
    ProductService,
    NamedUserService,
    LicensingsService,
    UsersGridService
  ],
})
export class ProductModule { }




