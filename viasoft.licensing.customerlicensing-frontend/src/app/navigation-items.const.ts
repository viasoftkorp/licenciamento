import { authorizationNavigationConfig } from '@viasoft/authorization-management';
import { IItemMenuOptions } from '@viasoft/navigation';
import {Policies} from "./authorizations/policies.class";

export const SIDENAV_MENU_ITEMS: IItemMenuOptions[] = [
  {
    position: -1,
    label: 'sideNav.licensings',
    icon: 'list-alt',
    path: '/'
  },
  {
    label: 'sideNav.licensing.title',
    icon: 'file-certificate',
    exactMatch: true,
    children: [
      {
        label: 'sideNav.licensing.Details',
        path: '/licensing-info',
      },
      {
        label:  'sideNav.licensing.environment',
        path: '/licensing-info/organizations',
        authorizations: [Policies.ReadEnvironments],
        authorizationType: 'HIDE',
      }
    ]
  },
  {
    label: 'sideNav.settings',
    path: '/settings',
    icon: 'cog',
    exactMatch: true,
    authorizations: [Policies.ReadSettings],
    authorizationType: 'HIDE'
  },
  authorizationNavigationConfig,

];

