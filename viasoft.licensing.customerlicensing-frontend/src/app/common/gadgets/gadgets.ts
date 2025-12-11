import { VsGadgetDataSource } from '@viasoft/dashboard';
import { environment } from 'src/environments/environment';

export const GADGETS: VsGadgetDataSource[] = [
  {
    componentType: 'LicensingsUsersGadget',
    name: 'Licensings.DashBoard.OnlineUsers.OnlineUsersTitle',
    description: 'Licensings.DashBoard.OnlineUsers.OnlineUsersDescription',
    icon: `${environment.assetsUrl}user-solid.svg`,
    instanceId: -1,
    permissionName: '',
    tags: [{ facet: 'General', name: 'Licensings.DashBoard.OnlineUsers.OnlineUsersTitle' }],
    config: {
      propertyPages: [
        {
          displayName: 'Run',
          groupId: 'run',
          properties: {},
        },
      ],
    },
    actions: [
      {
        name: 'Licensings.DashBoard.Add',
      },
    ],
  },
  {
    componentType: 'LicensingsAppsGadget',
    name: 'Licensings.DashBoard.OnlineApps.OnlineAppsTitle',
    description: 'Licensings.DashBoard.OnlineApps.OnlineAppsDescription',
    icon: `${environment.assetsUrl}server-solid.svg`,
    instanceId: -1,
    permissionName: '',
    tags: [{ facet: 'General', name: 'Licensings.DashBoard.OnlineApps.OnlineAppsTitle' }],
    config: {
      propertyPages: [
        {
          displayName: 'Run',
          groupId: 'run',
          properties: {},
        },
      ],
    },
    actions: [
      {
        name: 'Licensings.DashBoard.Add',
      },
    ],
  },
];
