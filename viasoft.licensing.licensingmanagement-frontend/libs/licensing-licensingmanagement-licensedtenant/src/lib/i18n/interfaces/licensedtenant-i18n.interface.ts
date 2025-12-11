import { IKeyTranslate } from '@viasoft/common';

export interface LicensedTenantI18n extends IKeyTranslate {
  LicensedTenant: {
    TenantFilterSelectModal: {
      Title: string,
      Headers: {
        LicensingIdentifier: string,
        AccountName: string
      }
    }
  };
}
