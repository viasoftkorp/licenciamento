using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NSubstitute;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Product;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.Product;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageInRealTime;
using Viasoft.Licensing.LicensingManagement.UnitTest;
using Xunit;

namespace Viasoft.Licensing.CustomerLicensing.Tests.Product
{
    public class ProductServiceTest : CustomerLicensingTestBase
    {
        private static Guid LicensedTenantId => Guid.Parse("F6B23E09-2501-4F34-9243-7112363A5F57");
        private static Guid LicensingIdentifier => Guid.Parse("64D88844-5C75-4EF3-A917-7CA6F0060AE5");
        private static string BundleIdentifier => "Default";

        [Fact]
        public async Task GetAllProduct()
        {
            // prepare data
            var fakeApiClientCallBuilder = new Mock<IApiClientCallBuilder>();
            var fakeApiClientCall = new Mock<IApiClientCall>();
            var fakeLicenseUsageInRealTimeService = Substitute.For<ILicenseUsageInRealTimeService>();
            var licenseUsageInRealTimeRepo = ServiceProvider.GetRequiredService<IRepository<LicenseUsageInRealTime>>();
            var fakePagedResult = new PagedResultDto<ProductOutput>()
            {
                TotalCount = 1,
                Items = new List<ProductOutput>()
                {
                    new()
                    {
                        Identifier = BundleIdentifier
                    }
                }
            };
            
            fakeApiClientCall.Setup(s => s.ResponseCallAsync<PagedResultDto<ProductOutput>>())
                .ReturnsAsync(fakePagedResult);

            fakeApiClientCallBuilder.Setup(s => s.WithEndpoint(
                    It.IsAny<string>())
                .WithServiceName(It.IsAny<string>())
                .WithHttpMethod(HttpMethod.Get)
                .Build()).Returns(fakeApiClientCall.Object);

            var licensedBundleService = GetServiceWithMocking(fakeApiClientCallBuilder, fakeLicenseUsageInRealTimeService);

            await licenseUsageInRealTimeRepo.InsertAsync(new LicenseUsageInRealTime()
            {
                BundleIdentifier = BundleIdentifier,
                AppIdentifier = string.Empty,
                AppName = string.Empty,
                SoftwareIdentifier = string.Empty,
                SoftwareName = string.Empty
            }, true);
            
            var input = new GetAllProductsInput()
            {
                SkipCount = 0,
                MaxResultCount = 25
            };

            // execute
            await licensedBundleService.GetAll(Guid.Parse("D7D0C7EA-6853-490F-B8FD-FA38FAB046FD"), input);
            
            // test
            fakeApiClientCallBuilder.Invocations.AssertSingle(nameof(IApiClientCallBuilder.WithEndpoint));
            fakeApiClientCall.Invocations.AssertSingle(nameof(IApiClientCall.ResponseCallAsync));
        }
        
        [Fact]
        public async Task GetById()
        {
            // prepare data
            var fakeApiClientCallBuilder = new Mock<IApiClientCallBuilder>();
            var fakeApiClientCall = new Mock<IApiClientCall>();
            var fakeLicenseUsageInRealTimeService = Substitute.For<ILicenseUsageInRealTimeService>();
            var licenseUsageInRealTimeRepo = ServiceProvider.GetRequiredService<IRepository<LicenseUsageInRealTime>>();
            
            var licensedBundleOutput = new ProductOutput()
            {
              Identifier = BundleIdentifier,
              NumberOfLicenses = 2,
            };
            
            fakeApiClientCall.Setup(s => s.ResponseCallAsync<ProductOutput>())
                .ReturnsAsync(licensedBundleOutput);

            fakeApiClientCallBuilder.Setup(s => s.WithEndpoint(
                    It.IsAny<string>())
                .WithServiceName(It.IsAny<string>())
                .WithHttpMethod(HttpMethod.Get)
                .Build()).Returns(fakeApiClientCall.Object);

            var licensedBundleService = GetServiceWithMocking(fakeApiClientCallBuilder, fakeLicenseUsageInRealTimeService);

            await licenseUsageInRealTimeRepo.InsertAsync(new LicenseUsageInRealTime()
            {
                BundleIdentifier = BundleIdentifier,
                LicensingModel = LicensingModels.Floating,
                LicensingIdentifier = LicensingIdentifier, 
                AppIdentifier = string.Empty,
                AppName = string.Empty,
                SoftwareIdentifier = string.Empty,
                SoftwareName = string.Empty
            }, true);

            var expectedOutput = new ProductOutput()
            {
                Identifier = BundleIdentifier,
                NumberOfLicenses = 2,
                NumberOfUsedLicenses = 1
            };

            var input = new GetProductByIdInput()
            {
                ProductType = ProductType.LicensedBundle,
                LicensingIdentifier = LicensingIdentifier
            };

            var bundleIdentifiers = It.Is<List<string>>(e => e.Contains(BundleIdentifier));

            var licenseUsage = new Dictionary<string, int>();
            licenseUsage.Add(BundleIdentifier, 1);
            fakeLicenseUsageInRealTimeService.GetLicensesConsumed(LicensingIdentifier, bundleIdentifiers, It.IsAny<List<string>>())
               .ReturnsForAnyArgs(licenseUsage);
            
            // execute
            var output = await licensedBundleService.GetById(LicensedTenantId, Guid.Parse("D7D0C7EA-6853-490F-B8FD-FA38FAB046FD"), input);
            
            // test
            fakeApiClientCallBuilder.Invocations.AssertSingle(nameof(IApiClientCallBuilder.WithEndpoint));
            fakeApiClientCall.Invocations.AssertSingle(nameof(IApiClientCall.ResponseCallAsync));
            await fakeLicenseUsageInRealTimeService.Received(1).GetLicensesConsumed(LicensingIdentifier, Arg.Is<List<string>>(list => list.Contains(BundleIdentifier)), Arg.Any<List<string>>());
            output.Should().BeEquivalentTo(expectedOutput);
        }
        
        private ProductService GetServiceWithMocking(Mock<IApiClientCallBuilder> apiClientCallBuilder, ILicenseUsageInRealTimeService licenseUsageInRealTimeService)
        {
            return ActivatorUtilities.CreateInstance<ProductService>(ServiceProvider, apiClientCallBuilder.Object, licenseUsageInRealTimeService);
        }
    }
}