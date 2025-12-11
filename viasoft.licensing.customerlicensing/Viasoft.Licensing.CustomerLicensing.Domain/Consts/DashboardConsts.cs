﻿namespace Viasoft.Licensing.CustomerLicensing.Domain.Consts
{
    public class DashboardConsts
    {
        public const string DashboardJson =
            "{\"board\":[{\"id\":1,\"title\":\"Prancheta\",\"structure\":\"100\",\"rows\":[{\"columns\":[{\"flex\":\"100\",\"gadgets\":[{\"componentType\":\"LicensingsUsersGadget\",\"name\":\"licensings.dashBoard.onlineUsers.onlineUsersTitle\",\"description\":\"licensings.dashBoard.onlineUsers.onlineUsersDescription\",\"icon\":\"/assets/user-solid.svg\",\"instanceId\":1589915567647,\"permissionName\":\"\",\"tags\":[{\"facet\":\"General\",\"name\":\"OnlineUsersCount\"}],\"config\":{\"propertyPages\":[{\"displayName\":\"Run\",\"groupId\":\"run\",\"properties\":{}}]},\"actions\":[{\"name\":\"licensings.dashBoard.add\"}]},{\"componentType\":\"LicensingsAppsGadget\",\"name\":\"licensings.dashBoard.onlineApps.onlineAppsTitle\",\"description\":\"licensings.dashBoard.onlineApps.onlineAppsDescription\",\"icon\":\"/assets/server-solid.svg\",\"instanceId\":1589915570407,\"permissionName\":\"\",\"tags\":[{\"facet\":\"General\",\"name\":\"OnlineAppsCount\"}],\"config\":{\"propertyPages\":[{\"displayName\":\"Run\",\"groupId\":\"run\",\"properties\":{}}]},\"actions\":[{\"name\":\"licensings.dashBoard.add\"}]}]}]}]}]}";

        public const string ConsumerId = "0C876E34-CE79-48C3-AB36-740E42D2EC8E";
    }
}