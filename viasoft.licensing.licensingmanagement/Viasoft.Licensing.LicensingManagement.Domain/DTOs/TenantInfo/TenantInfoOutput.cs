using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.TenantInfo
{
    public class TenantInfoOutput
    {
        public LicensingStatus LicensingStatus { get; set; }

        public string Cnpj { get; set; }
        
        public OperationValidation OperationValidation { get; set; }

        public string OperationValidationDescription => OperationValidation.ToString();
        
        public Guid LicensedTenantId { get; set; }
    }
}