using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.TenantInfo
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