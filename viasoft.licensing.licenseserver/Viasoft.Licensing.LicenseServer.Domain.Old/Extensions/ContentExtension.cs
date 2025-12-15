using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Extensions
{
    public static class ContentExtension
    {
        public static StringContent GenerateStringContent(this object input, string mediaType = "application/json")
        {
            var body = JsonConvert.SerializeObject(input);
            var content = new StringContent(body, Encoding.UTF8, mediaType);
            return content;
        }
    }
}