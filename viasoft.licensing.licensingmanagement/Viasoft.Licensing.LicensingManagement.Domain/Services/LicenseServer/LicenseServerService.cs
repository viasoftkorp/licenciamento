using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicenseServer
{
    public class LicenseServerService: ILicenseServerService, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly ILicenseRepository _licenseRepository;

        public LicenseServerService(IRepository<Entities.LicensedTenant> licensedTenants, ILicenseRepository licenseRepository)
        {
            _licensedTenants = licensedTenants;
            _licenseRepository = licenseRepository;
        }
        
        public async Task<LicenseByIdentifier> GetLicenseByIdentifier(Guid identifier)
        {
            var licensedTenantIdentifier = await _licensedTenants.SingleOrDefaultAsync(tenant => tenant.Identifier == identifier);
        
            if (licensedTenantIdentifier == null)
            {
                throw new ArgumentException($"Could not find a license with the given tenantId {identifier}");
            }
        
            var licenseTenantDetails = await _licenseRepository.GetLicenseDetailsByIdentifier(licensedTenantIdentifier);
            
            return new LicenseByIdentifier
            {
                LicenseConsumeType = licensedTenantIdentifier.LicenseConsumeType,
                Cnpjs = licensedTenantIdentifier.LicensedCnpjList,
                ExpirationDateTime = licensedTenantIdentifier.ExpirationDateTime,
                Identifier = licensedTenantIdentifier.Identifier,
                LicensedTenantDetails = licenseTenantDetails,
                Status = licensedTenantIdentifier.Status,
                HardwareId = licensedTenantIdentifier.HardwareId
            };
        }

        public async Task<List<LicenseByIdentifier>> GetLicensesByIdentifiers(List<Guid> identifiers)
        {
            var output = new List<LicenseByIdentifier>();
            
            var licensedTenantIdentifiers = await _licensedTenants
                .Where(tenant =>  identifiers.Contains(tenant.Identifier))
                .Select(l => new { l.LicensedCnpjList, l.ExpirationDateTime, l.Status, l.Identifier, l.LicenseConsumeType})
                .ToListAsync();
            
            var licenseTenantDetails = await _licenseRepository.GetLicenseDetailsByIdentifiers(identifiers);

            foreach (var licensedTenantIdentifier in licensedTenantIdentifiers)
            {
                var currentTenantDetail = licenseTenantDetails.FirstOrDefault(l => l.LicenseIdentifier == licensedTenantIdentifier.Identifier);
                
                output.Add(new LicenseByIdentifier
                {
                    LicenseConsumeType = licensedTenantIdentifier.LicenseConsumeType,
                    Cnpjs = licensedTenantIdentifier.LicensedCnpjList,
                    ExpirationDateTime = licensedTenantIdentifier.ExpirationDateTime,
                    Identifier = licensedTenantIdentifier.Identifier,
                    LicensedTenantDetails = currentTenantDetail,
                    Status = licensedTenantIdentifier.Status
                });
            }

            return output;
        }
    }
}