using System;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Validator
{
    public interface IAccountCrudValidator
    {
        bool ValidateAccountForCreate(AccountCreateInput input, out AccountCreateOutput output);
        bool ValidateAccountForUpdate(AccountUpdateInput input, out AccountUpdateOutput output);
        bool ValidateAccountForDelete(Guid id, out AccountDeleteOutput output);
    }
}