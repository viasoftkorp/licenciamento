using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DynamicLinqQueryBuilder;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.CustomerLicensing.Domain.Consts;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle;
using System.Text.Json;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserApp;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserProduct;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;
using Viasoft.Licensing.CustomerLicensing.Domain.Extensions;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.NamedUserBundle
{
    public class NamedUserBundleService : INamedUserBundleService, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;

        public NamedUserBundleService(IApiClientCallBuilder apiClientCallBuilder)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
        }
        public async Task<PagedResultDto<GetAllUsersOutput>> GetAllUsers(Guid licensingIdentifier, GetAllUsersInput input)
        {
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(
                    ExternalServicesConsts.LicensingManagement.LicensedBundle.GetAllUsers(licensingIdentifier, input))
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .Build();

            var gatewayCallResponse = await gatewayCall.ResponseCallAsync<PagedResultDto<GetAllUsersOutput>>();

            return gatewayCallResponse;
        }

        public async Task<AddNamedUserToProductOutput> AddNamedUserToProduct(Guid licensedTenantId, Guid productId, CreateNamedUserProductLicenseInput input)
        {
            var gatewayInput = new CreateNamedUserInput()
            {
                NamedUserId = input.NamedUserId,
                NamedUserEmail = input.NamedUserEmail,
                DeviceId = input.DeviceId
            };
            
            if (input.ProductType == ProductType.LicensedApp)
            {
                var gatewayCall = _apiClientCallBuilder
                    .WithEndpoint(ExternalServicesConsts.LicensingManagement.LicensedBundle.AddNamedUserToApp(licensedTenantId, productId))
                    .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                    .WithBody(gatewayInput)
                    .WithHttpMethod(HttpMethod.Post)
                    .DontThrowOnFailureCall()
                    .Build();

                var rawResult = await gatewayCall.CallAsync<AddNamedUserToLicensedAppOutput>();
                if (rawResult.IsSuccessStatusCode)
                {
                    return new AddNamedUserToProductOutput()
                    {
                        ValidationCode = AddNamedUserToProductValidationCode.NoError
                    };
                }
                var result = await rawResult.GetResponse();
                return AddNamedUserToProductOutput.FromAddNamedUserToLicensedAppOutput(result);
            }
            else
            {
                var gatewayCall = _apiClientCallBuilder
                    .WithEndpoint(
                        ExternalServicesConsts.LicensingManagement.LicensedBundle.AddNamedUserToBundle(licensedTenantId, productId))
                    .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                    .WithBody(gatewayInput)
                    .WithHttpMethod(HttpMethod.Post)
                    .DontThrowOnFailureCall()
                    .Build();

                var rawResult = await gatewayCall.CallAsync<NamedUserBundleLicenseOutput>();
                if (rawResult.IsSuccessStatusCode)
                {
                    return new AddNamedUserToProductOutput()
                    {
                        ValidationCode = AddNamedUserToProductValidationCode.NoError
                    };
                }
                var result = await rawResult.GetResponse();
                return AddNamedUserToProductOutput.FromAddNamedUserToLicensedBundleOutput(result);
            }
        }

        public async Task<UpdateNamedUsersFromProductOutput> UpdateNamedUserFromProduct(Guid licensedTenantId, Guid productId,
            string namedUserEmail, UpdateNamedUserProductLicenseInput input)
        {
            var gatewayInput = new UpdateNamedUserInput()
            {
                NamedUserId = input.NamedUserId,
                NamedUserEmail = input.NamedUserEmail,
                DeviceId = input.DeviceId
            };

            var namedUserBundleId = input.ProductType == ProductType.LicensedBundle
                ? await GetNamedUserIdFromBundle(licensedTenantId, productId, namedUserEmail)
                : await GetNamedUserIdFromApp(licensedTenantId, productId, namedUserEmail);

            if (input.ProductType == ProductType.LicensedBundle)
            {
                var gatewayCall = _apiClientCallBuilder
                    .WithEndpoint(
                        ExternalServicesConsts.LicensingManagement.LicensedBundle.UpdateNamedUserFromBundle(licensedTenantId, productId, namedUserBundleId))
                    .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                    .WithBody(gatewayInput)
                    .WithHttpMethod(HttpMethod.Put)
                    .DontThrowOnFailureCall()
                    .Build();

                var rawResult = await gatewayCall.CallAsync<UpdateNamedUsersFromBundleOutput>();
                if (rawResult.IsSuccessStatusCode)
                {
                    return new UpdateNamedUsersFromProductOutput()
                    {
                        ValidationCode = UpdateNamedUsersFromProductValidationCode.NoError
                    };
                }
                var result = await rawResult.GetResponse();
                return new UpdateNamedUsersFromProductOutput()
                {
                    ValidationCode = result.ValidationCode.ToUpdateNamedUserBundleToProductValidationCode()
                };
            }
            else
            {
                var gatewayCall = _apiClientCallBuilder
                    .WithEndpoint(
                        ExternalServicesConsts.LicensingManagement.LicensedBundle.UpdateNamedUserFromApp(licensedTenantId, productId, namedUserBundleId))
                    .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                    .WithBody(gatewayInput)
                    .WithHttpMethod(HttpMethod.Put)
                    .DontThrowOnFailureCall()
                    .Build();

                var rawResult = await gatewayCall.CallAsync<UpdateNamedUsersFromAppOutput>();
                if (rawResult.IsSuccessStatusCode)
                {
                    return new UpdateNamedUsersFromProductOutput()
                    {
                        ValidationCode = UpdateNamedUsersFromProductValidationCode.NoError
                    };
                }
                var result = await rawResult.GetResponse();
                return new UpdateNamedUsersFromProductOutput()
                {
                    ValidationCode = result.ValidationCode.ToUpdateNamedUserAppToProductValidationCode()
                };
            }
        }
        
        public async Task<RemoveNamedUserFromProductOutput> RemoveNamedUserFromProduct(Guid licensedTenant, Guid productId, string namedUserEmail, ProductType productType)
        {
            var namedUserBundleId = productType == ProductType.LicensedBundle
                ? await GetNamedUserIdFromBundle(licensedTenant, productId, namedUserEmail)
                : await GetNamedUserIdFromApp(licensedTenant, productId, namedUserEmail);
            
            if (productType == ProductType.LicensedBundle)
            {
                var gatewayCall = _apiClientCallBuilder
                    .WithEndpoint(
                        ExternalServicesConsts.LicensingManagement.LicensedBundle.RemoveNamedUserFromBundle(licensedTenant, productId, namedUserBundleId))
                    .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                    .WithHttpMethod(HttpMethod.Delete)
                    .DontThrowOnFailureCall()
                    .Build();

                var rawResult = await gatewayCall.CallAsync<RemoveNamedUserFromBundleOutput>();
                if (rawResult.IsSuccessStatusCode)
                {
                    return new RemoveNamedUserFromProductOutput
                    {
                        ValidationCode = RemoveNamedUserFromProductValidationCode.NoError
                    };
                }
                var result = await rawResult.GetResponse();
                return new RemoveNamedUserFromProductOutput()
                {
                    ValidationCode = result.ValidationCode.ToRemoveNamedUserBundleToProductValidationCode()
                };
            }
            else
            {
                var gatewayCall = _apiClientCallBuilder
                    .WithEndpoint(
                        ExternalServicesConsts.LicensingManagement.LicensedBundle.RemoveNamedUserFromApp(licensedTenant, productId, namedUserBundleId))
                    .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                    .WithHttpMethod(HttpMethod.Delete)
                    .DontThrowOnFailureCall()
                    .Build();

                var rawResult = await gatewayCall.CallAsync<RemoveNamedUserFromAppOutput>();
                if (rawResult.IsSuccessStatusCode)
                {
                    return new RemoveNamedUserFromProductOutput
                    {
                        ValidationCode = RemoveNamedUserFromProductValidationCode.NoError
                    };
                }
                var result = await rawResult.GetResponse();
                return new RemoveNamedUserFromProductOutput()
                {
                    ValidationCode = result.ValidationCode.ToRemoveNamedUserAppToProductValidationCode()
                };
            }
        }

        private async Task<Guid> GetNamedUserIdFromBundle(Guid licensedTenant, Guid licensedBundle, string namedUserEmail)
        {
            var filterRule = new JsonNetFilterRule()
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>()
                {
                    new()
                    {
                        Field = "namedUserEmail",
                        Operator = "equal",
                        Type = "string",
                        Value = namedUserEmail
                    }
                }
            };
            
            var advancedFilter = JsonSerializer.Serialize(filterRule);
            var filter = new PagedFilteredAndSortedRequestInput()
            {
                AdvancedFilter = advancedFilter
            };
            
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(
                    ExternalServicesConsts.LicensingManagement.LicensedBundle.GetNamedUserFromBundle(licensedTenant, licensedBundle, filter))
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .DontThrowOnFailureCall()
                .Build();
            
            var rawResult = await gatewayCall.ResponseCallAsync<GetNamedUserFromBundleOutput>();

            return rawResult.NamedUserBundleLicenseOutputs.Items.First().Id;
        }
        
        private async Task<Guid> GetNamedUserIdFromApp(Guid licensedTenant, Guid licensedApp, string namedUserEmail)
        {
            var filterRule = new JsonNetFilterRule()
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>()
                {
                    new()
                    {
                        Field = "namedUserEmail",
                        Operator = "equal",
                        Type = "string",
                        Value = namedUserEmail
                    }
                }
            };
            
            var advancedFilter = JsonSerializer.Serialize(filterRule);
            var filter = new PagedFilteredAndSortedRequestInput()
            {
                AdvancedFilter = advancedFilter
            };
            
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(
                    ExternalServicesConsts.LicensingManagement.LicensedBundle.GetNamedUserFromApp(licensedTenant, licensedApp, filter))
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .DontThrowOnFailureCall()
                .Build();
            
            var rawResult = await gatewayCall.ResponseCallAsync<GetNamedUserFromAppOutput>();

            return rawResult.NamedUserAppLicenseOutputs.Items.First().Id;
        }
    }
}