using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.Services.FileQuota;
using FileTenantQuota = Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota.FileTenantQuota;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers.FileQuota
{
    public class TenantQuotaController: BaseController
    {
        private readonly IFileQuotaCallerService _fileQuotaCallerService;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IFileQuotaViewService _fileQuotaViewService;

        public TenantQuotaController(IFileQuotaCallerService fileQuotaCallerService, ILicenseRepository licenseRepository, IFileQuotaViewService fileQuotaViewService)
        {
            _fileQuotaCallerService = fileQuotaCallerService;
            _licenseRepository = licenseRepository;
            _fileQuotaViewService = fileQuotaViewService;
        }

        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task AddOrUpdateTenantQuota(FileTenantQuotaInput input)
        {
            var licenseTenantIdentifier = await _licenseRepository.GetIdentifierFromLicenseTenantIdExistence(input.LicenseTenantId);
            if (licenseTenantIdentifier == Guid.Empty)
                throw new ArgumentException(nameof(licenseTenantIdentifier));

            var result = await _fileQuotaCallerService.AddOrUpdateFileTenantQuota(licenseTenantIdentifier, input.QuotaLimit);
            if(result == null)
                throw new HttpRequestException("File Provider Tenant Quota Call failed");
            await _fileQuotaViewService.AddOrUpdateTenantQuotaView(input.LicenseTenantId, result.QuotaLimit);
        }

        [HttpGet]
        public async Task<FileTenantQuota> GetTenantQuota(Guid licenseTenantId)
        {
            return await _fileQuotaViewService.GetTenantQuota(licenseTenantId);
        }
    }
}