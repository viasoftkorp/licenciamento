using System.Net.Http;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.Authentication.Proxy.Dtos.Outputs;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Consts;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.Authentication;

public class AuthenticationCaller : IAuthenticationCaller, ITransientDependency
{
    private readonly IApiClientCallBuilder _apiClientCallBuilder;

    public AuthenticationCaller(IApiClientCallBuilder apiClientCallBuilder)
    {
        _apiClientCallBuilder = apiClientCallBuilder;
    }

    public async Task<GetUserOutput> GetUserIdByEmail(string email)
    {
        var call = _apiClientCallBuilder
            .WithServiceName(AuthenticationConsts.ServiceName)
            .WithEndpoint(AuthenticationConsts.Users.GetByEmail(email))
            .WithHttpMethod(HttpMethod.Get)
            .Build();

        var response = await call.CallAsync<GetUserOutput>();
        if (response.IsSuccessStatusCode)
        {
            var result = await response.GetResponse();
            return result;
        }

        return null;
    }
}