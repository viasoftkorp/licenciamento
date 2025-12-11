using System;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Domain.AmbientData
{
    public class TenantIdParameterHint: Attribute
    {
        public string Name { get; }
        public ParameterLocation ParameterLocation { get; }
        public TenantIdParameterKind Kind { get; }

        public TenantIdParameterHint(string name, ParameterLocation parameterLocation)
        {
            Name = name;
            ParameterLocation = parameterLocation;
            Kind = TenantIdParameterKind.LicensingIdentifier;
        }
        
        public TenantIdParameterHint(string name, ParameterLocation parameterLocation, TenantIdParameterKind kind)
        {
            Name = name;
            ParameterLocation = parameterLocation;
            Kind = kind;
        }
    }
}