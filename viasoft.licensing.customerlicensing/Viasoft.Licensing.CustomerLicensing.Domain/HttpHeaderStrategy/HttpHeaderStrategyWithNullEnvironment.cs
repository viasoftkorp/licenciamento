using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;
using Viasoft.Core.AspNetCore.AmbientData;

namespace Viasoft.Licensing.CustomerLicensing.Domain.HttpHeaderStrategy;

public class HttpHeaderStrategyWithNullEnvironment : AmbientDataCallOptionsHttpHeaderStrategy
{

    public HttpHeaderStrategyWithNullEnvironment(IServiceProvider serviceProvider) : base(ActivatorUtilities.CreateInstance<AmbientDataCallOptionsResolver>(serviceProvider))
    {
    }

    public override Dictionary<string, string> GetHeaders()
    {
        var headers = base.GetHeaders();
        headers["EnvironmentId"] = null;
        return headers;
    }
}