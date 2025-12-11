using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;
using Xunit;

namespace Viasoft.Licensing.LicenseServer.UnitTest.New.LicenseTenant
{
    public class ReleaseLicensesTest
    {
        private const string AppIdentifier = "LS01";
        private const string AppName = "Licenciamento";
        private const string SoftwareName = "Sistema web";
        private const string SoftwareIdentifier = "WEB";

        private const string Cnpj = "60835783000155";
        private Guid _tenantId = Guid.NewGuid();

        private const string Sid = "3128f510-ddbc-11eb-bd5e-fc4596fac591";

        [Fact(DisplayName = "Tenta liberar licenças com usuários diferentes compartilhando mesmo computador (não é terminal server)")]
        public async Task ReleaseLicenseMultipleUsersSameComputer()
        {
            var app = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(),  null);
            
            var consumeLicenseInput = new ConsumeLicenseInput { Cnpj = Cnpj, User = "User01", AppIdentifier = AppIdentifier, TenantId = _tenantId, CustomAppName = AppName, Token = Sid, IsTerminalServer = false };
            await app.TryConsumeLicense(consumeLicenseInput, true, LicenseConsumeType.Connection);
            
            consumeLicenseInput = new ConsumeLicenseInput { Cnpj = Cnpj, User = "User02", AppIdentifier = AppIdentifier, TenantId = _tenantId, CustomAppName = AppName, Token = Sid, IsTerminalServer = false };
            await app.TryConsumeLicense(consumeLicenseInput, true, LicenseConsumeType.Connection);

            var releasedLicense = app.TryReleaseLicense("User02", true, LicenseConsumeType.Connection, Sid, false);
            
            releasedLicense.ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
            app.AppLicenseConsumers.Count.Should().Be(1);

            var licensedStillConsumed = app.AppLicenseConsumers[Sid];
            licensedStillConsumed.Count.Should().Be(1);
            licensedStillConsumed[0].User.Should().Be("User01");
            licensedStillConsumed[0].AppIdentifier.Should().Be(AppIdentifier);
            licensedStillConsumed[0].TimesUsedByUser.Should().Be(1);
        }

        [Fact(DisplayName = "Tenta liberar licença sem usos prévios")]
        public async Task TestTryReleaseLicense()
        {
            var service = new LicenseTenantStatusApp(1, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(),  null);
            
            var consumeLicenseInput = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                Token = Sid
            };
            
            await service.TryConsumeLicense(consumeLicenseInput, false, LicenseConsumeType.Access);

            var result = service.TryReleaseLicense("User01", false, LicenseConsumeType.Access, Sid, false);
            
            service.AppLicensesConsumed.Should().Be(0);

            result.Cnpj.Should().Be(Cnpj);
            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
        }

        [Fact(DisplayName = "Tenta liberar licença com múltiplos usos de um mesmo Sid")]
        public async Task TestTryReleaseLicencesWithSameSid()
        {
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);
            
            var consumeLicenseInput = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                Token = Sid
            };

            for (int i = 0; i < 2; i++)
            {
                await service.TryConsumeLicense(consumeLicenseInput, false, LicenseConsumeType.Access);
            }

            var outputs = new List<TryReleaseLicenseOutput>();

            for (int i = 0; i < 2; i++)
            {
                outputs.Add(service.TryReleaseLicense("User01", false, LicenseConsumeType.Access, Sid, false));
            }

            service.AppLicensesConsumed.Should().Be(0);

            outputs[0].Cnpj.Should().Be(Cnpj);
            outputs[0].AppName.Should().Be(AppName);
            outputs[0].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[0].SoftwareName.Should().Be(SoftwareName);
            outputs[0].ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
            
            outputs[1].Cnpj.Should().Be(Cnpj);
            outputs[1].AppName.Should().Be(AppName);
            outputs[1].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[1].SoftwareName.Should().Be(SoftwareName);
            outputs[1].ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
        }

        [Fact(DisplayName = "Tenta liberar licença com múltiplos usos de Sids diferentes")]
        public async Task TestTryReleaseLicensesWithDifferentSids()
        {
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);
            
            var consumeLicenseInput = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                Token = Sid
            };

            await service.TryConsumeLicense(consumeLicenseInput, false, LicenseConsumeType.Access);
            await service.TryConsumeLicense(consumeLicenseInput, false, LicenseConsumeType.Access);

            var outputs = new List<TryReleaseLicenseOutput>
            {
                service.TryReleaseLicense("User01", false, LicenseConsumeType.Access, Sid, false),
                service.TryReleaseLicense("User01", false, LicenseConsumeType.Access, Sid, false)
            };

            service.AppLicensesConsumed.Should().Be(0);
            
            outputs[0].Cnpj.Should().Be(Cnpj);
            outputs[0].AppName.Should().Be(AppName);
            outputs[0].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[0].SoftwareName.Should().Be(SoftwareName);
            outputs[0].ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
            
            outputs[1].Cnpj.Should().Be(Cnpj);
            outputs[1].AppName.Should().Be(AppName);
            outputs[1].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[1].SoftwareName.Should().Be(SoftwareName);
            outputs[1].ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
        }

        [Fact(DisplayName = "Tenta liberar licença com múltiplos usos e tipo connection")]
        public async Task TestTryReleaseLicensesWithSameSidsAndConnectionType()
        {
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);
            
            var consumeLicenseInput = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                Token = Sid
            };

            for (int i = 0; i < 2; i++)
            {
                await service.TryConsumeLicense(consumeLicenseInput, false, LicenseConsumeType.Connection);
            }

            var outputs = new List<TryReleaseLicenseOutput>();

            for (int i = 0; i < 2; i++)
            {
                outputs.Add(service.TryReleaseLicense("User01", false, LicenseConsumeType.Connection, Sid, false));
            }
            
            service.AppLicensesConsumed.Should().Be(0);

            outputs[0].Cnpj.Should().Be(Cnpj);
            outputs[0].AppName.Should().Be(AppName);
            outputs[0].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[0].SoftwareName.Should().Be(SoftwareName);
            outputs[0].ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseStillInUseByUser);
            
            outputs[1].Cnpj.Should().Be(Cnpj);
            outputs[1].AppName.Should().Be(AppName);
            outputs[1].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[1].SoftwareName.Should().Be(SoftwareName);
            outputs[1].ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
        }

        [Fact(DisplayName = "Tenta liberar licença com bundled app")]
        public async Task TestTryReleaseLicenseWithBundledApp()
        {
            var service = new LicenseTenantStatusApp(1, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);
            
            var consumeLicenseInput = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                Token = Sid
            };
            
            await service.TryConsumeLicense(consumeLicenseInput, true, LicenseConsumeType.Access);

            var result = service.TryReleaseLicense("User01", true, LicenseConsumeType.Access, Sid, false);
            
            service.AppLicensesConsumed.Should().Be(0);

            result.Cnpj.Should().Be(Cnpj);
            result.AppName.Should().Be(AppName);
            result.SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            result.SoftwareName.Should().Be(SoftwareName);
            result.ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
        }

        [Fact(DisplayName = "Tenta liberar licenças sendo consumida em modo servidor terminal")]
        public async Task TestTryReleaseLicensesWithTerminalServer()
        {
            var service = new LicenseTenantStatusApp(2, AppIdentifier, AppName, LicensedAppStatus.AppActive, SoftwareName, SoftwareIdentifier, 
                new List<NamedUserAppLicenseOutput>(), LicensingModels.Floating, null, 
                new List<NamedUserBundleLicenseOutput>(), null);
            
            var consumeLicenseInput = new ConsumeLicenseInput
            {
                Cnpj = Cnpj,
                User = "User01",
                AppIdentifier = AppIdentifier,
                TenantId = _tenantId,
                CustomAppName = AppName,
                Token = Sid,
                IsTerminalServer = true
            };

            for (int i = 0; i < 2; i++)
            {
                await service.TryConsumeLicense(consumeLicenseInput, false, LicenseConsumeType.Connection);
            }

            var outputs = new List<TryReleaseLicenseOutput>();

            for (int i = 0; i <= 3; i++)
            {
                outputs.Add(service.TryReleaseLicense("User01", false, LicenseConsumeType.Connection, Sid, true));
            }
            
            service.AppLicensesConsumed.Should().Be(0);

            outputs[0].Cnpj.Should().Be(Cnpj);
            outputs[0].AppName.Should().Be(AppName);
            outputs[0].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[0].SoftwareName.Should().Be(SoftwareName);
            outputs[0].ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
            
            outputs[1].Cnpj.Should().Be(Cnpj);
            outputs[1].AppName.Should().Be(AppName);
            outputs[1].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[1].SoftwareName.Should().Be(SoftwareName);
            outputs[1].ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.LicenseReleased);
            
            outputs[2].Cnpj.Should().BeNull();
            outputs[2].AppName.Should().Be(AppName);
            outputs[2].SoftwareIdentifier.Should().Be(SoftwareIdentifier);
            outputs[2].SoftwareName.Should().Be(SoftwareName);
            outputs[2].ReleaseAppLicenseStatus.Should().Be(ReleaseAppLicenseStatus.NoConsumedLicenseToRelease);
        }
    }
}