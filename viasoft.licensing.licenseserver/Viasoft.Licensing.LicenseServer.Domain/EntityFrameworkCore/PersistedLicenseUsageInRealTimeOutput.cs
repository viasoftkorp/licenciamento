using System;
using System.Text;
using Newtonsoft.Json;
using Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models;

namespace Viasoft.Licensing.LicenseServer.Domain.EntityFrameworkCore;

//this class was created based on LicenseUsageInRealTimeOutput excluding complex types to support relational database usage
public class PersistedLicenseUsageInRealTimeOutput
{
    public PersistedLicenseUsageInRealTimeOutput()
    {
        
    }
    public PersistedLicenseUsageInRealTimeOutput(LicenseUsageInRealTimeOutput licenseUsageInRealTimeOutput)
    {
        TenantId = licenseUsageInRealTimeOutput.TenantId;
        Update(licenseUsageInRealTimeOutput);
    }
    
    public Guid TenantId { get; set; }
        
    public byte[] SoftwareUtilized { get; set;}

    public byte[] LicenseUsageInRealTimeDetails { get; set; }

    public void Update(LicenseUsageInRealTimeOutput licenseUsageInRealTimeOutput)
    {
        SoftwareUtilized = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(licenseUsageInRealTimeOutput.SoftwareUtilized));
        LicenseUsageInRealTimeDetails = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(licenseUsageInRealTimeOutput.LicenseUsageInRealTimeDetails));
    }
}