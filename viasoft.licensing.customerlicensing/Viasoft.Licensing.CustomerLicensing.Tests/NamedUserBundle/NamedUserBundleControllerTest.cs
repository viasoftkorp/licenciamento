using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.Testing;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.NamedUserBundle;
using Viasoft.Licensing.CustomerLicensing.Host.Controllers;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.NamedUserBundle
{
    public class NamedUserBundleControllerTest : CustomerLicensingTestBase
    {
        private static Guid LicensingIdentifier => Guid.Parse("77D9BCEE-18B6-46D8-9818-2A43FDFED8E3");

        [Fact]
        public async Task GetAllUsers()
        {
            // prepare data
            var fakeNamedUserBundleService = Substitute.For<INamedUserBundleService>();
            var input = new GetAllUsersInput()
            {
                SkipCount = 0,
                MaxResultCount = 25
            };
            
            var expectedOutput = new PagedResultDto<GetAllUsersOutput>()
            {
                TotalCount = 1,
                Items = new List<GetAllUsersOutput>()
                {
                    new()
                    {
                        Email = "admin@admin.com"
                    }
                }
            };
            
            fakeNamedUserBundleService.GetAllUsers(LicensingIdentifier, input).Returns(expectedOutput);

            var namedUserBundleController = new NamedUserController(fakeNamedUserBundleService);
            
            // execute
            var output = await namedUserBundleController.GetAllUsers(LicensingIdentifier, input);
            
            // test
            await fakeNamedUserBundleService.Received(1).GetAllUsers(LicensingIdentifier, input);
            output.Should().BeEquivalentTo(expectedOutput);
        }
    }
}