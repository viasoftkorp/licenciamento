using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Licensing.CustomerLicensing.Host.AmbientData;

namespace Viasoft.Licensing.CustomerLicensing.Host.Extensions
{
    public static class AmbientDataCallOptionsExtensions
    {
        public static IServiceCollection AddNullEnvironmentAmbientDataCallOptionsResolver(this IServiceCollection service)
        {
            service.AddTransient<IAmbientDataCallOptionsResolver, NullEnvironmentAmbientDataCallOptionsResolver>();
            return service;
        }
    }
}