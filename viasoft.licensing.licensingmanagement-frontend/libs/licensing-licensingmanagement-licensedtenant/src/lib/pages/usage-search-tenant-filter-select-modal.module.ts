import { NgModule } from '@angular/core';
import { VsGridModule } from '@viasoft/components/grid';
import { VsIconModule } from '@viasoft/components/icon';
import { VsCommonModule } from '@viasoft/common';
import { LICENSED_TENANT_I18N_EN } from '../i18n/consts/licensedtenant-i18n-en.const';
import { LICENSED_TENANT_I18N_PT } from '../i18n/consts/licensedtenant-i18n-pt.const';
import { UsageSearchTenantFilterSelectModalService } from '../services/select-modal/usage-search-tenant-filter-select-modal.service';
import { LicensedTenantViewService } from '../services/view/licensedtenant-view.service';
import { VsSelectModalModule } from '@viasoft/components/select-modal';
import { ApiModule } from '../clients/license-management/api.module';
import { BASE_PATH as USAGE_SEARCH_TENANT_FILTER_PATH } from '../clients/license-management/variables';
import { VS_BACKEND_URL } from '@viasoft/client-core';
import { Configuration } from '../clients/license-management/configuration';

@NgModule({
    imports: [
        VsCommonModule.forChild({
            translates: {
                pt: LICENSED_TENANT_I18N_PT,
                en: LICENSED_TENANT_I18N_EN
            }
        }),
        VsGridModule,
        VsIconModule,
        VsSelectModalModule,
        ApiModule
    ],
    providers: [
        UsageSearchTenantFilterSelectModalService,
        LicensedTenantViewService,
        {
            provide: USAGE_SEARCH_TENANT_FILTER_PATH,
            useExisting: VS_BACKEND_URL
        }
    ]
})
export class UsageSearchTenantFilterSelectModalModule { }

export { LICENSED_TENANT_I18N_EN } from '../i18n/consts/licensedtenant-i18n-en.const';
export { LICENSED_TENANT_I18N_PT } from '../i18n/consts/licensedtenant-i18n-pt.const';
export { USAGE_SEARCH_TENANT_FILTER_PATH, ApiModule, Configuration };