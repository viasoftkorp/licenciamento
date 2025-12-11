import { NgModule } from '@angular/core';
import { VsCommonModule } from '@viasoft/common';
import { LicensingsComponent } from './licensings.component';
import { TabsViewTemplateModule } from '@viasoft/view-template';
import { LicensingsRouting } from './licensings-routing.module';
import { VsInputModule, VsButtonModule, VsFormModule, VsTreeTableModule, VsGridModule, VsTabGroupModule, VsLayoutModule } from '@viasoft/components';
import { pt } from './i18n/pt';
import { en } from './i18n/en';
import { LicensingsService } from './licensings.service';
import { LicensingsTreeTableComponent } from './licensings-tree-table/licensings-tree-table.component';
import { LicensingsTreeTableService } from './licensings-tree-table/licensings-tree-table.service';
import { CustomFilterService } from './licensings-tree-table/custom-filters.service';
import { LicensingsGridComponent } from './licensings-grid/licensings-grid.component';
import { LicensingsDashboardComponent } from './licensings-dashboard/licensings-dashboard.component';
import { VsDashboardModule, VsGadgetModule, VsDashboardService, VsDashboardApiService } from '@viasoft/dashboard';
import { GADGETS } from 'src/app/common/gadgets/gadgets';
import {
  LicensingsOnlineAppsGadgetModule
} from 'src/app/common/gadgets/licensings-online-apps-gadget/licensings-online-apps-gadget.module';
import { LicensingsOnlineUsersGadgetModule } from 'src/app/common/gadgets/licensings-online-users-gadget/licensings-online-users-gadget.module';
import { LicensingsOnlineUsersGadgetComponent } from 'src/app/common/gadgets/licensings-online-users-gadget/licensings-online-users-gadget.component';
import { LicensingsOnlineAppsGadgetComponent } from 'src/app/common/gadgets/licensings-online-apps-gadget/licensings-online-apps-gadget.component';
import { LicensingsGridService } from './licensings-grid/licensings-grid.service';
import { LicensingsDashboardService } from './licensings-dashboard/licensings-dashboard.service';

@NgModule({
  declarations: [LicensingsComponent, LicensingsGridComponent, LicensingsDashboardComponent, LicensingsTreeTableComponent],
  imports: [
    VsCommonModule.forChild({
      translates: {
        pt, en
      }
    }),
    TabsViewTemplateModule,
    LicensingsRouting,
    VsInputModule,
    VsButtonModule,
    VsGridModule,
    VsTabGroupModule,
    VsTreeTableModule,
    LicensingsOnlineAppsGadgetModule,
    LicensingsOnlineUsersGadgetModule,
    VsFormModule,
    VsLayoutModule,
    VsDashboardModule.forChild({
      gadgetsDataSource: [
          ...GADGETS
      ]
    }),
    VsGadgetModule.config({
      gadgetComponentDefinition: {
        LicensingsUsersGadget: LicensingsOnlineUsersGadgetComponent,
        LicensingsAppsGadget: LicensingsOnlineAppsGadgetComponent
      }
    })
  ],
  providers: [
    LicensingsService,
    LicensingsTreeTableService,
    CustomFilterService,
    {
      provide: VsDashboardService, useClass: LicensingsDashboardService
    },
    LicensingsGridService,
    VsDashboardApiService
  ]
})
export class LicensingsModule { }
