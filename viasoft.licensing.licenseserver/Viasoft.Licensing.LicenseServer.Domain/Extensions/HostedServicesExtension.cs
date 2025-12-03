using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Viasoft.Licensing.LicenseServer.Domain.Extensions
{
    public static class HostedServicesExtension
    {
        public static IServiceCollection AddHostedServices(this IServiceCollection serviceCollection, Assembly assembly)
        {
            return serviceCollection.AddHostedServices(new [] { assembly });
        }
        
        public static IServiceCollection AddHostedServices(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        {
            var typesToRegister = assemblies
                .SelectMany(assembly => 
                    assembly.GetTypes()
                        .Where(type => type.GetInterfaces().Contains(typeof(IHostedService))))
                .ToList();

            foreach (var typeToRegister in typesToRegister)
                serviceCollection.AddTransient(typeof(IHostedService), typeToRegister);

            return serviceCollection;
        }
    }
}