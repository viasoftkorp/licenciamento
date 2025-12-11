using System.Threading.Tasks;

namespace Viasoft.Licensing.LicenseServer.Shared.AccessTokenProvider
{
    public interface IAccessTokenProvider
    {
        public Task<string> ProvideAccessToken(string clientSecret, string scope, string clientId, string authority);
    }
}