using System;
using System.Threading.Tasks;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Account;

namespace Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.Account
{
    public interface IAccountService
    {
        public Task<AccountInfoOutput> GetAccountNameFromLicensingIdentifier(Guid licensingIdentifier);
    }
}