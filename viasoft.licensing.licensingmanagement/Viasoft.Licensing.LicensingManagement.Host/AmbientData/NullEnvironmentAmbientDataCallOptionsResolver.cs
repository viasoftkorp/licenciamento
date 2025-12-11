using Microsoft.AspNetCore.Http;
using Viasoft.Core.AmbientData;
using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.AspNetCore.AmbientData;

namespace Viasoft.Licensing.LicensingManagement.Host.AmbientData
{
    public class NullEnvironmentAmbientDataCallOptionsResolver: AmbientDataCallOptionsResolver
    {
        public NullEnvironmentAmbientDataCallOptionsResolver(IAmbientData ambientData, IHttpContextAccessor contextAccessor) : base(ambientData, contextAccessor)
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