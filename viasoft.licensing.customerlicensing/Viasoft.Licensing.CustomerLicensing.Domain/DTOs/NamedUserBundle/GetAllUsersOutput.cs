using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle
{
    public class GetAllUsersOutput
    {
        public string FirstName { get; set; }
        
        public string SecondName { get; set; }
        
        public string Login { get; set; }

        public string UrlImg { get; set; }

        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public Guid Id { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreationTime { get; set; }
    }
}