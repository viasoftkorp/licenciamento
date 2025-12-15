﻿using System;
 using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

 namespace Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant
{
    public class LicensedBundleDetails
    {
        public Guid BundleId { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public LicensedBundleStatus Status { get; set; }
        public int NumberOfLicenses { get; set; }
    }
}