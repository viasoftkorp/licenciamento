using System;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.MultiTenancy.Abstractions.Store;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings;

namespace Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.LicensingManagement
{
    public interface ILicensingManagementApiCaller
    {
        Task<IApiClientCallResponse<InfrastructureConfigurationUpdateOutput>> UpdateInfrastructureConfiguration(InfrastructureConfigurationUpdateInput input);
    }
}