using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.AmbientData;
using Viasoft.Core.AmbientData.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.LicenseServer
{
    public class LicenseServerServiceUnitTestUtils
    {
        private readonly IServiceProvider _serviceProvider;

        private LicenseServerServiceUnitTestUtils(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            MockLicenseRepository = new Mock<ILicenseRepository>(MockBehavior.Strict);
            LicensedIdentifiers = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            AmbientData = serviceProvider.GetRequiredService<IAmbientData>();
            ExpectedLicenseByIdentifiers = new List<LicenseByIdentifier>
            {
                new LicenseByIdentifier
                {
                    LicenseConsumeType = LicenseConsumeType.Access,
                    Cnpjs = new List<string>(),
                    Identifier = LicensedIdentifiers[0],
                    Status = LicensingStatus.Active,
                    ExpirationDateTime = new DateTime(2080, 5, 2),
                    LicensedTenantDetails = NewTenantDetails(LicensedIdentifiers[0])
                },
                new LicenseByIdentifier
                {
                    LicenseConsumeType = LicenseConsumeType.Connection,
                    Cnpjs = new List<string>(),
                    Identifier = LicensedIdentifiers[1],
                    Status = LicensingStatus.Active,
                    ExpirationDateTime = new DateTime(2080, 5, 2),
                    LicensedTenantDetails = NewTenantDetails(LicensedIdentifiers[1])
                }
            };
            
            ConfigureMock();
        }
        
        public List<Guid> LicensedIdentifiers { get; }
        public Mock<ILicenseRepository> MockLicenseRepository { get; }
        public List<LicenseByIdentifier> ExpectedLicenseByIdentifiers { get; }
        public IAmbientData AmbientData { get; }

        public static async Task<LicenseServerServiceUnitTestUtils> NewUtils(IServiceProvider serviceProvider)
        {
            var utils = new LicenseServerServiceUnitTestUtils(serviceProvider);
            await utils.AddFakeData();

            return utils;
        }

        private void ConfigureMock()
        {
            var licensedTenantDetails = new List<LicensedTenantDetails>();
            foreach (var licensedIdentifier in LicensedIdentifiers)
            {
                var tenantDetails = NewTenantDetails(licensedIdentifier);
                licensedTenantDetails.Add(tenantDetails);
            }
            
            MockLicenseRepository
                .Setup(r => r.GetLicenseDetailsByIdentifiers(LicensedIdentifiers))
                .ReturnsAsync(licensedTenantDetails);
        }

        private async Task AddFakeData()
        {
            var memoryRepository = _serviceProvider.GetRequiredService<IRepository<Domain.Entities.LicensedTenant>>();
            var i = 0;
            foreach (var licensedIdentifier in LicensedIdentifiers)
            {
                var newLicensedTenant = new Domain.Entities.LicensedTenant
                {
                    Identifier = licensedIdentifier,
                    Id = licensedIdentifier,
                    Status = LicensingStatus.Active,
                    ExpirationDateTime = new DateTime(2080, 5, 2),
                    LicenseConsumeType = i % 2 == 0 ? LicenseConsumeType.Access : LicenseConsumeType.Connection,
                    AccountId = licensedIdentifier,
                    TenantId = AmbientData.GetTenantId()
                };
                i++;
                
                await memoryRepository.InsertAsync(newLicensedTenant, true);
            }
        }

        private static LicensedTenantDetails NewTenantDetails(Guid licensedIdentifier)
        {
            var tenantSettings = new LicensedTenantSettingsOutput
            {
                LicensingIdentifier = licensedIdentifier,
                Key = LicensedTenantSettingsKeys.UseSimpleHardwareIdKey,
                Value = bool.FalseString
            };
                
            var tenantDetails = new LicensedTenantDetails(new List<LicensedBundleDetails>(), new List<LicensedAppDetails>(), tenantSettings)
            {
                LicenseIdentifier = licensedIdentifier
            };

            return tenantDetails;
        }
    }
}