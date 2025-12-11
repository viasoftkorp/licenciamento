using Microsoft.Extensions.Configuration;

namespace Viasoft.Licensing.LicensingManagement.Domain.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetCompanySearchToken(this IConfiguration configuration)
        {
            return configuration["ConsultaCnpjApiKey"];
        }
    }
}