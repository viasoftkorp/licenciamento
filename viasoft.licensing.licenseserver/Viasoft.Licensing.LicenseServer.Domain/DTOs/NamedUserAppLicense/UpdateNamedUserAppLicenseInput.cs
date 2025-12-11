using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense
{
    public class UpdateNamedUserAppLicenseInput
    {
        [StrictRequired]
        public Guid NamedUserId { get; set; }
        [StrictRequired]
        public string NamedUserEmail { get; set; }
        public string DeviceId { get; set; }
    }
}