using System;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Account;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class LicensedAccountDetailsByTenant
    {
        public Guid LicensedTenantIdentifier { get; set; }
        public AccountDetails LicensedAccountDetails { get; set; }
    }
}