using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.Testing;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.NamedUserBundle;
using Xunit;


namespace Viasoft.Licensing.LicensingManagement.UnitTest.NamedUserBundle
{
    public class NamedUserBundleServiceTest : CustomerLicensingTestBase
    {  
        private static readonly ILogger<NamedUserBundleService> Logger = new NullLogger<NamedUserBundleService>();

        private static HttpResponseMessage _httpResponseMessageOk = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK
        };

        private static ApiClientCallPrototype _apiClientCallPrototype = new ApiClientCallPrototype();
        private static Guid LicensingIdentifier => Guid.Parse("F6B23E09-2501-4F34-9243-7112363A5F57");
        private static Guid LicensedTenant => Guid.Parse("FBA11C2D-84CD-47F5-AC2A-37D722F195CD");
        private static Guid LicensedBundle => Guid.Parse("2C6D9C82-8D90-4935-9802-CD87BB7C96E9"); 
        private static string NamedUserEmail => "admin@korp.com";
        
        [Fact]
        public async Task GetAllUsers()
        {
            // prepare data
            var fakeApiClientCallBuilder = new Mock<IApiClientCallBuilder>();
            var fakeApiClientCall = new Mock<IApiClientCall>();
            var fakePagedResult = new PagedResultDto<GetAllUsersOutput>()
            {
                TotalCount = 1,
                Items = new List<GetAllUsersOutput>()
                {
                    new()
                    {
                        Email = "admin@korp.com"
                    }
                }
            };
            
            var input = new GetAllUsersInput()
            {
                SkipCount = 0,
                MaxResultCount = 25
            };

            fakeApiClientCall.Setup(s => s.ResponseCallAsync<PagedResultDto<GetAllUsersOutput>>())
                .ReturnsAsync(fakePagedResult);
            fakeApiClientCallBuilder.Setup(s => s.WithEndpoint(
                    It.IsAny<string>())
                .WithServiceName(It.IsAny<string>())
                .WithHttpMethod(HttpMethod.Get)
                .Build()).Returns(fakeApiClientCall.Object);
            
            var namedUserBundleService = GetServiceWithMocking(fakeApiClientCallBuilder);
            
            // execute
           var output = await namedUserBundleService.GetAllUsers(LicensingIdentifier, input);
            
            // test
            fakeApiClientCallBuilder.Invocations.AssertSingle(nameof(IApiClientCallBuilder.WithEndpoint));
            fakeApiClientCall.Invocations.AssertSingle(nameof(IApiClientCall.ResponseCallAsync));
            output.Should().BeEquivalentTo(fakePagedResult);
        }
        
        [Fact]
        public async Task RemoveNamedUserFromBundle()
        {
            // prepare data
            var fakeApiClientCallBuilder = new Mock<IApiClientCallBuilder>();
            var fakeApiClientCall = new Mock<IApiClientCall>();
            var fakeApiClientCallGetNamedUserBundle = new Mock<IApiClientCall>();
            var fakeResult = new ApiClientCallResponse<RemoveNamedUserFromBundleOutput>(_httpResponseMessageOk, _apiClientCallPrototype, Logger);
            
            fakeApiClientCall.Setup(s => s.CallAsync<RemoveNamedUserFromBundleOutput>())
                .ReturnsAsync(fakeResult);
            fakeApiClientCallGetNamedUserBundle.Setup(s => s.ResponseCallAsync<GetNamedUserFromBundleOutput>())
                .ReturnsAsync(new GetNamedUserFromBundleOutput()
                {
                    NamedUserBundleLicenseOutputs = new PagedResultDto<NamedUserBundleLicenseOutput>()
                    {
                        TotalCount = 1,
                        Items = new List<NamedUserBundleLicenseOutput>()
                        {
                            new()
                            {
                                Id = new Guid()
                            }
                        }
                    }
                });
            fakeApiClientCallBuilder.Setup(s => s.WithEndpoint(
                    It.IsAny<string>())
                .WithServiceName(It.IsAny<string>())
                .WithHttpMethod(HttpMethod.Delete)
                .DontThrowOnFailureCall()
                .Build()).Returns(fakeApiClientCall.Object);
            fakeApiClientCallBuilder.Setup(s => s.WithEndpoint(
                    It.IsAny<string>())
                .WithServiceName(It.IsAny<string>())
                .WithHttpMethod(HttpMethod.Get)
                .DontThrowOnFailureCall()
                .Build()).Returns(fakeApiClientCallGetNamedUserBundle.Object);
            var namedUserBundleService = GetServiceWithMocking(fakeApiClientCallBuilder);
            
            // execute
            await namedUserBundleService.RemoveNamedUserFromProduct(LicensedTenant, LicensedBundle, NamedUserEmail, ProductType.LicensedBundle);
            
            // test
            fakeApiClientCallBuilder.Invocations.AssertCount(2, nameof(IApiClientCallBuilder.WithEndpoint));
            fakeApiClientCall.Invocations.AssertSingle(nameof(IApiClientCall.CallAsync));
            fakeApiClientCallGetNamedUserBundle.Invocations.AssertSingle(nameof(IApiClientCall.ResponseCallAsync));
        }
        
        [Fact]
        public async Task AddNamedUserToLicensedBundle()
        {
            // prepare data
            var fakeApiClientCallBuilder = new Mock<IApiClientCallBuilder>();
            var fakeApiClientCall = new Mock<IApiClientCall>();
            var fakeResult = new ApiClientCallResponse<NamedUserBundleLicenseOutput>(_httpResponseMessageOk, _apiClientCallPrototype, Logger);
            var inputProduct = new CreateNamedUserProductLicenseInput()
            {
                NamedUserEmail = "admin@korp.com",
                NamedUserId = Guid.Parse("167B6FAA-E49A-4702-938D-C8000FB39970"),
                ProductType = ProductType.LicensedBundle
            }; 
            var input = new UpdateNamedUserInput()
            {
                NamedUserEmail = "admin@korp.com",
                NamedUserId = Guid.Parse("167B6FAA-E49A-4702-938D-C8000FB39970"),
            };
            fakeApiClientCall.Setup(s => s.CallAsync<NamedUserBundleLicenseOutput>())
                .ReturnsAsync(fakeResult);
            fakeApiClientCallBuilder.Setup(s => s.WithEndpoint(
                    It.IsAny<string>())
                .WithServiceName(It.IsAny<string>())
                .WithBody(It.Is<CreateNamedUserInput>(e => e.NamedUserEmail == input.NamedUserEmail && e.NamedUserId == input.NamedUserId))
                .WithHttpMethod(HttpMethod.Post)
                .DontThrowOnFailureCall()
                .Build()).Returns(fakeApiClientCall.Object);
            
            var namedUserBundleService = GetServiceWithMocking(fakeApiClientCallBuilder);
            // execute
            await namedUserBundleService.AddNamedUserToProduct(LicensedTenant, LicensedBundle, inputProduct);
            
            // test
            fakeApiClientCallBuilder.Invocations.AssertSingle(nameof(IApiClientCallBuilder.WithEndpoint));
            fakeApiClientCall.Invocations.AssertSingle(nameof(IApiClientCall.CallAsync));
        }
        [Fact]
        public async Task UpdateNamedUserFromBundle()
        {
            // prepare data
            var fakeApiClientCallBuilder = new Mock<IApiClientCallBuilder>();
            var fakeApiClientCall = new Mock<IApiClientCall>();
            var fakeApiClientCallGetNamedUserBundle = new Mock<IApiClientCall>();
            var fakeResult = new ApiClientCallResponse<UpdateNamedUsersFromBundleOutput>(_httpResponseMessageOk, _apiClientCallPrototype, Logger);
            var inputProduct = new UpdateNamedUserProductLicenseInput()
            {
                NamedUserEmail = "admin@korp.com",
                NamedUserId = Guid.Parse("167B6FAA-E49A-4702-938D-C8000FB39970"),
                ProductType = ProductType.LicensedBundle
            };
            
            fakeApiClientCall.Setup(s => s.CallAsync<UpdateNamedUsersFromBundleOutput>())
                .ReturnsAsync(fakeResult);
            fakeApiClientCallGetNamedUserBundle.Setup(s => s.ResponseCallAsync<GetNamedUserFromBundleOutput>())
                .ReturnsAsync(new GetNamedUserFromBundleOutput()
                {
                    NamedUserBundleLicenseOutputs = new PagedResultDto<NamedUserBundleLicenseOutput>()
                    {
                        TotalCount = 1,
                        Items = new List<NamedUserBundleLicenseOutput>()
                        {
                            new()
                            {
                                Id = new Guid()
                            }
                        }
                    }
                });
            var input = new UpdateNamedUserInput()
            {
                NamedUserEmail = "admin@korp.com",
                NamedUserId = Guid.Parse("167B6FAA-E49A-4702-938D-C8000FB39970"),
            };
            fakeApiClientCallBuilder.Setup(s => s.WithEndpoint(
                    It.IsAny<string>())
                .WithServiceName(It.IsAny<string>())
                .WithBody(It.Is<UpdateNamedUserInput>(e => e.NamedUserEmail == input.NamedUserEmail && e.NamedUserId == input.NamedUserId))
                .WithHttpMethod(HttpMethod.Put)
                .DontThrowOnFailureCall()
                .Build()).Returns(fakeApiClientCall.Object);

            fakeApiClientCallBuilder.Setup(s => s.WithEndpoint(
                    It.IsAny<string>())
                .WithServiceName(It.IsAny<string>())
                .WithHttpMethod(HttpMethod.Get)
                .DontThrowOnFailureCall()
                .Build()).Returns(fakeApiClientCallGetNamedUserBundle.Object);
            
            var namedUserBundleService = GetServiceWithMocking(fakeApiClientCallBuilder);
            
            // execute
            await namedUserBundleService.UpdateNamedUserFromProduct(LicensedTenant, LicensedBundle,"admin@korp.com", inputProduct);
            
            // test
            fakeApiClientCallBuilder.Invocations.AssertCount(2, nameof(IApiClientCallBuilder.WithEndpoint));
            fakeApiClientCall.Invocations.AssertSingle(nameof(IApiClientCall.CallAsync));
            fakeApiClientCallGetNamedUserBundle.Invocations.AssertSingle(nameof(IApiClientCall.ResponseCallAsync));
        }
        private NamedUserBundleService GetServiceWithMocking(Mock<IApiClientCallBuilder> apiClientCallBuilder)
        {
            return ActivatorUtilities.CreateInstance<NamedUserBundleService>(ServiceProvider, apiClientCallBuilder.Object);
        }
    }
}