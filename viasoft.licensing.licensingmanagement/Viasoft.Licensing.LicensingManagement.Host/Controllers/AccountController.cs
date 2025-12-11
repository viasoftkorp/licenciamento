using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Command;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Event;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Service;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Validator;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class AccountController : BaseCrudController<Account, AccountCreateOutput, AccountCreateInput, AccountUpdateInput, AccountUpdateOutput, GetAllAccountsInput, AccountDeleteOutput, OperationValidation, string>
    {
        private readonly IRepository<Account> _accounts;
        private readonly IRepository<LicensedTenant> _licensedTenants;
        private readonly IAccountCrudValidator _accountCrudValidator;
        private readonly IMapper _mapper;
        private readonly IServiceBus _serviceBus;
        private readonly IAccountsService _accountsService;
        public AccountController(IMapper mapper, IRepository<Account> repository, IRepository<Account> accounts, IServiceBus serviceBus, IAccountCrudValidator accountCrudValidator, IRepository<LicensedTenant> licensedTenants, IAccountsService accountsService) : base(mapper, repository)
        {
            _mapper = mapper;
            _accounts = accounts;
            _serviceBus = serviceBus;
            _accountCrudValidator = accountCrudValidator;
            _licensedTenants = licensedTenants;
            _accountsService = accountsService;

            Authorizations.Create = Policy.CreateAccount;
            Authorizations.Update = Policy.UpdateAccount;
            Authorizations.Delete = Policy.DeleteAccount;
        }

        protected override (Expression<Func<Account, string>>, bool) DefaultGetAllSorting()
        {
            return (account => account.TradingName, true);
        }

        protected override IQueryable<Account> ApplyCustomFilters(IQueryable<Account> query, GetAllAccountsInput input)
        {
            query = query.WhereIf(input.OnlyActiveAccounts, account => account.Status == AccountStatus.Active);
            return query;
        }

        [HttpPost]
        [Authorize(Policy.CreateAccount)]
        public override async Task<AccountCreateOutput> Create(AccountCreateInput input)
        {
            if (!_accountCrudValidator.ValidateAccountForCreate(input, out var output))
            {
                return output;
            }
            
            return await base.Create(input);
        }

        [HttpPost]
        [Authorize(Policy.UpdateAccount)]
        public override async Task<AccountUpdateOutput> Update(AccountUpdateInput input)
        {
            if (!_accountCrudValidator.ValidateAccountForUpdate(input, out var output))
            {
                return output;
            }
            
            var acc = await _accounts.FindAsync(input.Id);
            _mapper.Map(input, acc);
            var updateAccount = await _accounts.UpdateAsync(acc);
            var accountUpdatedEvent = new AccountUpdatedEvent
            {
                AccountId = acc.Id,
                CompanyName = acc.CompanyName
            };
            await _serviceBus.Publish(accountUpdatedEvent);
            return _mapper.Map<AccountUpdateOutput>(updateAccount);
        }

        [HttpDelete]
        [Authorize(Policy.DeleteAccount)]
        public override async Task<AccountDeleteOutput> Delete(Guid id)
        {
            if (!_accountCrudValidator.ValidateAccountForDelete(id, out var output))
            {
                return await Task.FromResult(output);
            }
            return await base.Delete(id);
        }
        
        [HttpPost]
        [Authorize(Policy.UpdateAccount)]
        public Task UpdateAccountsWithCrmService()
        { 
            return _serviceBus.SendLocal(new PopulateAccountCommand());
        }

        [HttpGet]
        public async Task<AccountInfoOutput> GetAccountInfoFromLicensingIdentifier(Guid licensingIdentifier)
        {
            return await _accountsService.GetAccountInfoFromLicensingIdentifier(licensingIdentifier);
        }
    }
}