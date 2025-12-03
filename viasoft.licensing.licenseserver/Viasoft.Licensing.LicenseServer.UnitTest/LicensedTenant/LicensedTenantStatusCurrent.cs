using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Implementations;
using Xunit;

namespace Viasoft.Licensing.LicenseServer.UnitTest.LicensedTenant
{
    public class LicensedTenantStatusCurrent
    {
        [Fact, Category("TenantStatus")]
        public void Test_Bundles_And_Apps_From_Status_Current()
        {
            // prepare
            var licensedByTenantIdInput = CreateLicenseByTenantIdWithTemporaryLicenses();
            var looseApp = licensedByTenantIdInput.LicensedTenantDetails.OwnedApps[0];
            var ownedBundle = licensedByTenantIdInput.LicensedTenantDetails.OwnedBundles[0];
            var appsInOwnedBundle = licensedByTenantIdInput.LicensedTenantDetails.OwnedApps
                .Where(a => a.LicensedBundleId == ownedBundle.BundleId).ToList();
            var expectedLooseAppStatus = new LicenseTenantStatusApp(looseApp.NumberOfLicenses, looseApp.AdditionalNumberOfLicenses, looseApp.Identifier, looseApp.Name,
                looseApp.Status, looseApp.SoftwareName, looseApp.SoftwareIdentifier);
            var expectedBundleStatus = new LicenseTenantStatusUsedBundle(ownedBundle, appsInOwnedBundle);
            var licensedTenantStatusCurrent = new LicenseTenantStatusCurrentOld(licensedByTenantIdInput, new ProvideHardwareIdService());
            // execute
            var resultBundles = licensedTenantStatusCurrent.Bundles;
            var resultApps = licensedTenantStatusCurrent.LooseApps;
            var resultTenantDetails = licensedTenantStatusCurrent.TenantDetails;
            // test
            resultTenantDetails.Should().BeEquivalentTo(licensedByTenantIdInput);
            Assert.True(resultBundles.ContainsKey(ownedBundle.Identifier));
            Assert.True(resultApps.ContainsKey(looseApp.Identifier));
            Assert.Single(resultBundles);
            Assert.Single(resultApps);
            var looseAppStatus = resultApps[looseApp.Identifier];
            var bundleStatus = resultBundles[ownedBundle.Identifier];
            looseAppStatus.Should().BeEquivalentTo(expectedLooseAppStatus);
            bundleStatus.Should().BeEquivalentTo(expectedBundleStatus);
        }
        
        [Fact, Category("TenantStatus")]
        public void Test_Bundles_And_Apps_From_Status_Current_When_Have_Account()
        {
            // prepare
            var licensedByTenantIdInput = CreateLicenseByTenantIdWithTemporaryLicenses();
            var looseApp = licensedByTenantIdInput.LicensedTenantDetails.OwnedApps[0];
            var ownedBundle = licensedByTenantIdInput.LicensedTenantDetails.OwnedBundles[0];
            var appsInOwnedBundle = licensedByTenantIdInput.LicensedTenantDetails.OwnedApps
                .Where(a => a.LicensedBundleId == ownedBundle.BundleId).ToList();
            var expectedLooseAppStatus = new LicenseTenantStatusApp(looseApp.NumberOfLicenses, looseApp.AdditionalNumberOfLicenses, looseApp.Identifier, looseApp.Name,
                looseApp.Status, looseApp.SoftwareName, looseApp.SoftwareIdentifier);
            var expectedBundleStatus = new LicenseTenantStatusUsedBundle(ownedBundle, appsInOwnedBundle);
            var accountDetails = new LicensedAccountDetails
            {
                Email = "teste@teste.com.br",
                Phone = "4635355432333",
                Status = LicensedAccountStatusEnum.Active,
                CnpjCpf = "04276545689",
                CompanyName = "Korp do Junior",
                TradingName = "teste",
                WebSite = "korp.junior.com.br"
            };
            licensedByTenantIdInput.LicensedTenantDetails.AccountDetails = accountDetails;
            var licensedTenantStatusCurrent = new LicenseTenantStatusCurrentOld(licensedByTenantIdInput, new ProvideHardwareIdService());
            // execute
            var resultAccount = licensedTenantStatusCurrent.AccountDetails;
            var resultBundles = licensedTenantStatusCurrent.Bundles;
            var resultApps = licensedTenantStatusCurrent.LooseApps;
            var resultTenantDetails = licensedTenantStatusCurrent.TenantDetails;
            // test
            resultAccount.Should().BeEquivalentTo(accountDetails);
            resultTenantDetails.Should().BeEquivalentTo(licensedByTenantIdInput);
            Assert.True(resultBundles.ContainsKey(ownedBundle.Identifier));
            Assert.True(resultApps.ContainsKey(looseApp.Identifier));
            Assert.Single(resultBundles);
            Assert.Single(resultApps);
            var looseAppStatus = resultApps[looseApp.Identifier];
            var bundleStatus = resultBundles[ownedBundle.Identifier];
            looseAppStatus.Should().BeEquivalentTo(expectedLooseAppStatus);
            bundleStatus.Should().BeEquivalentTo(expectedBundleStatus);
        }
        
        [Fact, Category("TenantStatus")]
        public void Test_Clone_Current_Status()
        {
            // prepare
            var licensedByTenantIdInput = CreateLicenseByTenantIdWithTemporaryLicenses();
            var looseApp = licensedByTenantIdInput.LicensedTenantDetails.OwnedApps[0];
            var ownedBundle = licensedByTenantIdInput.LicensedTenantDetails.OwnedBundles[0];
            var appsInOwnedBundle = licensedByTenantIdInput.LicensedTenantDetails.OwnedApps
                .Where(a => a.LicensedBundleId == ownedBundle.BundleId).ToList();
            var expectedLooseAppStatus = new LicenseTenantStatusApp(looseApp.NumberOfLicenses, looseApp.AdditionalNumberOfLicenses, looseApp.Identifier, looseApp.Name,
                looseApp.Status, looseApp.SoftwareName, looseApp.SoftwareIdentifier);
            var expectedBundleStatus = new LicenseTenantStatusUsedBundle(ownedBundle, appsInOwnedBundle);
            var accountDetails = new LicensedAccountDetails
            {
                Email = "teste@teste.com.br",
                Phone = "4635355432333",
                Status = LicensedAccountStatusEnum.Active,
                CnpjCpf = "04276545689",
                CompanyName = "Korp do Junior",
                TradingName = "teste",
                WebSite = "korp.junior.com.br"
            };
            licensedByTenantIdInput.LicensedTenantDetails.AccountDetails = accountDetails;
            var licensedTenantStatusCurrent = new LicenseTenantStatusCurrentOld(licensedByTenantIdInput, new MockProvideHardwareIdService());
            // execute
            var cloneStatus = licensedTenantStatusCurrent.Clone();
            // test
            var resultAccount = cloneStatus.AccountDetails;
            var resultBundles = cloneStatus.Bundles;
            var resultApps = cloneStatus.LooseApps;
            var resultTenantDetails = cloneStatus.TenantDetails;
            resultAccount.Should().BeEquivalentTo(accountDetails);
            resultTenantDetails.Should().BeEquivalentTo(licensedByTenantIdInput);
            Assert.True(resultBundles.ContainsKey(ownedBundle.Identifier));
            Assert.True(resultApps.ContainsKey(looseApp.Identifier));
            Assert.Single(resultBundles);
            Assert.Single(resultApps);
            var looseAppStatus = resultApps[looseApp.Identifier];
            var bundleStatus = resultBundles[ownedBundle.Identifier];
            looseAppStatus.Should().BeEquivalentTo(expectedLooseAppStatus);
            bundleStatus.Should().BeEquivalentTo(expectedBundleStatus);
        }
        
        [Fact, Category("TenantStatus")]
        public void Test_Restore_License_Single_Data()
        {
            // prepare
            var licensedByTenantIdInput = CreateLicenseByTenantIdWithTemporaryLicenses();
            var restoreInput = new List<AppLicenseConsumer>{ new AppLicenseConsumer("HaveBundleName", "HaveBundle", new DateTime(2090, 5, 5),
                new DateTime(2090, 5, 4), false, new ConsumeLicenseInput() )};
            var licensedTenantStatusCurrent = new LicenseTenantStatusCurrentOld(licensedByTenantIdInput, new ProvideHardwareIdService());
            licensedTenantStatusCurrent.Bundles.TryGetValue("Numb", out var bundle);
            // execute
            licensedTenantStatusCurrent.RestoreLicensesInUse(restoreInput);
            // test 
            Assert.Equal(1, bundle.BundleConsumedLicenseCount);
        }
        
        [Fact, Category("TenantStatus")]
        public void Test_Restore_License_Multiple_Data()
        {
            // prepare
            var licensedByTenantIdInput = CreateLicenseByTenantIdWithTemporaryLicenses();
            var ownedBundle = licensedByTenantIdInput.LicensedTenantDetails.OwnedBundles[0];
            licensedByTenantIdInput.LicensedTenantDetails.OwnedApps.Add(new LicensedAppDetails
            {
                Identifier = "HaveBundle_Multiple",
                NumberOfLicenses = 6,
                AdditionalNumberOfLicenses = 1,
                Name = "HaveBundleName_Multiple",
                Status = LicensedAppStatus.AppActive,
                SoftwareName = "App da Nicole",
                SoftwareIdentifier = "Test",
                LicensedBundleId = ownedBundle.BundleId
            });
            var restoreInput = new List<AppLicenseConsumer>{ 
                new AppLicenseConsumer("HaveBundleName", "HaveBundle", new DateTime(2090, 5, 5),
                new DateTime(2090, 5, 4), false, new ConsumeLicenseInput() 
                ),
                new AppLicenseConsumer("HaveBundleName_Multiple", "HaveBundle_Multiple", new DateTime(2090, 5, 5),
                    new DateTime(2090, 5, 4), false, new ConsumeLicenseInput() 
                )
            };
            var licensedTenantStatusCurrent = new LicenseTenantStatusCurrentOld(licensedByTenantIdInput, new ProvideHardwareIdService());
            licensedTenantStatusCurrent.Bundles.TryGetValue("Numb", out var bundle);
            // execute
            licensedTenantStatusCurrent.RestoreLicensesInUse(restoreInput);
            // test 
            Assert.Equal(2, bundle.BundleConsumedLicenseCount);
        }

        private LicenseByTenantIdOld CreateLicenseByTenantIdWithTemporaryLicenses()
        {
            var cnpjs = new List<string> {"94829839000180", "62134155000178"};
            var identifier = Guid.NewGuid();
            var status = LicensingStatus.Active;
            var expirationDateTime = new DateTime(3000, 4, 13);
            var bundleIdToApp = Guid.NewGuid();
            var tenantDetails = new Domain.Old.Contracts.LicensedTenant.LicensedTenantDetails
            {
                OwnedApps = new List<LicensedAppDetails>
                {
                    new LicensedAppDetails
                    {
                        Identifier = "BleedItOut",
                        NumberOfLicenses = 5,
                        AdditionalNumberOfLicenses = 1,
                        Name = "Lets Have Adventure",
                        Status = LicensedAppStatus.AppActive,
                        SoftwareName = "App do Junior",
                        SoftwareIdentifier = "LSTESTE"
                    },
                    new LicensedAppDetails
                    {
                        Identifier = "HaveBundle",
                        NumberOfLicenses = 6,
                        AdditionalNumberOfLicenses = 1,
                        Name = "HaveBundleName",
                        Status = LicensedAppStatus.AppActive,
                        SoftwareName = "App do Gubert",
                        SoftwareIdentifier = "Test",
                        LicensedBundleId = bundleIdToApp
                    }
                },
                OwnedBundles = new List<LicensedBundleDetails>
                {
                    new LicensedBundleDetails
                    {
                        Identifier = "Numb",
                        Name = "I Hate the Beach",
                        Status = LicensedBundleStatus.BundleActive,
                        BundleId = bundleIdToApp,
                        NumberOfLicenses = 15
                    }
                }
            };
            return new LicenseByTenantIdOld(identifier, status, expirationDateTime, cnpjs, tenantDetails, "hardwareId", LicenseConsumeType.Access);
        }
        
    }
}