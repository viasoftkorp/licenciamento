using System;
using Viasoft.Core.DDD.Application.Dto.Entities;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO
{
    public class LicenseTenantUpdateInput: IEntityDto
    {
        [StrictRequired]
        public Guid Id { get; set; }
        
        [StrictRequired]
        public Guid AccountId { get; set; }
        
        [StrictRequired(AllowZeroNumeric = true)]
        public LicensingStatus Status { get; set; }
        
        [StrictRequired]
        public Guid Identifier { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }
        
        public LicenseConsumeType LicenseConsumeType { get; set; }
        
        [StrictRequired]
        public string LicensedCnpjs { get; set; }

        [StrictRequired]
        public string AdministratorEmail { get; set; }

        public string Notes { get; set; }
        
        public string HardwareId { get; set; }

    }
}