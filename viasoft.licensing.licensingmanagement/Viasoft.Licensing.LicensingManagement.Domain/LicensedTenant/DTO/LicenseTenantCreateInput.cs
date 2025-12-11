using System;
using System.Collections.Generic;
using Viasoft.Core.DDD.Application.Dto.Entities;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO
{
    public class LicenseTenantCreateInput : IEntityDto
    {
        [StrictRequired]
        public Guid Id { get; set; }
        
        [StrictRequired]
        public Guid AccountId { get; set; }
        
        [StrictRequired(AllowZeroNumeric = true)]
        public LicensingStatus Status { get; set; }
        
        [StrictRequired]
        public Guid Identifier { get; set; }
        
        public LicenseConsumeType LicenseConsumeType { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }
        
        [StrictRequired]
        public string LicensedCnpjs { get; set; }

        [StrictRequired]
        public string AdministratorEmail { get; set; }
        
        //atualmente isso daqui é só utilizado pelo Viasoft.Infrastructure
        public Guid? AdministratorUserId {get;set;}

        public string Notes {get; set;}
        public List<Guid> BundleIds { get; set; }
        public int NumberOfLicenses { get; set; }
        public string HardwareId { get; set; }
    }
}