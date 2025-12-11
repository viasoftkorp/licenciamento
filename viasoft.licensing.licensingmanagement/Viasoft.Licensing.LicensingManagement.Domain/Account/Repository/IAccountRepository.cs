using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Repository
{
    public interface IAccountRepository
    {
        Dictionary<Guid, string> GetAccountsNamesFromIdList(List<Guid> ids);

        string GetAccountNameFromId(Guid id);
    }
}