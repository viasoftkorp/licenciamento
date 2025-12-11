using System;
using System.Threading.Tasks;

namespace Viasoft.Licensing.LicensingManagement.Domain.Seeder
{
    public interface ISoftwareAndAppDataSeeder
    {
        Task Seed(Guid tenantIdentifier, string administratorEmail, string defaultLicensedCnpjs, Guid? administratorUserId);
    }
}