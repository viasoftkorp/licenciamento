using System;
using Viasoft.Core.Mapper.Extensions;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Host.Mappers;
using Viasoft.Licensing.LicensingManagement.Infrastructure.EntityFrameworkCore;

namespace Viasoft.Licensing.LicensingManagement.UnitTest;

public class LicensingManagementTestBase : UnitTestBase
{
    protected override Type GetDbContextType()
    {
        return typeof(ViasoftLicenseServerDbContext);
    }
    
    protected override void AddServices()
    {
        AddAutoMapperProfiles();
        base.AddServices();
    }
    
    protected void AddAutoMapperProfiles()
    {
        var assemblies = new[] {typeof(LicenseServerMapperProfile).Assembly};
        ServiceCollection.AddAutoMapper(assemblies);
    }
}