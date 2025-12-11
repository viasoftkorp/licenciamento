using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.TenantInfo;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantInfo
{
    public class TenantInfoService : ITenantInfoService, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;

        public TenantInfoService(IRepository<Entities.LicensedTenant> licensedTenants)
        {
            _licensedTenants = licensedTenants;
        }

        public async Task<TenantInfoOutput> GetTenantInfoFromLicensingIdentifier(Guid identifier)
        {
            var tenant = await _licensedTenants
                .Select(l => new
                {
                    l.Identifier,
                    l.LicensedCnpjs,
                    l.Status,
                    l.Id
                })
                .FirstOrDefaultAsync(l => l.Identifier == identifier);
            if (tenant != null)
            {
                return new TenantInfoOutput
                {
                    LicensedTenantId = tenant.Id,
                    Cnpj = tenant.LicensedCnpjs,
                    LicensingStatus = tenant.Status,
                    OperationValidation = OperationValidation.NoError
                };
            }
            return new TenantInfoOutput
            {
                OperationValidation = OperationValidation.NoTenantWithSuchId
            };
        }
    }
}