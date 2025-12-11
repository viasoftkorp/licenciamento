using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Product;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.Product;
using Viasoft.Licensing.CustomerLicensing.Host.Controllers;
using Viasoft.Licensing.LicensingManagement.UnitTest;
using Xunit;


namespace Viasoft.Licensing.CustomerLicensing.Tests.Product
{
    public class ProductControllerTest : CustomerLicensingTestBase
    {
        private static Guid LicensingIdentifier => Guid.Parse("1BA7A91F-80C0-4975-842F-957A76504CE6");
        private static Guid ProductId => Guid.Parse("D7D0C7EA-6853-490F-B8FD-FA38FAB046FD");

        [Fact]
        public async Task GetAllProduct()
        {
            // prepare data
            var fakeLicensedBundleService = Substitute.For<IProductService>();
            var licenseUsageInRealTimeRepo = ServiceProvider.GetRequiredService<IRepository<LicenseUsageInRealTime>>();
            
            await licenseUsageInRealTimeRepo.InsertAsync(new LicenseUsageInRealTime()
            {
                BundleIdentifier = "Default",
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
            
            var expectedOutput = new PagedResultDto<ProductOutput>()
            {
                TotalCount = 1,
                Items = new List<ProductOutput>()
                {
                    new()
                    {
                        Id = new Guid(),
                        Name = "CRM",
                        LicensingModel = 0,
                        Identifier = "Default",
                        NumberOfUsedLicenses = 1
                    }
                }
            };

            fakeLicensedBundleService.GetAll(new Guid(), input).Returns(expectedOutput);
            var licensedBundleController = new ProductController(fakeLicensedBundleService);
            
            // execute
            var output = await licensedBundleController.GetAll(new Guid(), input);
            
            // test
            await fakeLicensedBundleService.Received(1).GetAll(new Guid(), input);
            output.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public async Task GetById()
        {
            // prepare data
            var fakeLicensedBundleService = Substitute.For<IProductService>();
            var licenseUsageInRealTimeRepo = ServiceProvider.GetRequiredService<IRepository<LicenseUsageInRealTime>>();
            
            await licenseUsageInRealTimeRepo.InsertAsync(new LicenseUsageInRealTime()
            {
                BundleIdentifier = "Default",
                AppIdentifier = string.Empty,
                AppName = string.Empty,
                SoftwareIdentifier = string.Empty,
                SoftwareName = string.Empty
            }, true);

            var expectedOutput = new ProductOutput()
            {
                Id = new Guid(),
                Identifier = "Default"
            };

            var input = new GetProductByIdInput()
            {
                ProductType = ProductType.LicensedBundle
            };

            fakeLicensedBundleService.GetById(LicensingIdentifier, ProductId, input).Returns(expectedOutput);
            var licensedBundleController = new ProductController(fakeLicensedBundleService);
            
            // execute
            var output = await licensedBundleController.GetById(LicensingIdentifier, ProductId, input);
            
            // test
            await fakeLicensedBundleService.Received(1).GetById(LicensingIdentifier, ProductId, input);
            output.Should().BeEquivalentTo(expectedOutput);
        }
    }
}