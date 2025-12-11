using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.UserDetails.DTO
{
    public class UserDetail
    {
        public bool IsActive { get; set; }

        public string SecondName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
        
        public Guid UserId { get; set; }
    }
}