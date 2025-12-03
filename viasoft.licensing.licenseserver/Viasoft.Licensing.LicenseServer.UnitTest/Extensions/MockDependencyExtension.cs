using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Extensions
{
    public static class MockDependencyExtension
    {
        public static IServiceCollection ReplaceDependenciesWithMockImplementation(this IServiceCollection serviceCollection, Assembly assembly)
        {
            return serviceCollection.ReplaceDependenciesWithMockImplementation(new [] {assembly});
        }
        
        public static IServiceCollection ReplaceDependenciesWithMockImplementation(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        {
            return serviceCollection.ReplaceDependenciesWithMockByType(assemblies);
        }

        private static IServiceCollection ReplaceDependenciesWithMockByType(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
        {
            var registeredServiceTypes = serviceCollection
                .Select(s => s.ServiceType)
                .ToList();
            
            var registeredImplementationTypes = serviceCollection
                .Select(s => s.ImplementationType)
                .ToList();
            
            var mockDependencies = assemblies
                .SelectMany(assembly => assembly.GetTypes()
                    // Get types not registered on the services collection, and then get mock types that implement an interface already registered. 
                    .Where(type => !registeredImplementationTypes.Contains(type) && type.GetInterfaces().Any(i => registeredServiceTypes.Contains(i))))
                .Select(type =>
                { 
                    var serviceType = type.BaseType != null
                        ? type.GetInterfaces().Except(type.BaseType.GetInterfaces()).First()
                        : type.GetInterfaces().First();
                    return new
                    {
                        ImplementationType = type,
                        ServiceType = serviceType,
                        ServiceLifeTime = serviceCollection.First(s => s.ServiceType == serviceType).Lifetime
                    };
                })
                .ToList();

            var serviceDescriptors = mockDependencies
                .Select(mockDependency => new ServiceDescriptor(mockDependency.ServiceType, mockDependency.ImplementationType, mockDependency.ServiceLifeTime))
                .ToList();
            
            foreach (var serviceDescriptor in serviceDescriptors)
                serviceCollection.Replace(serviceDescriptor);
            
            return serviceCollection;
        }
    }
}