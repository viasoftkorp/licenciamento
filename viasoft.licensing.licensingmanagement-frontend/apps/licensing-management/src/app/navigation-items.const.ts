import { authorizationNavigationConfig } from '@viasoft/authorization-management';
import { IItemMenuOptions } from '@viasoft/navigation';

import { Policies } from './tokens/classes/policies.class';

export const NAVIGATION_MENU_ITEMS: IItemMenuOptions[] = [
  {
    position: -1,
    label: 'licensings.licensings',
    icon: 'rectangle-list',
    path: 'licensings',
    exactMatch: true
  },
  {
    position: -1,
    label: 'products.main_title',
    icon: 'bars',
    children: [
      {
        label: 'products.products',
        path: 'products',
        exactMatch: true
      },
      {
        label: 'products.batch_operation_products',
        path: 'products/batch-operations',
        authorizations: [Policies.BatchOperationInsertBundlesInLicenses]
      }
    ]
  },
  {
    position: -1,
    label: 'apps.main_title',
    icon: 'server',
    children: [
      {
        label: 'apps.apps',
        path: 'apps'
      },
      {
        label: 'apps.batch_operation_licenses',
        path: 'apps/batch-operations-licenses',
        authorizations: [Policies.BatchOperationInsertAppsInLicenses]
      },
      {
        label: 'apps.batch_operation_products',
        path: 'apps/batch-operations-products',
        authorizations: [Policies.BatchOperationInsertAppsInBundles]
      }
    ]
  },
  {
    position: -1,
    label: 'softwares.softwares',
    icon: 'boxes-stacked',
    path: 'softwares'
  },
  {
    position: -1,
    label: 'accounts.accounts',
    icon: 'briefcase',
    path: 'accounts'
  },
  {
    position: -1,
    label: 'Auditing.Title',
    icon: 'scale-balanced',
    path: 'auditing'
  },
  authorizationNavigationConfig
];
