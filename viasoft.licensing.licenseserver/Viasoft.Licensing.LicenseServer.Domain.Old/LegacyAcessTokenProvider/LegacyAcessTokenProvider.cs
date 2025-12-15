using System.Threading.Tasks;
using Viasoft.Core.Authorization.Services;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.LegacyAcessTokenProvider
{
    public class LegacyAcessTokenProvider: IAccessTokenProvider
    {
        public Task<string> GetAccessToken(string serviceName)
        {
            throw new System.NotImplementedException();
        }
    }
}