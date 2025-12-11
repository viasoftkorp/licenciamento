using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageStatistics;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageStatistics;
using Viasoft.Licensing.LicensingManagement.UnitTest;
using Xunit;

namespace Viasoft.Licensing.CustomerLicensing.Tests.LicenseUsageStaticsControllerUnitTest
{
    public class LicenseUsageStatisticsControllerUnitTest: CustomerLicensingTestBase
    {
        [Fact]
        public async Task Check_Controller_Call_GetLicenseUsages()
        {
            // Prepare Data
            var fakeLicenseUsageStatisticsService = new Mock<ILicenseUsageStatisticsService>();
            var fakeResult = new List<LicenseUsageReportingOutput>();
            fakeLicenseUsageStatisticsService.Setup(s => s.GetLicenseUsageForReporting())
                .ReturnsAsync(fakeResult);
            var controller = GetController(fakeLicenseUsageStatisticsService);
            // Execute
            var result = await controller.GetLicenseUsageForReporting();
            // Test
            var count = fakeLicenseUsageStatisticsService.Invocations.Where(i => i.Method.Name == nameof(ILicenseUsageStatisticsService.GetLicenseUsageForReporting)).ToList().Count;
            Assert.Equal(1, count);
            result.Should().BeEquivalentTo(fakeResult);
        }
        
        [Fact]
        public async Task Check_Controller_Call_GetLicenseIdentifiersForUsages()
        {
            // Prepare Data
            var fakeLicenseUsageStatisticsService = new Mock<ILicenseUsageStatisticsService>();
            var fakeResult = new List<Guid>();
            fakeLicenseUsageStatisticsService.Setup(s => s.GetLicenseIdentifiersForUsageReporting("teste"))
                .ReturnsAsync(fakeResult);
            var controller = GetController(fakeLicenseUsageStatisticsService);
            // Execute
            var result = await controller.GetLicenseIdentifiersForUsageReporting("teste");
            // Test
            var count = fakeLicenseUsageStatisticsService.Invocations.Where(i => i.Method.Name == nameof(ILicenseUsageStatisticsService.GetLicenseIdentifiersForUsageReporting)).ToList().Count;
            Assert.Equal(1, count);
            var invocation = fakeLicenseUsageStatisticsService.Invocations
                .First(i => i.Method.Name == nameof(ILicenseUsageStatisticsService.GetLicenseIdentifiersForUsageReporting)).Arguments;
            Assert.Equal("teste", invocation[0]);
            result.Should().BeEquivalentTo(fakeResult);
        }
        
        private Host.Controllers.LicenseUsageStatisticsController GetController(Mock<ILicenseUsageStatisticsService> fakeLicenseUsageStatisticsService)
        {
            return ActivatorUtilities.CreateInstance<Host.Controllers.LicenseUsageStatisticsController>(ServiceProvider, fakeLicenseUsageStatisticsService.Object);
        }
    }
}