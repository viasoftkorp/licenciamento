using System;
using System.Collections.Generic;
using System.Linq;
using Viasoft.Core.DDD.Application.Dto.BaseCrud;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Validator
{
    public class AccountCrudValidator : IAccountCrudValidator, ITransientDependency
    {
        private readonly IRepository<Entities.Account> _accounts;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;

        public AccountCrudValidator(IRepository<Entities.Account> accounts, ILicenseRepository licenseRepository, IRepository<Entities.LicensedTenant> licensedTenants)
        {
            _accounts = accounts;
            _licenseRepository = licenseRepository;
            _licensedTenants = licensedTenants;
        }

        public bool ValidateAccountForCreate(AccountCreateInput input, out AccountCreateOutput output)
        {
            output = null;
            if (_accounts.Any(acc => acc.CnpjCpf == input.CnpjCpf && acc.Id != input.Id))
            {
                var account = _accounts.First(acc => acc.CnpjCpf == input.CnpjCpf);
                output = new AccountCreateOutput
                {
                    OperationValidation = OperationValidation.CnpjAlreadyRegistered,
                    Id = account.Id,
                    CompanyName = account.CompanyName
                };
            }

            return output == null;
        }

        public bool ValidateAccountForUpdate(AccountUpdateInput input, out AccountUpdateOutput output)
        {
            output = null;
            if (_accounts.Any(acc => acc.CnpjCpf == input.CnpjCpf && acc.Id != input.Id))
            {
                var account = _accounts.First(acc => acc.CnpjCpf == input.CnpjCpf);
                output = new AccountUpdateOutput
                {
                    OperationValidation = OperationValidation.CnpjAlreadyRegistered,
                    Id = account.Id,
                    CompanyName = account.CompanyName
                };
            }

            return output == null;
        }

        public bool ValidateAccountForDelete(Guid id, out AccountDeleteOutput output)
        {
            output = null;
            if (_licenseRepository.IsAccountBeingUsedByLicense(id))
            {
                var license = _licensedTenants.First(l => l.AccountId == id);
                output = new AccountDeleteOutput
                {
                    Success = false,
                    Errors = new List<BaseCrudResponseError<OperationValidation>>
                    {
                        new BaseCrudResponseError<OperationValidation>
                        {
                            Message = license.Identifier.ToString(),
                            ErrorCode = OperationValidation.UsedByOtherRegister
                        }
                    }
                };
            }

            return output == null;
        }
    }
}