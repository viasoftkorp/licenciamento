using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;
using Xunit;

namespace Viasoft.Licensing.LicenseServer.UnitTest.New.LicenseTenant
{
    public class ConsumeLicensesTest
    {
        private const string AppIdentifier = "LS01";
        private const string AppName = "Licenciamento";
        private const string SoftwareName = "Sistema web";
        private const string SoftwareIdentifier = "WEB";

        private const string Cnpj = "60835783000155";
        private Guid _tenantId = Guid.NewGuid();

        private const string Sid = "3128f510-ddbc-11eb-bd5e-fc4596fac591";

        [Fact(DisplayName = "Tenta consumir licença com um usuário usando ela pela primeira vez")]
        public async Task TestTryConsumeLicenseNoPreviousUse()
        {
            var service = new LicenseTenantStatusApp(1, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);

            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                Token = Sid
            };
            
            var result = await service.TryConsumeLicense(input, false, LicenseConsumeType.Access);

            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            result.LicenseUsageStartTime.Should().NotBeNull();

            service.AppLicensesConsumed.Should().Be(1);
        }

        [Fact(DisplayName = "Tenta consumir licença com o mesmo usuário e sid múltiplas vezes")]
        public async Task TestTryConsumeLicenseWithPreviousUse()
        {
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);

            var inputs = new List<ConsumeLicenseInput>();

            for (int i = 0; i < 3; i++)
            {
                inputs.Add(new ConsumeLicenseInput
                {
                    Cnpj = Cnpj,
                    User = "User01",
                    AppIdentifier = AppIdentifier,
                    TenantId = _tenantId,
                    CustomAppName = AppName,
                    Token = Sid
                });
            }

            var outputs = new List<TryConsumeLicenseOutput>();

            foreach (var input in inputs)
            {
                outputs.Add(await service.TryConsumeLicense(input, false, LicenseConsumeType.Access));
            }

            service.AppLicensesConsumed.Should().Be(2);
            
            outputs[0].AppName.Should().Be(AppName);
            outputs[0].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[0].SoftwareName.Should().Be(SoftwareName);
            outputs[0].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            outputs[0].LicenseUsageStartTime.Should().NotBeNull();
            
            outputs[1].AppName.Should().Be(AppName);
            outputs[1].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[1].SoftwareName.Should().Be(SoftwareName);
            outputs[1].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            outputs[1].LicenseUsageStartTime.Should().NotBeNull();
            
            outputs[2].AppName.Should().Be(AppName);
            outputs[2].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[2].SoftwareName.Should().Be(SoftwareName);
            outputs[2].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.NotEnoughLicenses);
            outputs[2].LicenseUsageStartTime.Should().BeNull();
        }
        
                [Fact(DisplayName = "Tenta consumir licença com o mesmo usuário e sids diferentes usando múltiplas vezes")]
        public async Task TestTryConsumeLicenseWithPreviousUseAndDifferentSids()
        {
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);

            var inputs = new List<ConsumeLicenseInput>();

            for (int i = 0; i < 3; i++)
            {
                inputs.Add(new ConsumeLicenseInput
                {
                    Cnpj = Cnpj,
                    User = "User01",
                    AppIdentifier = AppIdentifier,
                    TenantId = _tenantId,
                    CustomAppName = AppName,
                    Token = Sid
                });
            }

            var outputs = new List<TryConsumeLicenseOutput>();
            
            outputs.Add(await service.TryConsumeLicense(inputs[0], false, LicenseConsumeType.Access));
            outputs.Add(await service.TryConsumeLicense(inputs[1], false, LicenseConsumeType.Access));
            outputs.Add(await service.TryConsumeLicense(inputs[2], false, LicenseConsumeType.Access));
            
            service.AppLicensesConsumed.Should().Be(2);
            
            outputs[0].AppName.Should().Be(AppName);
            outputs[0].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[0].SoftwareName.Should().Be(SoftwareName);
            outputs[0].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            outputs[0].LicenseUsageStartTime.Should().NotBeNull();
            
            outputs[1].AppName.Should().Be(AppName);
            outputs[1].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[1].SoftwareName.Should().Be(SoftwareName);
            outputs[1].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            outputs[1].LicenseUsageStartTime.Should().NotBeNull();
            
            outputs[2].AppName.Should().Be(AppName);
            outputs[2].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[2].SoftwareName.Should().Be(SoftwareName);
            outputs[2].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.NotEnoughLicenses);
            outputs[2].LicenseUsageStartTime.Should().BeNull();
        }

        [Fact(DisplayName = "Tenta consumir licença com o app bloqueado")]
        public async Task TestTryConsumeLicenseWithBlockedApp()
        {
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppBlocked, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);

            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                Token = Sid
            };

            var output = await service.TryConsumeLicense(input, false, LicenseConsumeType.Access);
            output.AppName.Should().Be(AppName);
            output.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            output.SoftwareName.Should().Be(SoftwareName);
            output.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.AppBlocked);
            output.LicenseUsageStartTime.Should().BeNull();
        }

        [Fact(DisplayName = "Tenta consumir licença com o mesmo usuário múltiplas vezes e o tipo connection")]
        public async Task TestTryConsumeLicenseWithSameUserMultipleTimesAndConnectionType()
        {
            var service = new LicenseTenantStatusApp(1, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);
            
            var inputs = new List<ConsumeLicenseInput>();

            for (int i = 0; i < 3; i++)
            {
                inputs.Add(new ConsumeLicenseInput
                {
                    Cnpj = Cnpj,
                    User = "User01",
                    AppIdentifier = AppIdentifier,
                    TenantId = _tenantId,
                    CustomAppName = AppName,
                    Token = Sid
                });
            }

            var outputs = new List<TryConsumeLicenseOutput>();

            foreach (var input in inputs)
            {
                outputs.Add(await service.TryConsumeLicense(input, false, LicenseConsumeType.Connection));
            }

            service.AppLicensesConsumed.Should().Be(1);
            
            outputs[0].AppName.Should().Be(AppName);
            outputs[0].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[0].SoftwareName.Should().Be(SoftwareName);
            outputs[0].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            outputs[0].LicenseUsageStartTime.Should().NotBeNull();
            
            outputs[1].AppName.Should().Be(AppName);
            outputs[1].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[1].SoftwareName.Should().Be(SoftwareName);
            outputs[1].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseAlreadyInUseByUser);
            outputs[1].LicenseUsageStartTime.Should().NotBeNull();
            
            outputs[2].AppName.Should().Be(AppName);
            outputs[2].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[2].SoftwareName.Should().Be(SoftwareName);
            outputs[2].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseAlreadyInUseByUser);
            outputs[2].LicenseUsageStartTime.Should().NotBeNull();
        }

        [Fact(DisplayName = "Tenta consumir licença com bundled app")]
        public async Task TestTryConsumeLicenseWithBundledApp()
        {
            var service = new LicenseTenantStatusApp(1, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);

            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                Token = Sid
            };
            
            var result = await service.TryConsumeLicense(input, true, LicenseConsumeType.Access);

            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            result.LicenseUsageStartTime.Should().NotBeNull();
        }

        [Fact(DisplayName = "Tenta consumirLicença já consumida com servidor terminal")]
        public async Task TestTryConsumeLicenseWithTerminalServer()
        {
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);

            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = Sid,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = true
            };
            
            var outputs = new List<TryConsumeLicenseOutput>();

            for (int i = 0; i <= 3; i ++)
            {
                outputs.Add(await service.TryConsumeLicense(input, false, LicenseConsumeType.Connection));
            }
            
            outputs[0].AppName.Should().Be(AppName);
            outputs[0].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[0].SoftwareName.Should().Be(SoftwareName);
            outputs[0].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            outputs[0].LicenseUsageStartTime.Should().NotBeNull();
            
            outputs[1].AppName.Should().Be(AppName);
            outputs[1].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[1].SoftwareName.Should().Be(SoftwareName);
            outputs[1].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            outputs[1].LicenseUsageStartTime.Should().NotBeNull();
            
            outputs[2].AppName.Should().Be(AppName);
            outputs[2].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[2].SoftwareName.Should().Be(SoftwareName);
            outputs[2].ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.NotEnoughLicenses);
            outputs[2].LicenseUsageStartTime.Should().BeNull();
        }

        [Fact(DisplayName = "Tenta consumir licença nomeada de app com um usuário usando ela pela primeira vez")]
        public async Task TryToConsumeNamedAppLicenseForFirstTime()
        {
            var namedUserAppLicenses = new List<NamedUserAppLicenseOutput>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantId,
                    DeviceId = "DeviceId",
                    LicensedAppId = Guid.NewGuid(),
                    LicensedTenantId = Guid.NewGuid(),
                    NamedUserEmail = "presley89@gmail.com",
                    NamedUserId = Guid.NewGuid()
                }
            };
            var service = new LicenseTenantStatusApp(1, AppIdentifier, AppName, LicensedAppStatus.AppActive,
                SoftwareName, SoftwareIdentifier, namedUserAppLicenses, LicensingModels.Named, LicensingModes.Online,
                new List<NamedUserBundleLicenseOutput>(), null);

            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = Sid,
                User = "presley89@gmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false
            };

            var result = await service.TryConsumeLicense(input, false, LicenseConsumeType.Access);

            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
        }

        [Fact(DisplayName = "Tenta consumir licença nomeada de app e falha por usuário não estar associado a tal licença")]
        public async Task TryToConsumeNamedAppLicenseButFailDueToNotAvailable()
        {
            var namedUserAppLicenses = new List<NamedUserAppLicenseOutput>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantId,
                    DeviceId = "DeviceId",
                    LicensedAppId = Guid.NewGuid(),
                    LicensedTenantId = Guid.NewGuid(),
                    NamedUserEmail = "claire_brakus77@hotmail.com",
                    NamedUserId = Guid.NewGuid()
                }
            };
            var service = new LicenseTenantStatusApp(1, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, namedUserAppLicenses, 
                LicensingModels.Named, LicensingModes.Online, new List<NamedUserBundleLicenseOutput>(), null);

            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = Sid,
                User = "presley89@gmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false
            };

            var result = await service.TryConsumeLicense(input, false, LicenseConsumeType.Access);

            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.NamedAppLicenseNotAvailable);
        }

        [Fact(DisplayName = "Tenta consumir licença nomeada de pacote e falha por usuário não estar associado a tal licença")]
        public async Task TryToConsumeNamedBundleLicenseButFailsDueToNotAvailable()
        {
            var namedUserBundleLicenses = new List<NamedUserBundleLicenseOutput>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantId,
                    DeviceId = "DeviceId",
                    LicensedBundleId = Guid.NewGuid(),
                    LicensedTenantId = Guid.NewGuid(),
                    NamedUserEmail = "claire_brakus77@hotmail.com",
                    NamedUserId = Guid.NewGuid()
                }
            };
            var service = new LicenseTenantStatusApp(1, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Named, LicensingModes.Online, namedUserBundleLicenses, null);

            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = Sid,
                User = "presley89@gmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false
            };

            var result = await service.TryConsumeLicense(input, true, LicenseConsumeType.Access);

            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.NamedBundleLicenseNotAvailable);
        }

        [Fact(DisplayName = "Tenta consumir a licença nomeada de app no modo online, mas falha por estar consumindo com SID diferente")]
        public async Task TryToConsumeNamedAppLicenseButFailsDueToConsumingWithDifferentSidAndOnlineMode()
        {
            var namedUserBundleLicenses = new List<NamedUserAppLicenseOutput>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantId,
                    DeviceId = "DeviceId",
                    LicensedAppId = Guid.NewGuid(),
                    LicensedTenantId = Guid.NewGuid(),
                    NamedUserEmail = "claire_brakus77@hotmail.com",
                    NamedUserId = Guid.NewGuid()
                }
            };
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, namedUserBundleLicenses, 
                LicensingModels.Named, LicensingModes.Online, new List<NamedUserBundleLicenseOutput>(), null);

            var input01 = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = Sid,
                User = "claire_brakus77@hotmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false
            };
            var input02 = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = "652ADB45-8C4D-4E1D-AFF5-5710C18F756A",
                User = "claire_brakus77@hotmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false
            };

            var result01 = await service.TryConsumeLicense(input01, false, LicenseConsumeType.Access);
            result01.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
            
            var result02 = await service.TryConsumeLicense(input02, false, LicenseConsumeType.Access);
            result02.AppName.Should().Be(AppName);
            result02.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result02.SoftwareName.Should().Be(SoftwareName);
            result02.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.CantConsumeOnlineNamedLicenseInDifferentComputers);
        }

        [Fact(DisplayName = "Tenta consumir licenças nomeadas de app com usuários diferentes")]
        public async Task TryToConsumeNamedAppLicensesMultipleTimesWithDifferentUsers()
        {
            var namedUserBundleLicenses = new List<NamedUserAppLicenseOutput>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantId,
                    DeviceId = "DeviceId",
                    LicensedAppId = Guid.NewGuid(),
                    LicensedTenantId = Guid.NewGuid(),
                    NamedUserEmail = "claire_brakus77@hotmail.com",
                    NamedUserId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantId,
                    DeviceId = "DeviceId",
                    LicensedAppId = Guid.NewGuid(),
                    LicensedTenantId = Guid.NewGuid(),
                    NamedUserEmail = "lazaro35@gmail.com",
                    NamedUserId = Guid.NewGuid()
                }
            };
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, namedUserBundleLicenses, 
                LicensingModels.Named, LicensingModes.Online, new List<NamedUserBundleLicenseOutput>(), null);

            var input01 = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = Sid,
                User = "claire_brakus77@hotmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false
            };
            var input02 = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = "652ADB45-8C4D-4E1D-AFF5-5710C18F756A",
                User = "lazaro35@gmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false
            };

            var result01 = await service.TryConsumeLicense(input01, false, LicenseConsumeType.Access);

            result01.AppName.Should().Be(AppName);
            result01.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result01.SoftwareName.Should().Be(SoftwareName);
            result01.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);

            var result02 = await service.TryConsumeLicense(input02, false, LicenseConsumeType.Access);
            result02.AppName.Should().Be(AppName);
            result02.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result02.SoftwareName.Should().Be(SoftwareName);
            result02.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.LicenseConsumed);
        }

        [Fact(DisplayName = "Tenta consumir licença nomeada offline de app, mas falha pelo fato dos identificadores de dispositivo serem diferentes")]
        public async Task TryToConsumeOfflineNamedAppLicenseButFailDueToInvalidDeviceId()
        {
            var namedUserBundleLicenses = new List<NamedUserAppLicenseOutput>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantId,
                    DeviceId = "DeviceId",
                    LicensedAppId = Guid.NewGuid(),
                    LicensedTenantId = Guid.NewGuid(),
                    NamedUserEmail = "claire_brakus77@hotmail.com",
                    NamedUserId = Guid.NewGuid()
                }
            };

            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive,
                SoftwareName, SoftwareIdentifier, namedUserBundleLicenses, LicensingModels.Named, LicensingModes.Offline,
                new List<NamedUserBundleLicenseOutput>(), null);
            
            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = Sid,
                User = "claire_brakus77@hotmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false,
                DeviceId = Guid.NewGuid().ToString()
            };

            var result = await service.TryConsumeLicense(input, false, LicenseConsumeType.Access);

            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.NamedUserLicenseDeviceIdDoesNotMatch);
        }

        [Fact(DisplayName = "Tenta consumir a licença nomeada offline de app, mas o identificador do dispositivo está vazio")]
        public async Task TryToConsumeOfflineNameduserAppLicenseWithEmptyHardwareIdButFailsRequestWithNoNamedUser()
        {
            var namedUserId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var licensedAppId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var namedUserBundleLicenses = new List<NamedUserAppLicenseOutput>
            {
                new()
                {
                    Id = id,
                    TenantId = _tenantId,
                    DeviceId = string.Empty,
                    LicensedAppId = licensedAppId,
                    LicensedTenantId = licensedTenantId,
                    NamedUserEmail = "claire_brakus77@hotmail.com",
                    NamedUserId = namedUserId
                }
            };
            
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, namedUserBundleLicenses, 
                LicensingModels.Named, LicensingModes.Offline, new List<NamedUserBundleLicenseOutput>(), null);
            
            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = "FD393466-EBA9-42F2-9B07-24EED243417B",
                User = "claire_brakus77@hotmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false
            };

            var result = await service.TryConsumeLicense(input, false, LicenseConsumeType.Access);

            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.EmptyDeviceId);
        }

        [Fact(DisplayName = "Tenta consumir a licença nomeada offline de app, mas o identificador do dispositivo está vazio e a requisição falha ao encontrar o licensedTenant")]
        public async Task TryToConsumeOfflineNameduserAppLicenseWithEmptyHardwareIdButFailsRequestWithNoLicensedTenant()
        {
            var namedUserId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var licensedAppId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var namedUserBundleLicenses = new List<NamedUserAppLicenseOutput>
            {
                new()
                {
                    Id = id,
                    TenantId = _tenantId,
                    DeviceId = string.Empty,
                    LicensedAppId = licensedAppId,
                    LicensedTenantId = licensedTenantId,
                    NamedUserEmail = "claire_brakus77@hotmail.com",
                    NamedUserId = namedUserId
                }
            };
            
            var licenseServerServiceMock = new Mock<IExternalLicensingManagementService>();

            licenseServerServiceMock.Setup(x => x.UpdateNamedUserApp(It.IsAny<UpdateNamedUserAppLicenseInput>(), It.IsAny<Guid>(), 
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new UpdateNamedUserAppLicenseOutput
            {
                ValidationCode = UpdateNamedUserAppLicenseValidationCode.NoLicensedTenant
            });
            
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, namedUserBundleLicenses, 
                LicensingModels.Named, LicensingModes.Offline, new List<NamedUserBundleLicenseOutput>(), licenseServerServiceMock.Object);
            
            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = "FD393466-EBA9-42F2-9B07-24EED243417B",
                User = "claire_brakus77@hotmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false,
                DeviceId = Guid.NewGuid().ToString()
            };

            var result = await service.TryConsumeLicense(input, false, LicenseConsumeType.Access);

            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.CouldNotUpdateNamedUserAppLicenseDueToNoLicensedTenant);
            
            licenseServerServiceMock.Invocations.Count.Should().Be(1);
        }

        [Fact(DisplayName = "Tenta consumir a licença nomeada offline de app, mas o identificador do dispositivo está vazio e a requisição falha ao encontrar o licensedTenant")]
        public async Task TryToConsumeOfflineNameduserAppLicenseWithEmptyHardwareIdButFailsRequestWithNoLicensedApp()
        {
            var namedUserId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var licensedAppId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var namedUserBundleLicenses = new List<NamedUserAppLicenseOutput>
            {
                new()
                {
                    Id = id,
                    TenantId = _tenantId,
                    DeviceId = string.Empty,
                    LicensedAppId = licensedAppId,
                    LicensedTenantId = licensedTenantId,
                    NamedUserEmail = "claire_brakus77@hotmail.com",
                    NamedUserId = namedUserId,
                }
            };

            var licenseServerServiceMock = new Mock<IExternalLicensingManagementService>();

            licenseServerServiceMock.Setup(x => x.UpdateNamedUserApp(It.IsAny<UpdateNamedUserAppLicenseInput>(), It.IsAny<Guid>(), 
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new UpdateNamedUserAppLicenseOutput
            {
                ValidationCode = UpdateNamedUserAppLicenseValidationCode.NoLicensedApp
            });
            
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, namedUserBundleLicenses, 
                LicensingModels.Named, LicensingModes.Offline, new List<NamedUserBundleLicenseOutput>(), licenseServerServiceMock.Object);
            
            var input = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                Token = "FD393466-EBA9-42F2-9B07-24EED243417B",
                User = "claire_brakus77@hotmail.com",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                IsTerminalServer = false,
                DeviceId = Guid.NewGuid().ToString()
            };

            var result = await service.TryConsumeLicense(input, false, LicenseConsumeType.Access);

            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatus.CouldNotUpdateNamedUserAppLicenseDueToNoLicensedApp);

            licenseServerServiceMock.Invocations.Count.Should().Be(1);
        }
        
    }
}