using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.AuditingLog
{
    public class AuditingLogServiceUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testa se a inserção de um log está sendo realizada corretamente")]
        public async Task Check_Inserted_Logs()
        {
            // prepare data
            var service = GetService();
            var fakeInsertData = GetFakeLog();
            // execute
            var result = await service.InsertLogs(fakeInsertData);
            // test
            var singleResult = await ServiceProvider.GetService<IRepository<Domain.Entities.AuditingLog>>().SingleAsync();
            singleResult.Should().BeEquivalentTo(result);
        }
        
        private Domain.Services.AuditingLogService.AuditingLogService GetService()
        {
            var repository = ServiceProvider.GetService<IRepository<Domain.Entities.AuditingLog>>();
            return new Domain.Services.AuditingLogService.AuditingLogService(repository);
        }

        private Domain.Entities.AuditingLog GetFakeLog()
        {
            return new Domain.Entities.AuditingLog
            {
                Action = LogAction.InsertAppInLicenses,
                Details = "teste",
                ActionName = "teste",
                UserId = Guid.NewGuid(),
                UserName = "junior",
                Type = LogType.BatchAction
            };
        }
    }
}