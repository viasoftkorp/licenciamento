using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Viasoft.Licensing.CustomerLicensing.Host.ActionResults
{
    public class HttpResponseMessageResult: IActionResult
    {
        private readonly HttpResponseMessage _httpResponseMessage;

        public HttpResponseMessageResult(HttpResponseMessage httpResponseMessage)
        {
            _httpResponseMessage = httpResponseMessage;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            try
            {
                context.HttpContext.Response.StatusCode = (int)_httpResponseMessage.StatusCode;
                
                var responseFeature = context.HttpContext.Features.Get<IHttpResponseFeature>();
                if (responseFeature != null)
                {
                    responseFeature.ReasonPhrase = _httpResponseMessage.ReasonPhrase;
                }

                var responseHeaders = _httpResponseMessage.Headers;
                
                if (responseHeaders.TransferEncodingChunked == true &&
                    responseHeaders.TransferEncoding.Count == 1)
                {
                    responseHeaders.TransferEncoding.Clear();
                }

                foreach (var header in responseHeaders)
                {
                    context.HttpContext.Response.Headers.Append(header.Key, header.Value.ToArray());
                }

                if (_httpResponseMessage.Content != null)
                {
                    var contentHeaders = _httpResponseMessage.Content.Headers;
                    
                    var _ = contentHeaders.ContentLength;

                    foreach (var header in contentHeaders)
                    {
                        context.HttpContext.Response.Headers.Append(header.Key, header.Value.ToArray());
                    }

                    await _httpResponseMessage.Content.CopyToAsync(context.HttpContext.Response.Body);
                }
            }
            finally
            {
                _httpResponseMessage?.Dispose();
            }
        }
    }
}