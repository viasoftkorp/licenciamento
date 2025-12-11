using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Service
{
    public class AccountsService : IAccountsService, ITransientDependency
    {
        private readonly IRepository<Entities.Account> _accounts;
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        
        public AccountsService(IRepository<Entities.Account> accounts, IRepository<Entities.LicensedTenant> licensedTenants)
        {
            _accounts = accounts;
            _licensedTenants = licensedTenants;
        }

        public bool CheckIfCnpjIsAlreadyRegistered(string cnpjCpf, Guid id)
        {
            return _accounts.Any(acc => acc.CnpjCpf == cnpjCpf && acc.Id != id);
        }

        public async Task<string> GetAccountCompanyName(Guid id)
        {
            return await _accounts.Where(a => a.Id == id).Select(a => a.CompanyName).FirstOrDefaultAsync();
        }

        public async Task<AccountInfoOutput> GetAccountInfoFromLicensingIdentifier(Guid licensingIdentifier)
        {
            var tenant = await _licensedTenants.FirstOrDefaultAsync(t => t.Identifier == licensingIdentifier);
            if (tenant == null)
            {
                return null;
            }
            var account = await _accounts.FirstAsync(a => a.Id == tenant.AccountId);
            return new AccountInfoOutput
            {
                AccountId = account.Id,
                AccountName = account.CompanyName
            };
        }
    }
}