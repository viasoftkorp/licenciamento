using System;
using Microsoft.AspNetCore.Http;
using Viasoft.Core.AmbientData;
using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.AspNetCore.AmbientData;
using Viasoft.Core.IoC.Abstractions;

namespace Viasoft.Licensing.CustomerLicensing.Host.AmbientData
{
    public class NullEnvironmentAmbientDataCallOptionsResolver: AmbientDataCallOptionsResolver
    {
        public NullEnvironmentAmbientDataCallOptionsResolver(IAmbientData ambientData, IHttpContextAccessor httpContextAccessor) : base(ambientData, httpContextAccessor)
        {
        }

        public override AmbientDataCallOptions GetOptions()
        {
            var options = base.GetOptions();
            options.EnvironmentId = null;
            return options;
        }
    }
}