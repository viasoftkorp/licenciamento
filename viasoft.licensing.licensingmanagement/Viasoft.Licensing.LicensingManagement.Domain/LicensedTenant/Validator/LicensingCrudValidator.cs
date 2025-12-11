using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DateTimeProvider;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Validator
{
    public class LicensingCrudValidator : ILicensingCrudValidator, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly IRepository<Entities.Account> _accounts;
        private readonly IRepository<Bundle> _bundles;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public LicensingCrudValidator(IRepository<Entities.LicensedTenant> licensedTenants, IMapper mapper, IRepository<Entities.Account> accounts, IRepository<Bundle> bundles, IDateTimeProvider dateTimeProvider)
        {
            _licensedTenants = licensedTenants;
            _mapper = mapper;
            _accounts = accounts;
            _bundles = bundles;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<(bool isValid, LicenseTenantCreateOutput output)> ValidateLicensingForCreate(LicenseTenantCreateInput input)
        {
            var output = _mapper.Map<LicenseTenantCreateOutput>(input);
            
            var duplicatedIndentifier = await _licensedTenants.AnyAsync(l => l.Identifier == input.Identifier);

            if (duplicatedIndentifier)
            {
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
                return (false, output);
            }

            var administratorEmailAlreadyInUse = await _licensedTenants.AnyAsync(l => l.AdministratorEmail == input.AdministratorEmail);

            if (administratorEmailAlreadyInUse)
            {
                output.OperationValidation = OperationValidation.AdministrationEmailAlreadyInUse;
                return (false, output);
            }
            
            var doesAccountExist = await _accounts.AnyAsync(a => a.Id == input.AccountId);

            if (!doesAccountExist)
            {
                output.OperationValidation = OperationValidation.NoAccountWithSuchId;
                return (false, output);
            }
            
            var accountIdInUse = await _licensedTenants
                .Select(l => new {l.Identifier , l.AccountId})
                .FirstOrDefaultAsync(l => l.AccountId == input.AccountId);

            if (accountIdInUse != null)
            {
                output.Identifier = accountIdInUse.Identifier;
                output.OperationValidation = OperationValidation.AccountIdAlreadyInUse;
                return (false, output);
            }

            if (input.BundleIds != null && input.BundleIds.Count > 0)
            {
                if (input.NumberOfLicenses < 0)
                {
                    output.OperationValidation = OperationValidation.InvalidNumberOfLicenses;
                    return (false, output);
                }
                
                var bundlesCount = await _bundles.Select(b => b.Id)
                    .Where(id => input.BundleIds.Contains(id))
                    .CountAsync();
                if (bundlesCount != input.BundleIds.Count)
                {
                    output.OperationValidation = OperationValidation.BundleDoesNotExist;
                    return (false, output);
                }
                
                
            }
            
            return (true, null);
        }
        public async Task<(bool isValid, LicenseTenantUpdateOutput output)> ValidateLicensingForUpdate(LicenseTenantUpdateInput input)
        {
            LicenseTenantUpdateOutput output = null;
            
            var currentDate = _dateTimeProvider.UtcNow().Date;
            if (input.ExpirationDateTime.HasValue && input.ExpirationDateTime.Value.Date < currentDate)
            {
                output = _mapper.Map<LicenseTenantUpdateOutput>(input);
                output.OperationValidation = OperationValidation.InvalidDate;
            }
            
            if (await _licensedTenants.AnyAsync(l => l.Identifier == input.Identifier && l.Id != input.Id))
            {
                output = _mapper.Map<LicenseTenantUpdateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
            }

            var licensedTenantWithInvalidAccountId = await _licensedTenants
                .Select(l => new {l.Identifier, l.AccountId, l.Id})
                .FirstOrDefaultAsync(l => l.AccountId == input.AccountId && l.Id != input.Id);
            if (licensedTenantWithInvalidAccountId != null)
            {
                output = _mapper.Map<LicenseTenantUpdateOutput>(input);
                output.Identifier = licensedTenantWithInvalidAccountId.Identifier;
                output.OperationValidation = OperationValidation.AccountIdAlreadyInUse;
            }
            
            var licensedTenantWithInvalidEmail = await _licensedTenants
                .Select(l => new {l.Identifier, l.AdministratorEmail, l.Id})
                .FirstOrDefaultAsync(l => l.AdministratorEmail == input.AdministratorEmail && l.Id != input.Id);
            if (licensedTenantWithInvalidEmail != null)
            {
                output = _mapper.Map<LicenseTenantUpdateOutput>(input);
                output.Identifier = licensedTenantWithInvalidEmail.Identifier;
                output.OperationValidation = OperationValidation.AdministrationEmailAlreadyInUse;
            }
            return (output == null, output);
        }
    }
}