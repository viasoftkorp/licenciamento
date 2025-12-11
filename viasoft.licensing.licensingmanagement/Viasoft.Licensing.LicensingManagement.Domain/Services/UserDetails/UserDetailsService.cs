using System.Threading.Tasks;
using Viasoft.Core.Identity.Abstractions;
using Viasoft.Core.Identity.Abstractions.Store;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Services.UserDetails.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.UserDetails
{
    public class UserDetailsService: IUserDetailsService, ITransientDependency
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUserStore _userStore;

        public UserDetailsService(ICurrentUser currentUser, IUserStore userStore)
        {
            _currentUser = currentUser;
            _userStore = userStore;
        }
        
        public async Task<UserDetail> GetUserDetailsAsync()
        {
            var detailsModel = await _userStore.GetUserDetailsAsync(_currentUser.Id);
            return new UserDetail
            {
                Email = detailsModel.Email,
                FirstName = detailsModel.FirstName,
                IsActive = detailsModel.IsActive,
                SecondName = detailsModel.SecondName,
                UserId = _currentUser.Id,
                UserName = detailsModel.Login
            };
             
        }
    }
}