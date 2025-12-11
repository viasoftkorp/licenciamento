using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Viasoft.Core.DDD.Entities;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensingLicenses;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.OwnedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensingLicenses;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.LicensingLicenses
{
    public class ProvideLicensingComponentsTest: LicensingManagementTestBase
    {
        private static readonly ILogger<ProvideLicensingLicensesService> Logger = new NullLogger<ProvideLicensingLicensesService>();

        private readonly Guid _licensedTenantId = Guid.Parse("8248bf70-d849-11eb-9ecb-fc4596fac591");
        private readonly Guid _licensedTenantIdentifier = Guid.Parse("8b808500-d849-11eb-bad3-fc4596fac591");
        private readonly Guid _accountId = Guid.Parse("36634071-859E-44CB-A673-11AF5E06DD69");
        private readonly Guid _appId = Guid.Parse("976B0B4A-5D2C-46B2-8FCE-4D381FDF577F");
        private readonly Guid _licensedAppId = Guid.Parse("836B0B4A-5D2C-46B2-8FCE-4D381FDF577F");
        private readonly Guid _bundleId = Guid.Parse("42AAD82C-07E7-4FC6-B5E5-97417992C7FC");
        private readonly Guid _softwareId = Guid.Parse("C01F0AD9-6933-4F4A-B1F0-D7AD45BA06C3");
        private readonly Guid _licensedBundleId = Guid.Parse("CBADB111-0B12-47E4-B2CA-B5FCB9BADD90");
        private readonly Guid _licensedTenantSettingsId = TestUtils.Guids[0];
        private readonly List<Guid> _namedUserBundleLicenses = new()
        {
            Guid.Parse("1C178B53-54CB-4F78-A9F1-E78A0B5957D4"),
            Guid.Parse("F2EA7980-1BB1-489F-A6DE-7FC858FF2F22"),
            Guid.Parse("A1028933-8485-481D-9C83-F59435FC80BA")
        };
        private readonly List<Guid> _namedUserAppLicenses = new()
        {
            Guid.Parse("5A0E1237-BA9B-4139-8C3F-22C64E62AC42"),
            Guid.Parse("DB96B699-F68D-4788-8F2C-212408871FC2"),
            Guid.Parse("20359493-0AE8-4EF7-A48A-55D597A9D956")
        };
        private readonly List<Guid> _namedUserIds = new()
        {
            Guid.Parse("A48505D9-1D31-40A0-99F7-7EF2C6799015"),
            Guid.Parse("F2E19235-F890-4144-B68C-3A347BA31175"),
            Guid.Parse("AAD182AA-7A94-42D0-96BC-E840E06593CD"),
            Guid.Parse("66FFEEC5-EF54-49EB-BBC5-19ACE2EE449E"),
            Guid.Parse("60C43375-F381-41CC-9651-871E36A86097"),
            Guid.Parse("01AA12BC-B20D-49B9-A31A-6EC3C3FA9D85")
        };
        private readonly List<string> _namedUserEmails = new()
        {
          "namedUserEmail01",
          "namedUserEmail02",
          "namedUserEmail03",
          "namedUserEmail04",
          "namedUserEmail05",
          "namedUserEmail06"
        };


        private const string AppIdentifier = "identifier";
        private const string AppName = "app";
        private const string SoftwareIdentifier = "software";
        private const string SoftwareName = "soft";
        private const string DeviceId = "device";

        [Fact(DisplayName = "Testar fluxo completo com sucesso")]
        public async Task TestSuccessfulFlow()
        {
            var licensedTenantRepo = await GetRepo<Domain.Entities.LicensedTenant>(new List<object>
            {
                new Domain.Entities.LicensedTenant
                {
                    Id = _licensedTenantId,
                    Identifier = _licensedTenantIdentifier,
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = _accountId,
                    AdministratorEmail = "myrtie11@gmail.com",
                    HardwareId = "Lorem ipsum",
                    TenantId = _licensedTenantIdentifier,
                    LicensedCnpjs = "08016137000146",
                    LicenseConsumeType = LicenseConsumeType.Access,
                    ExpirationDateTime = null
                }
            });
            var accountsRepo = await GetRepo<Domain.Entities.Account>(new List<object>
            {
                new Domain.Entities.Account
                {
                    City = "Cidade",
                    Country = "Brazil",
                    Detail = "Detalhe",
                    Email = "myrtie11@gmail.com",
                    Id = _accountId,
                    Neighborhood = "Boqueirão",
                    Number = "Numéro",
                    Phone = "4155667788",
                    State = "Parana",
                    Status = AccountStatus.Active,
                    Street = "Rua Torres",
                    BillingEmail = "myrtie11@gmail.com",
                    CnpjCpf = "08016137000146",
                    CompanyName = "Empresa de sorvete",
                    ZipCode = "04223",
                    TenantId = _licensedTenantIdentifier,
                    TradingName = "Trading"
                }
            });
            var licensedAppRepo = await GetRepo<LicensedApp>(new List<object>
            {
                new LicensedApp
                {
                    Id = _licensedAppId,
                    Status = LicensedAppStatus.AppActive,
                    AppId = _appId,
                    TenantId = _licensedTenantIdentifier,
                    LicensedBundleId = _bundleId,
                    LicensedTenantId = _licensedTenantId,
                    NumberOfLicenses = 10,
                    NumberOfTemporaryLicenses = 15,
                    ExpirationDateOfTemporaryLicenses = null,
                    AdditionalNumberOfLicenses = 5
                }
            });
            var licensedTenantSettingsRepo = await GetRepo<LicensedTenantSettings>(new List<object>
            {
               new LicensedTenantSettings
               {
                   Id = _licensedTenantSettingsId,
                   Key = LicensedTenantSettingsKeys.UseSimpleHardwareIdKey,
                   Value = TestUtils.Names[0],
                   TenantId = _licensedTenantIdentifier,
                   LicensingIdentifier = _licensedTenantIdentifier
               } 
            });
            var bundles = await GetRepo<Bundle>(new List<object>
            {
                new Bundle
                {
                    Id = _bundleId,
                    Identifier = "bundle",
                    Name = "bundleName",
                    IsActive = true,
                    IsCustom = false,
                    TenantId = _licensedTenantIdentifier,
                    SoftwareId = _softwareId
                }
            });
            var licensedBundles = await GetRepo<Domain.Entities.LicensedBundle>(new List<object>
            {
                new Domain.Entities.LicensedBundle
                {
                    Id = _licensedBundleId,
                    Status = LicensedBundleStatus.BundleActive,
                    BundleId = _bundleId,
                    TenantId = _licensedTenantIdentifier,
                    ExpirationDateOfTemporaryLicenses = null,
                    NumberOfLicenses = 10,
                    NumberOfTemporaryLicenses = 5,
                    LicensedTenantId = _licensedTenantId
                },
                new Domain.Entities.LicensedBundle
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedBundleStatus.BundleActive,
                    BundleId = _bundleId,
                    TenantId = _licensedTenantIdentifier,
                    ExpirationDateOfTemporaryLicenses = null,
                    NumberOfLicenses = 10,
                    NumberOfTemporaryLicenses = 5,
                    LicensedTenantId = Guid.NewGuid()
                }
            });
            var software = await GetRepo<Software>(new List<object>
            {
                new Software
                {
                    Id = _softwareId,
                    Company = "Company",
                    Identifier = SoftwareIdentifier,
                    Name = SoftwareName,
                    TenantId = _licensedTenantIdentifier
                }
            });
            var app = await GetRepo<App>(new List<object>
            {
                new App
                {
                    Id = _appId,
                    TenantId = _licensedTenantIdentifier,
                    Default = false,
                    Domain = Domain.Enums.Domain.Accounting,
                    Identifier = AppIdentifier,
                    Name = AppName,
                    SoftwareId = _softwareId,
                    IsActive = true
                }
            });
            var listOfNamedUserAppLicenses = new List<object>
            {
                new NamedUserAppLicense
                {
                    Id = _namedUserAppLicenses[0],
                    DeviceId = DeviceId,
                    TenantId = _licensedTenantIdentifier,
                    NamedUserId = _namedUserIds[0],
                    NamedUserEmail = _namedUserEmails[0],
                    LicensedTenantId = _licensedTenantId,
                    LicensedAppId = _licensedAppId
                },
                new NamedUserAppLicense
                {
                    Id = _namedUserAppLicenses[1],
                    DeviceId = DeviceId,
                    TenantId = _licensedTenantIdentifier,
                    NamedUserId = _namedUserIds[1],
                    NamedUserEmail = _namedUserEmails[1],
                    LicensedTenantId = _licensedTenantId,
                    LicensedAppId = _licensedAppId
                },
                new NamedUserAppLicense
                {
                    Id = _namedUserAppLicenses[2],
                    DeviceId = DeviceId,
                    TenantId = _licensedTenantIdentifier,
                    NamedUserId = _namedUserIds[2],
                    NamedUserEmail = _namedUserEmails[2],
                    LicensedTenantId = _licensedTenantId,
                    LicensedAppId = _licensedAppId
                }
            };
            var listOfNamedUserBundleLicenses = new List<object>
            {
                new NamedUserBundleLicense
                {
                    Id = _namedUserBundleLicenses[0],
                    DeviceId = DeviceId,
                    TenantId = _licensedTenantIdentifier,
                    NamedUserId = _namedUserIds[3],
                    NamedUserEmail = _namedUserEmails[3],
                    LicensedTenantId = _licensedTenantId,
                    LicensedBundleId = _licensedBundleId
                },
                new NamedUserBundleLicense
                {
                    Id = _namedUserBundleLicenses[1],
                    DeviceId = DeviceId,
                    TenantId = _licensedTenantIdentifier,
                    NamedUserId = _namedUserIds[4],
                    NamedUserEmail = _namedUserEmails[4],
                    LicensedTenantId = _licensedTenantId,
                    LicensedBundleId = _licensedBundleId
                },
                new NamedUserBundleLicense
                {
                    Id = _namedUserBundleLicenses[2],
                    DeviceId = DeviceId,
                    TenantId = _licensedTenantIdentifier,
                    NamedUserId = _namedUserIds[5],
                    NamedUserEmail = _namedUserEmails[5],
                    LicensedTenantId = _licensedTenantId,
                    LicensedBundleId = _licensedBundleId
                }
            };
            var namedUserAppLicense = await GetRepo<NamedUserAppLicense>(listOfNamedUserAppLicenses);
            var namedUserBundleLicense = await GetRepo<NamedUserBundleLicense>(listOfNamedUserBundleLicenses); 
            
            var service = new ProvideLicensingLicensesService(licensedTenantRepo, accountsRepo, licensedAppRepo, licensedBundles, 
                bundles, software, app, namedUserAppLicense, namedUserBundleLicense, licensedTenantSettingsRepo, null, Logger);

            var result = await service.GetLicensingLicenses(_licensedTenantIdentifier);

            result.LicensedTenantSettings.Should().BeEquivalentTo(new LicensedTenantSettingsOutput
            {
                Id = licensedTenantSettingsRepo.First().Id,
                TenantId = licensedTenantSettingsRepo.First().TenantId,
                LicensingIdentifier = licensedTenantSettingsRepo.First().LicensingIdentifier,
                Key = licensedTenantSettingsRepo.First().Key,
                Value = licensedTenantSettingsRepo.First().Value
            });

            result.AccountDetails.Should().BeEquivalentTo(new AccountOutput
            {
                City = "Cidade",
                Country = "Brazil",
                Detail = "Detalhe",
                Email = "myrtie11@gmail.com",
                Neighborhood = "Boqueirão",
                Number = "Numéro",
                Phone = "4155667788",
                State = "Parana",
                Status = AccountStatus.Active,
                Street = "Rua Torres",
                BillingEmail = "myrtie11@gmail.com",
                CnpjCpf = "08016137000146",
                CompanyName = "Empresa de sorvete",
                ZipCode = "04223",
                TenantId = _licensedTenantIdentifier,
                TradingName = "Trading"
            });
            
            result.LicensedTenant.Should().BeEquivalentTo(new LicensedTenantOutput
            {
                Id = _licensedTenantId,
                Identifier = _licensedTenantIdentifier,
                Notes = "",
                Status = LicensingStatus.Active,
                AccountId = _accountId,
                AdministratorEmail = "myrtie11@gmail.com",
                HardwareId = "Lorem ipsum",
                LicensedCnpjs = "08016137000146",
                LicenseConsumeType = LicenseConsumeType.Access,
                ExpirationDateTime = null,
                AccountName = "Empresa de sorvete"
            });

            result.OwnedApps.Should().BeEquivalentTo(new List<LicensedAppOutput>()
            {
                new LicensedAppOutput
                {
                    Status = LicensedAppStatus.AppActive,
                    AppId = _appId,
                    TenantId = _licensedTenantIdentifier,
                    LicensedBundleId = _bundleId,
                    LicensedTenantId = _licensedTenantId,
                    NumberOfLicenses = 10,
                    NumberOfTemporaryLicenses = 15,
                    ExpirationDateOfTemporaryLicenses = null,
                    AdditionalNumberOfLicenses = 5,
                    Identifier = AppIdentifier,
                    Name = AppName,
                    SoftwareIdentifier = SoftwareIdentifier,
                    SoftwareName = SoftwareName
                }
            });

            result.OwnedBundles.Should().BeEquivalentTo(new List<OwnedBundleOutput>
            {
                new()
                {
                    BundleId = _bundleId,
                    Identifier = "bundle",
                    Name = "bundleName",
                    IsActive = true,
                    IsCustom = false,
                    SoftwareId = _softwareId,
                    ExpirationDateOfTemporaryLicenses = null,
                    NumberOfLicenses = 10,
                    NumberOfTemporaryLicenses = 5,
                    SoftwareName = SoftwareName,
                    LicensedBundleId = _licensedBundleId
                }
            });

            result.NamedUserAppLicenses.Should().BeEquivalentTo(listOfNamedUserAppLicenses.Select(a =>
            {
                var appLicense = (NamedUserAppLicense) a;
                return new NamedUserAppLicenseOutput
                {
                    Id = appLicense.Id,
                    DeviceId = appLicense.DeviceId,
                    TenantId = appLicense.TenantId,
                    LicensedAppId = appLicense.LicensedAppId,
                    LicensedTenantId = appLicense.LicensedTenantId,
                    NamedUserEmail = appLicense.NamedUserEmail,
                    NamedUserId = appLicense.NamedUserId
                };
            }).ToList());
            result.NamedUserBundleLicenses.Should().BeEquivalentTo(listOfNamedUserBundleLicenses.Select(b =>
            {
                var bundleLicense = (NamedUserBundleLicense) b;
                return new NamedUserBundleLicenseOutput()
                {
                    Id = bundleLicense.Id,
                    DeviceId = bundleLicense.DeviceId,
                    OperationValidation = OperationValidation.NoError,
                    TenantId = bundleLicense.TenantId,
                    LicensedBundleId = bundleLicense.LicensedBundleId,
                    LicensedTenantId = bundleLicense.LicensedTenantId,
                    NamedUserEmail = bundleLicense.NamedUserEmail,
                    NamedUserId = bundleLicense.NamedUserId
                };
            }).ToList());
        }

        [Fact(DisplayName = "Testar fluxo sem tenant")]
        public async Task TestFlowWithoutTenant()
        {
            var tenantRepo = await GetRepo<Domain.Entities.LicensedTenant>(new List<object>());

            var accountRepo = await GetRepo<Domain.Entities.Account>(new List<object>());
            
            var licensedAppRepo = await GetRepo<LicensedApp>(new List<object>());

            var licensedBundleRepo = await GetRepo<Domain.Entities.LicensedBundle>(new List<object>());
            
            var licensedTenantSettingsRepo = await GetRepo<LicensedTenantSettings>(new List<object>());

            var bundleRepo = await GetRepo<Bundle>(new List<object>());

            var softwareRepo = await GetRepo<Software>(new List<object>());

            var appRepo = await GetRepo<App>(new List<object>());

            var namedUserAppLicenses = await GetRepo<NamedUserAppLicense>(new List<object>());

            var namedUserBundleLicenses = await GetRepo<NamedUserBundleLicense>(new List<object>());

            var service = new ProvideLicensingLicensesService(tenantRepo, accountRepo, licensedAppRepo, licensedBundleRepo, 
                bundleRepo, softwareRepo, appRepo, namedUserAppLicenses, namedUserBundleLicenses, licensedTenantSettingsRepo, null, Logger);

            var result = await service.GetLicensingLicenses(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact(DisplayName = "Testar UpdatehardwareId com fluxo perfeito", Skip = "A atualização do hardwareId passou a ocorrer em LicensedTenantService")]
        public async Task UpdateHardwareIdWithPerfectFlow()
        {
            var repo = await GetRepo<Domain.Entities.LicensedTenant>(new List<object>
            {
                new Domain.Entities.LicensedTenant
                {
                    Id = _licensedTenantId,
                    Identifier = _licensedTenantIdentifier,
                    Notes = Encoding.UTF8.GetBytes("Teste"),
                    Status = LicensingStatus.Active,
                    AccountId = _accountId,
                    AdministratorEmail = "jeremy67@yahoo.com",
                    HardwareId = string.Empty,
                    LicensedCnpjs = "02720673000141",
                    TenantId = _licensedTenantIdentifier,
                    LicenseConsumeType = LicenseConsumeType.Connection,
                    ExpirationDateTime = null
                }
            });
            
            var service = new ProvideLicensingLicensesService(repo, null, null, null, null, 
                null, null, null, null, null, null, Logger);
        
            var result = await service.UpdateHardwareId(_licensedTenantIdentifier, new UpdateHardwareIdInput
            {
                HardwareId = "HardwareId"
            });
        
            result.IsSuccess.Should().BeTrue();
            result.Code.Should().Be(UpdateHardwareIdEnum.Success);
        }

        [Fact(DisplayName = "Testar UpdateHardwareId com erro de não encontrar entidade")]
        public async Task TestUpdateHardwareIdWithEntityNotFoundError()
        {
            var repo = await GetRepo<Domain.Entities.LicensedTenant>(new List<object>());
            
            var service = new ProvideLicensingLicensesService(repo, null, null, null, null, 
                null, null, null, null, null, null, Logger);

            var result = await service.UpdateHardwareId(_licensedTenantIdentifier, new UpdateHardwareIdInput
            {
                HardwareId = "Teste"
            });

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(UpdateHardwareIdEnum.CouldNotFindEntity);
        }

        [Fact(DisplayName = "Testar UpdateHardwareId com erro de não encontrar hardwareId input")]
        public async Task TestUpdateHardwareIdWithoutHardwareIdInput()
        {
            var service = new ProvideLicensingLicensesService(null, null, null, null, 
                null, null, null, null, null, null, null, Logger);
            
            var result = await service.UpdateHardwareId(_licensedTenantIdentifier, new UpdateHardwareIdInput
            {
                HardwareId = string.Empty
            });

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(UpdateHardwareIdEnum.InputHardwareIdEmpty);
        }

        [Fact(DisplayName = "Testar UpdateHardwareId com erro de hardwareId já preenchido")]
        public async Task TestUpdateHardwareIdWithHardwareIdAlreadySet()
        {
            var repo = await GetRepo<Domain.Entities.LicensedTenant>(new List<object>
            {
                new Domain.Entities.LicensedTenant
                {
                    Id = _licensedTenantId,
                    Identifier = _licensedTenantIdentifier,
                    Notes = Encoding.UTF8.GetBytes("Teste"),
                    Status = LicensingStatus.Active,
                    AccountId = _accountId,
                    AdministratorEmail = "electa_metz26@yahoo.com",
                    HardwareId = "TestePreenchido",
                    LicensedCnpjs = "46846472000160",
                    TenantId = _licensedTenantId,
                    ExpirationDateTime = null,
                    LicenseConsumeType = LicenseConsumeType.Connection
                }
            });

            var service = new ProvideLicensingLicensesService(repo, null, null, null, null, 
                null, null, null, null, null, null, Logger);

            var result = await service.UpdateHardwareId(_licensedTenantIdentifier, new UpdateHardwareIdInput
            {
                HardwareId = "Teste",
            });

            result.IsSuccess.Should().BeFalse();
            result.Code.Should().Be(UpdateHardwareIdEnum.EntityHardwareIdNotEmpty);
        }

        private async Task<IRepository<T>> GetRepo<T>(List<object> entities) where T : Entity
        {
            var repo = ServiceProvider.GetService<IRepository<T>>();

            foreach (var entity in entities)
            {
                await repo.InsertAsync((T) entity, true);
            }

            return repo;
        }
    }
}