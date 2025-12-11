using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData;

namespace Viasoft.Licensing.LicensingManagement.Host.Extensions
{
    public static class AmbientDataExtensions
    {
        public static IServiceCollection AddNullEnvironmentAmbientDataCallOptionsResolver(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAmbientDataCallOptionsResolver, NullEnvironmentAmbientDataCallOptionsResolver>();
            return serviceCollection;
        }
    }
}