using System;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts
{
    public static class Bundles
    {
        public static class Simple
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("770D6D1C-7399-4D9C-83BA-08C973F8107C");
            public static string Identifier => "B1";
            public static string Name => nameof(Simple);
            public static LicensedBundleStatus Status => LicensedBundleStatus.BundleActive;
            public static int NumberOfLicenses => 1;
        }
        
        public static class Multiple
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("770D6D1C-7399-4D9C-83BA-08C973F8107C");
            public static string Identifier => "B1";
            public static string Name => nameof(Multiple);
            public static LicensedBundleStatus Status => LicensedBundleStatus.BundleActive;
            public static int NumberOfLicenses => 2;
        }
    }
}