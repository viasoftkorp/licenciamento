using System;
using System.Collections.Generic;
using System.Linq;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Repository
{
    public class AccountRepository : IAccountRepository, ITransientDependency
    {
        private readonly IRepository<Entities.Account> _accounts;

        public AccountRepository(IRepository<Entities.Account> accounts)
        {
            _accounts = accounts;
        }

        public Dictionary<Guid, string> GetAccountsNamesFromIdList(List<Guid> ids)
        {
            return _accounts
                .Select(acc => new {acc.Id, acc.CompanyName})
                .Where(acc => ids.Contains(acc.Id))
                .ToDictionary(acc => acc.Id, acc => acc.CompanyName);
        }

        public string GetAccountNameFromId(Guid id)
        {
            return _accounts.FirstOrDefault(acc => acc.Id == id)?.CompanyName;
        }
    }
}