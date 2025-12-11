using System;
using System.Collections.Generic;
using Viasoft.Core.Testing;
using Viasoft.Licensing.CustomerLicensing.Infrastructure.EntityFrameworkCore;

namespace Viasoft.Licensing.LicensingManagement.UnitTest;

public class CustomerLicensingTestBase : UnitTestBase
{
    protected override Type GetDbContextType()
    {
        return typeof(ViasoftCustomerLicensingDbContext);
    }
}