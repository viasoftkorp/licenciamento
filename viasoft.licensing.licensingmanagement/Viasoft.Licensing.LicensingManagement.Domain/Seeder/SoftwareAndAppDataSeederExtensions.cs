using System;
using System.Collections.Generic;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Seeder
{
    public static class SoftwareAndAppDataSeederExtensions
    {
        public static List<App> GetDefaultApps(Guid defaultWebSoftware)
        {
            return new List<App>
            {
                new App 
                {
                    Id = Guid.NewGuid(),
                    Name = "Administração",
                    Identifier = "ADM01",
                    Default = true,
                    CreationTime = DateTime.UtcNow,
                    IsActive = true,
                    SoftwareId = defaultWebSoftware
                },
                new App
                {
                    Id =  Guid.NewGuid(),
                    Name = "Gerenciador de Licenças",
                    Identifier = "LS01",
                    Default = true,
                    CreationTime = DateTime.UtcNow,
                    IsActive = true,
                    SoftwareId = defaultWebSoftware
                },
                new App
                {
                    Id = Guid.NewGuid(),
                    Name = "Monitor de Licenças",
                    Identifier = "LS02",
                    Default = true,
                    CreationTime = DateTime.UtcNow,
                    IsActive = true,
                    SoftwareId = defaultWebSoftware
                }
            };
        }
    }
}