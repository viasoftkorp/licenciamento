using System;
using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant
{
    public class LicenseByTenantIdOld
    {
        public LicenseByTenantIdOld(Guid identifier, LicensingStatus status, DateTime? expirationDateTime, List<string> cnpjs, LicensedTenantDetails licensedTenantDetails,string hardwareId, 
            LicenseConsumeType licenseConsumeType = LicenseConsumeType.Connection)
        {
            Identifier = identifier;
            Status = status;
            ExpirationDateTime = expirationDateTime;
            Cnpjs = cnpjs;
            LicensedTenantDetails = licensedTenantDetails;
            LicenseConsumeType = licenseConsumeType;
            HardwareId = hardwareId;
        }

        private LicenseByTenantIdOld(LicenseByTenantIdOld licenseByTenantIdOld)
        {
            Identifier = licenseByTenantIdOld.Identifier;
            Status = licenseByTenantIdOld.Status;
            ExpirationDateTime = licenseByTenantIdOld.ExpirationDateTime;
            Cnpjs = licenseByTenantIdOld.Cnpjs;
            LicensedTenantDetails = licenseByTenantIdOld.LicensedTenantDetails;
            LicenseConsumeType = licenseByTenantIdOld.LicenseConsumeType;
            HardwareId = licenseByTenantIdOld.HardwareId;
        }

        public LicenseByTenantIdOld()
        {
        }
        
        public Guid Identifier { get; set; }
        public LicensingStatus Status { get; set; }
        
        public LicenseConsumeType LicenseConsumeType { get; set; }
        public string StatusDescription => Status.ToString();
        public DateTime? ExpirationDateTime { get; set; }

        public List<string> Cnpjs { get; set; }

        public LicensedTenantDetails LicensedTenantDetails { get; set; }
        
        public string HardwareId { get; set; }

        public LicenseByTenantIdOld Clone()
        {
            var licenseByTenantId = new LicenseByTenantIdOld(this);
            return licenseByTenantId;
        }
    }
}