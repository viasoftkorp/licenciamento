using System;
using Viasoft.Core.DDD.Application.Dto.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings
{
    public class LicensedTenantSettingsOutput : EntityDto
    {
        public Guid TenantId { get; set; }
        public Guid LicensingIdentifier { get; set; }    
        public string Key { get; set;}
        public string Value { get; set; }

        public LicensedTenantSettingsOutput() { }

        public LicensedTenantSettingsOutput(Entities.LicensedTenantSettings licensedTenantSettings)
        {
            Id = licensedTenantSettings.Id;
            TenantId = licensedTenantSettings.TenantId;
            LicensingIdentifier = licensedTenantSettings.LicensingIdentifier;
            Key = licensedTenantSettings.Key;
            Value = licensedTenantSettings.Value;
        }
    }
}