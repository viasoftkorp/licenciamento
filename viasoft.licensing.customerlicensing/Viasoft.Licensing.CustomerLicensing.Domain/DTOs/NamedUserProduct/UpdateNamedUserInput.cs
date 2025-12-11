using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle
{
    public class UpdateNamedUserInput
    {
        [StrictRequired]
        public Guid NamedUserId { get; set; }
        [StrictRequired]
        public string NamedUserEmail { get; set; }
        public string DeviceId { get; set; }
    }
}