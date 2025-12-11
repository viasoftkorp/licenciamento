using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.UserBehaviour;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.UserBehaviour;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    public class LicenseUserBehaviourController : BaseController
    {
        private readonly ILicenseUserBehaviourService _licenseUserBehaviourService;

        public LicenseUserBehaviourController(ILicenseUserBehaviourService licenseUserBehaviourService)
        {
            _licenseUserBehaviourService = licenseUserBehaviourService;
        }

        [HttpGet]
        public Task<PagedResultDto<LicenseUserBehaviourOutput>> GetUsersBehaviour([FromQuery] GetAllLicenseUserBehaviour input)
        {
            return _licenseUserBehaviourService.GetUsersBehaviour(input);
        }
        
        [HttpGet("/licensing/customer-licensing/user-behaviour/products/{productIdentifier}/floating")]
        public Task<PagedResultDto<LicenseUserBehaviourOutput>> GetUsersBehaviourFromBundleFloating([FromRoute] string productIdentifier, [FromQuery] GetUserBehaviorFromProductInput input)
        {
            return _licenseUserBehaviourService.GetUsersBehaviourFloating(productIdentifier, input);
        }
        
        [HttpGet("/licensing/customer-licensing/user-behaviour/products/{productIdentifier}/named/online")]
        public Task<PagedResultDto<LicenseUserBehaviourNamedOnlineOutput>> GetUsersBehaviourFromBundleNamedOnline([FromRoute] string productIdentifier, [FromQuery] GetUserBehaviorFromProductInput input)
        {
            return _licenseUserBehaviourService.GetUserBehaviourNamedOnline(productIdentifier, input);
        }
        
        [HttpGet("/licensing/customer-licensing/user-behaviour/products/{productIdentifier}/named/offline")]
        public Task<PagedResultDto<LicenseUserBehaviourNamedOfflineOutput>> GetUsersBehaviourNamedOffline([FromRoute] string productIdentifier, [FromQuery] GetUserBehaviorFromProductInput input)
        {
            return _licenseUserBehaviourService.GetUserBehaviourNamedOffline(productIdentifier, input);
        }
    }
}