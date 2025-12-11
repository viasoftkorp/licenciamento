using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.UserBehaviour;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.UserBehaviour
{
    public interface ILicenseUserBehaviourService
    { 
        Task<PagedResultDto<LicenseUserBehaviourOutput>> GetUsersBehaviour(GetAllLicenseUserBehaviour input);
        Task<PagedResultDto<LicenseUserBehaviourOutput>> GetUsersBehaviourFloating(string productIdentifier, GetUserBehaviorFromProductInput input);
        Task<PagedResultDto<LicenseUserBehaviourNamedOnlineOutput>> GetUserBehaviourNamedOnline(string productIdentifier, GetUserBehaviorFromProductInput input);
        Task<PagedResultDto<LicenseUserBehaviourNamedOfflineOutput>> GetUserBehaviourNamedOffline(string productIdentifier, GetUserBehaviorFromProductInput input);

    }
}