using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Services.AuditingLogService;
using Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices.Consts;
using Viasoft.Licensing.LicensingManagement.Domain.Services.UserDetails;
using Viasoft.Licensing.LicensingManagement.Domain.Services.UserDetails.DTO;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.BatchOperations.BatchOperationLoggerService
{
    public class BatchOperationLoggerServiceServiceUnitTest: LicensingManagementTestBase
    {
        
        [Fact(DisplayName = "Testa a chamada para os logs quando insere aplicativos em pacotes")]
        public async Task Test_LogInsertAppsInBundles_Calls_AuditingLog()
        {
            // prepare data
            var fakeUserDetails = new UserDetail { UserName = "Blackout - Linkin Park", UserId = Guid.NewGuid() };
            var appIdentifiers = new List<string> { "teste_1", "teste_2"};
            var bundleIdentifiers = new List<string> { "teste_1", "teste_2"};
            
            var expectedAppsToDetails = appIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var expectedBundlesToDetails = bundleIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.InsertAppsInBundles, expectedAppsToDetails, expectedBundlesToDetails);

            var userDetailsService = new Mock<IUserDetailsService>();
            userDetailsService
                .Setup(r => r.GetUserDetailsAsync()).ReturnsAsync(fakeUserDetails);
            var auditingLogService = new Mock<IAuditingLogService>();
            
            var service = GetServiceWithMocking(userDetailsService, auditingLogService);
            // execute
            await service.LogInsertAppsInBundles(appIdentifiers, bundleIdentifiers);
            // test calls
            userDetailsService.Invocations.AssertSingle(nameof(IUserDetailsService.GetUserDetailsAsync));
            auditingLogService.Invocations.AssertSingle(nameof(IAuditingLogService.InsertLogs));
            // test arguments
            var argumentsInFakeAuditingLogService = auditingLogService.Invocations.Where(i => i.Method.Name == nameof(IAuditingLogService.InsertLogs)).ToList()[0].Arguments;
            await Common_Assert_Auditing_Logs(argumentsInFakeAuditingLogService, logDetails, fakeUserDetails, LogAction.InsertMultipleAppsInMultipleBundles, ActionNameConsts.InsertAppsInBundlesActionName);
        }
        
        [Fact(DisplayName = "Testa a chamada para os logs quando insere aplicativos em pacotes e os licencia")]
        public async Task Test_LogInsertAppsFromBundlesInLicenses_Calls_AuditingLog()
        {
            // prepare data
            var fakeUserDetails = new UserDetail {UserName = "Hear Me Now - Alok, Zeeba", UserId = Guid.NewGuid()};
            var appIdentifiers = new List<string> { "teste_1", "teste_2"};
            var licenseTenantIdentifiers = new List<Guid> { Guid.NewGuid(),  Guid.NewGuid()};
            
            var expectedAppsToDetails = appIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var expectedLicenseTenantToDetails = licenseTenantIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.InsertAppsFromBundlesInLicenses, expectedAppsToDetails, expectedLicenseTenantToDetails);

            var userDetailsService = new Mock<IUserDetailsService>();
            userDetailsService
                .Setup(r => r.GetUserDetailsAsync()).ReturnsAsync(fakeUserDetails);
            var auditingLogService = new Mock<IAuditingLogService>();
            
            var service = GetServiceWithMocking(userDetailsService, auditingLogService);
            // execute
            await service.LogInsertAppsFromBundlesInLicenses(appIdentifiers, licenseTenantIdentifiers);
            // test calls
            userDetailsService.Invocations.AssertSingle(nameof(IUserDetailsService.GetUserDetailsAsync));
            auditingLogService.Invocations.AssertSingle(nameof(IAuditingLogService.InsertLogs));
            // test arguments
            var argumentsInFakeAuditingLogService = auditingLogService.Invocations.Where(i => i.Method.Name == nameof(IAuditingLogService.InsertLogs)).ToList()[0].Arguments;
            await Common_Assert_Auditing_Logs(argumentsInFakeAuditingLogService, logDetails, fakeUserDetails, LogAction.InsertMultipleAppsFromBundlesInLicenses, ActionNameConsts.InsertAppsFromBundlesInLicensesActionName);
        }
        
        [Fact(DisplayName = "Testa a chamada para os logs quando remove aplicativo de um pacote")]
        public async Task Test_LogRemoveAppFromBundleInLicenses_Calls_AuditingLog()
        {
            // prepare data
            var fakeUserDetails = new UserDetail {UserName = "Sail - Awolnation", UserId = Guid.NewGuid()};
            var appIdentifier = "teste_1";
            var licenseTenantIdentifiers = new List<Guid> { Guid.NewGuid(),  Guid.NewGuid()};
            
            var expectedLicenseTenantToDetails = licenseTenantIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.RemoveAppInLicensesDetails, appIdentifier, expectedLicenseTenantToDetails);
            
            var userDetailsService = new Mock<IUserDetailsService>();
            userDetailsService
                .Setup(r => r.GetUserDetailsAsync()).ReturnsAsync(fakeUserDetails);
            var auditingLogService = new Mock<IAuditingLogService>();
            
            var service = GetServiceWithMocking(userDetailsService, auditingLogService);
            // execute
            await service.LogRemoveAppFromBundleInLicenses(appIdentifier, licenseTenantIdentifiers);
            // test calls
            userDetailsService.Invocations.AssertSingle(nameof(IUserDetailsService.GetUserDetailsAsync));
            auditingLogService.Invocations.AssertSingle(nameof(IAuditingLogService.InsertLogs));
            // test arguments
            var argumentsInFakeAuditingLogService = auditingLogService.Invocations.Where(i => i.Method.Name == nameof(IAuditingLogService.InsertLogs)).ToList()[0].Arguments;
            await Common_Assert_Auditing_Logs(argumentsInFakeAuditingLogService, logDetails, fakeUserDetails, LogAction.RemoveAppInLicenses, ActionNameConsts.RemovedAppInLicensesActionName);
        }
        
        [Fact(DisplayName = "Testa a chamada para os logs quando insere pacotes em licenças")]
        public async Task Test_LogInsertBundleInLicenses_Calls_AuditingLog()
        {
            // prepare data
            var fakeUserDetails = new UserDetail {UserName = "Wiped Out - The Neighbourhood", UserId = Guid.NewGuid()};
            var bundleIdentifiers = new List<string> { "teste_1", "teste_2"};
            var licenseTenantIdentifiers = new List<Guid> { Guid.NewGuid(),  Guid.NewGuid()};
            var licenseNumber = 500;
            
            var expectedBundlesToDetails = bundleIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var expectedLicenseTenantToDetails = licenseTenantIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.InsertBundlesInLicensesDetails, expectedBundlesToDetails, licenseNumber, expectedLicenseTenantToDetails);
            
            var userDetailsService = new Mock<IUserDetailsService>();
            userDetailsService
                .Setup(r => r.GetUserDetailsAsync()).ReturnsAsync(fakeUserDetails);
            var auditingLogService = new Mock<IAuditingLogService>();
            
            var service = GetServiceWithMocking(userDetailsService, auditingLogService);
            // execute
            await service.LogInsertBundleInLicenses(licenseNumber, bundleIdentifiers, licenseTenantIdentifiers);
            // test calls
            userDetailsService.Invocations.AssertSingle(nameof(IUserDetailsService.GetUserDetailsAsync));
            auditingLogService.Invocations.AssertSingle(nameof(IAuditingLogService.InsertLogs));
            // test arguments
            var argumentsInFakeAuditingLogService = auditingLogService.Invocations.Where(i => i.Method.Name == nameof(IAuditingLogService.InsertLogs)).ToList()[0].Arguments;
            await Common_Assert_Auditing_Logs(argumentsInFakeAuditingLogService, logDetails, fakeUserDetails, LogAction.InsertMultipleBundlesInMultipleLicenses, ActionNameConsts.InsertBundlesInLicensesActionName);
        }
        
        [Fact(DisplayName = "Testa a chamada para os logs quando insere aplicativos em licenças")]
        public async Task Test_LogAppsInLicenses_Calls_AuditingLog()
        {
            // prepare data
            var fakeUserDetails = new UserDetail {UserName = "Happier - Bastille, Marshmello", UserId = Guid.NewGuid()};
            var appIdentifiers = new List<string> { "teste_1", "teste_2"};
            var licenseTenantIdentifiers = new List<Guid> { Guid.NewGuid(),  Guid.NewGuid()};
            var licenseNumber = 500;
            
            var expectedAppsToDetails = appIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var expectedLicenseTenantToDetails = licenseTenantIdentifiers.Select(identifier => identifier.ToString()).Aggregate((a, b) => $"{a}, {b}");
            var logDetails = string.Format(DetailsConsts.InsertAppsInLicensesDetails, expectedAppsToDetails, licenseNumber, expectedLicenseTenantToDetails);
            
            var userDetailsService = new Mock<IUserDetailsService>();
            userDetailsService
                .Setup(r => r.GetUserDetailsAsync()).ReturnsAsync(fakeUserDetails);
            var auditingLogService = new Mock<IAuditingLogService>();
            
            var service = GetServiceWithMocking(userDetailsService, auditingLogService);
            // execute
            await service.LogAppsInLicenses(licenseNumber, appIdentifiers, licenseTenantIdentifiers);
            // test calls
            userDetailsService.Invocations.AssertSingle(nameof(IUserDetailsService.GetUserDetailsAsync));
            auditingLogService.Invocations.AssertSingle(nameof(IAuditingLogService.InsertLogs));
            // test arguments
            var argumentsInFakeAuditingLogService = auditingLogService.Invocations.Where(i => i.Method.Name == nameof(IAuditingLogService.InsertLogs)).ToList()[0].Arguments;
            await Common_Assert_Auditing_Logs(argumentsInFakeAuditingLogService, logDetails, fakeUserDetails, LogAction.InsertMultipleAppsInMultipleLicenses, ActionNameConsts.InsertAppsInLicensesActionName);
        }
        
        private Task Common_Assert_Auditing_Logs(IReadOnlyList<object> argumentsInvoked, string details, UserDetail userDetail, LogAction logAction, string actionNameConsts)
        {
            var logArguments = (Domain.Entities.AuditingLog) argumentsInvoked[0];
            Assert.Equal(details, logArguments.Details);
            Assert.Equal(userDetail.UserName, logArguments.UserName);
            Assert.Equal(userDetail.UserId, logArguments.UserId);
            Assert.Equal(logAction, logArguments.Action);
            Assert.Equal(LogType.BatchAction, logArguments.Type);
            Assert.Equal(actionNameConsts, logArguments.ActionName);
            return Task.CompletedTask;
        }

        public Domain.Services.BatchOperationServices.BatchOperationLoggerService.BatchOperationLoggerService GetServiceWithMocking(Mock<IUserDetailsService> userDetails, Mock<IAuditingLogService> auditingLog)
        {
            return ActivatorUtilities.CreateInstance<Domain.Services.BatchOperationServices.BatchOperationLoggerService.BatchOperationLoggerService>(ServiceProvider, userDetails.Object, auditingLog.Object);
        }
    }
}