using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Consts;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Authentication;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.User;
using Viasoft.Licensing.LicensingManagement.Domain.HttpHeaderStrategy;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.AuthenticationService
{
    public class AuthenticationService: IAuthenticationService, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly IServiceProvider _serviceProvider;

        public AuthenticationService(IApiClientCallBuilder apiClientCallBuilder, IServiceProvider serviceProvider)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _serviceProvider = serviceProvider;
        }

        public async Task<PagedResultDto<UsersGetAllOutput>> GetAllUsersFromTenantIdentifier(Guid identifier,
            GetAllUsersInput getAllUsersInput)
        {
            var headerStrategy = new TenantIdentifierHeaderStrategy(identifier.ToString(), _serviceProvider);
            var call = _apiClientCallBuilder
                .WithEndpoint(AddQueryParams(AuthenticationConsts.Users.GetAll, getAllUsersInput))
                .WithServiceName(AuthenticationConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .WithHttpHeaderStrategy(headerStrategy)
                .Build();

            return await call.ResponseCallAsync<PagedResultDto<UsersGetAllOutput>>();
        }

        private static string AddQueryParams(string uri, GetAllUsersInput input)
        {
            var dictionary = new Dictionary<string, string>
            {
                {nameof(input.Filter), input.Filter},
                {nameof(input.Sorting), input.Sorting},
                {nameof(input.AdvancedFilter), input.AdvancedFilter},
                {nameof(input.SkipCount), input.SkipCount.ToString()},
                {nameof(input.MaxResultCount), input.MaxResultCount.ToString()}
            };
            return QueryHelpers.AddQueryString(uri, dictionary);
        }
    }
}