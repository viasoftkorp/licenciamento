using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense
{
    public class AddNamedUserToLicensedAppInput
    {
        [StrictRequired]
        public Guid NamedUserId { get; set; }
        [StrictRequired]
        public string NamedUserEmail { get; set; }
        public string DeviceId { get; set; }
    }
}