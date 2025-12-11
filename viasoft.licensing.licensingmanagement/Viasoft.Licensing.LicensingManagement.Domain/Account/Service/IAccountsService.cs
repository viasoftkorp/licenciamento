using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Service
{
    public interface IAccountsService
    { 
        bool CheckIfCnpjIsAlreadyRegistered(string cnpjCpf, Guid id);
        Task<string> GetAccountCompanyName(Guid id);
        Task<AccountInfoOutput> GetAccountInfoFromLicensingIdentifier(Guid licensingIdentifier);
    }
}