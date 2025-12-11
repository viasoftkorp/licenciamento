using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.OrganizationUnit;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class LicensingOrganizationController : BaseController
    {
        private readonly IRepository<LicensedTenant> _licensedTenant;
        private readonly ITenantManagementCaller _tenantManagementCaller;

        public LicensingOrganizationController(IRepository<LicensedTenant> licensedTenant, ITenantManagementCaller tenantManagementCaller)
        {
            _licensedTenant = licensedTenant;
            _tenantManagementCaller = tenantManagementCaller;
        }

        [HttpPost("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-units")]
        public async Task<CreateOrganizationUnitOutput> CreateOrganizationUnit(Guid identifier, CreateOrUpdateOrganizationUnitInput input)
        {
            var doesLicensedTenantExists = await _licensedTenant.AnyAsync(t => t.Id == input.OrganizationId && identifier == t.Identifier);
            if (!doesLicensedTenantExists)
                throw new Exception("No Licensed Tenant found for current organizationId");
            return await _tenantManagementCaller.CreateOrganizationUnit(identifier, input);
        }

        [HttpGet("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-units/{id}")]
        public async Task<OrganizationUnit> GetOrganizationUnit(Guid identifier, Guid id) {
            return await _tenantManagementCaller.GetOrganizationUnit(identifier, id);
        }

        [HttpGet("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-units")]
        public async Task<PagedResultDto<OrganizationUnit>> GetUnitsByOrganization(Guid identifier, [FromQuery] GetByOrganizationInput input) {
            return await _tenantManagementCaller.GetUnitsByOrganization(identifier, input);
        }

        [HttpPut("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-units/{id}")]
        public async Task<UpdateOrganizationUnitOutput> UpdateOrganizationUnit(Guid identifier, Guid id, CreateOrUpdateOrganizationUnitInput input)
        {
            input.Id = id;
            return await _tenantManagementCaller.UpdateOrganizationUnit(identifier, input);
        }

        [HttpPost("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-units/{id}/activate")]
        public async Task ActivateOrganizationUnit(Guid identifier, Guid id) {
            await _tenantManagementCaller.ActivateOrganizationUnit(identifier, id);
        }

        [HttpPost("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-units/{id}/deactivate")]
        public async Task DeactivateOrganizationUnit(Guid identifier, Guid id) {
            await _tenantManagementCaller.DeactivateOrganizationUnit(identifier, id);
        }

        [HttpPost("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-environments")]
        public async Task<CreateEnvironmentOutput> CreateOrganizationEnvironment(Guid identifier, CreateOrUpdateEnvironmentInput input) {
            return await _tenantManagementCaller.CreateOrganizationEnvironment(identifier, input);
        }

        [HttpGet("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-environments/{id}")]
        public async Task<OrganizationUnitEnvironmentOutput> GetOrganizationEnvironment(Guid identifier, Guid id) {
            return await _tenantManagementCaller.GetOrganizationEnvironment(identifier, id);
        }

        [HttpGet("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-units/{unitId}/organization-environments")]
        public async Task<PagedResultDto<OrganizationUnitEnvironment>> GetEnvironmentByUnitId(Guid identifier, Guid unitId, [FromQuery] GetEnvironmentByUnitInput input)
        {
            input.UnitId = unitId;
            return await _tenantManagementCaller.GetEnvironmentByUnitId(identifier, input);
        }

        [HttpPut("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-environments/{id}")]
        public async Task<UpdateEnvironmentOutput> UpdateOrganizationEnvironment(Guid identifier, Guid id, CreateOrUpdateEnvironmentInput input)
        {
            input.Id = id;
            return await _tenantManagementCaller.UpdateOrganizationEnvironment(identifier, input);
        }

        [HttpPost("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-environments/{id}/activate")]
        public async Task ActivateOrganizationEnvironment(Guid identifier, Guid id) {
            await _tenantManagementCaller.ActivateOrganizationEnvironment(identifier, id);
        }

        [HttpPost("/Licensing/LicensingManagement/licensed-tenant/{identifier}/organization-environments/{id}/deactivate")]
        public async Task DeactivateOrganizationEnvironment(Guid identifier, Guid id) {
            await _tenantManagementCaller.DeactivateOrganizationEnvironment(identifier, id);
        }
    }
}