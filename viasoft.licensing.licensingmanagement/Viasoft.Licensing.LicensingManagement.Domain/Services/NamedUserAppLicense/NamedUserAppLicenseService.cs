using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Events;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.NamedUserAppLicense
{
    public class NamedUserAppLicenseService: INamedUserAppLicenseService, ITransientDependency
    {
        private readonly IRepository<Entities.NamedUserAppLicense> _namedUserAppLicenses;
        private readonly ICurrentTenant _currentTenant;
        private readonly IServiceBus _serviceBus;

        public NamedUserAppLicenseService(IRepository<Entities.NamedUserAppLicense> namedUserAppLicenses, ICurrentTenant currentTenant, IServiceBus serviceBus)
        {
            _namedUserAppLicenses = namedUserAppLicenses;
            _currentTenant = currentTenant;
            _serviceBus = serviceBus;
        }

        public async Task<GetNamedUserFromLicensedAppOutput> GetNamedUserFromLicensedApp(
            Entities.LicensedTenant licensedTenant, Entities.LicensedApp licensedApp,
            GetAllNamedUserAppInput input)
        {
            var query = _namedUserAppLicenses
                .Where(a => a.LicensedAppId == licensedApp.Id && a.LicensedTenantId == licensedTenant.Id)
                .Select(a => new NamedUserAppLicenseOutput
                {
                    Id = a.Id,
                    DeviceId = a.DeviceId,
                    TenantId = a.TenantId,
                    LicensedAppId = a.LicensedAppId,
                    LicensedTenantId = a.LicensedTenantId,
                    NamedUserEmail = a.NamedUserEmail,
                    NamedUserId = a.NamedUserId
                });

            if (!string.IsNullOrEmpty(input.Sorting))
                query = query.ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting);
            else
            {
                var (defaultSorting, ascSorting) = DefaultGetAllSorting();
                query = query.ApplyAdvancedFilter(input.AdvancedFilter, defaultSorting, ascSorting);
            }

            var totalCount = await query.CountAsync();

            var result = await AdvancedFilterBasicOperations.PageBy(query, input.SkipCount, input.MaxResultCount).ToListAsync();
            
            return new GetNamedUserFromLicensedAppOutput
            {
                NamedUserAppLicenseOutputs = new PagedResultDto<NamedUserAppLicenseOutput>
                {
                    Items = result,
                    TotalCount = totalCount
                },
                ValidationCode = GetNamedUserFromLicensedAppValidationCode.NoError
            };
        }

        public async Task<AddNamedUserToLicensedAppOutput> AddNamedUserToLicensedApp(Entities.LicensedTenant licensedTenant, Entities.LicensedApp licensedApp,
            AddNamedUserToLicensedAppInput input)
        {
            var namedUsers = await _namedUserAppLicenses
                .Where(a => a.LicensedAppId == licensedApp.Id && a.LicensedTenantId == licensedTenant.Id).CountAsync();

            if (namedUsers >= licensedApp.NumberOfLicenses)
                return new AddNamedUserToLicensedAppOutput
                {
                    ValidationCode = AddNamedUserToLicensedAppValidationCode.TooManyNamedUsers
                };

            var namedUserByEmail = await _namedUserAppLicenses.FirstOrDefaultAsync(a => a.NamedUserEmail == input.NamedUserEmail && a.LicensedAppId == licensedApp.Id && a.LicensedTenantId == licensedTenant.Id);

            if (namedUserByEmail != null)
                return new AddNamedUserToLicensedAppOutput
                {
                    ValidationCode = AddNamedUserToLicensedAppValidationCode.NamedUserEmailAlreadyInUse
                };
            
            var entity = new Entities.NamedUserAppLicense
            {
                Id = Guid.NewGuid(),
                DeviceId = input.DeviceId,
                TenantId = _currentTenant.Id,
                LicensedAppId = licensedApp.Id,
                LicensedTenantId = licensedTenant.Id,
                NamedUserEmail = input.NamedUserEmail,
                NamedUserId = input.NamedUserId
            };

            await _namedUserAppLicenses.InsertAsync(entity);

            return new AddNamedUserToLicensedAppOutput
            {
                ValidationCode = AddNamedUserToLicensedAppValidationCode.NoError
            };
        }

        public async Task<UpdateNamedUsersFromAppOutput> UpdateNamedUsersFromApp(Entities.LicensedTenant licensedTenant,
            Entities.LicensedApp licensedApp,
            UpdateNamedUsersFromAppInput input, Guid namedUserId)
        {
            var entity = await _namedUserAppLicenses.FirstOrDefaultAsync(a => a.Id == namedUserId && a.LicensedAppId == licensedApp.Id && a.LicensedTenantId == licensedTenant.Id);

            if (entity == null)
                return new UpdateNamedUsersFromAppOutput
                {
                    ValidationCode = UpdateNamedUserAppLicenseValidationCode.NoNamedUser
                };

            var namedUserByEmail = await _namedUserAppLicenses.FirstOrDefaultAsync(a =>
                a.NamedUserEmail == input.NamedUserEmail && a.Id != entity.Id && a.LicensedAppId == licensedApp.Id && a.LicensedTenantId == licensedTenant.Id);

            if (namedUserByEmail != null)
                return new UpdateNamedUsersFromAppOutput
                {
                    ValidationCode = UpdateNamedUserAppLicenseValidationCode.NamedUserEmailAlreadyInUse
                };

            entity.DeviceId = input.DeviceId;
            entity.NamedUserEmail = input.NamedUserEmail;
            entity.NamedUserId = input.NamedUserId;

            await _namedUserAppLicenses.UpdateAsync(entity);

            return new UpdateNamedUsersFromAppOutput
            {
                ValidationCode = UpdateNamedUserAppLicenseValidationCode.NoError
            };
        }

        public async Task<DeleteNamedUsersFromAppOutput> DeleteNamedUsersFromApp(Entities.LicensedTenant licensedTenant,
            Entities.LicensedApp licensedApp, Guid namedUserId)
        {
            var namedUser = await _namedUserAppLicenses.FirstOrDefaultAsync(a => a.Id == namedUserId && a.LicensedAppId == licensedApp.Id && a.LicensedTenantId == licensedTenant.Id);

            if (namedUser == null)
                return new DeleteNamedUsersFromAppOutput
                {
                    ValidationCode = DeleteNamedUsersFromAppValidationCode.NoNamedUser
                };

            await _namedUserAppLicenses.DeleteAsync(namedUser);

            var namedUserRemoved = new NamedUserRemoved()
            {
                LicensingIdentifier = licensedTenant.Identifier,
                NamedUserEmail = namedUser.NamedUserEmail
            };

            await _serviceBus.Publish(namedUserRemoved);
            
            return new DeleteNamedUsersFromAppOutput
            {
                ValidationCode = DeleteNamedUsersFromAppValidationCode.NoError
            };
        }

        private static (Expression<Func<NamedUserAppLicenseOutput, string>>, bool) DefaultGetAllSorting()
        {
            return (a => a.NamedUserEmail, true);
        }
    }
}