using System;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts
{
    public static class Apps
    {
        public static class SingleLicense
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("7DBEB581-B85D-4477-BBB1-D7A8BDFBD140");
            public static string Name => nameof(SingleLicense);
            public static string Identifier => "M1";
            public static Guid? LicensedBundleId => null;
            public static LicensedAppStatus Status => LicensedAppStatus.AppActive;
            public static int NumberOfLicenses => 1;
            public static int AdditionalNumberOfLicenses => 0;
        }
        
        public static class MultipleLicense
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("7DBEB581-B85D-4477-BBB1-D7A8BDFBD140");
            public static string Name => nameof(MultipleLicense);
            public static string Identifier => "M1_S";
            public static Guid? LicensedBundleId => null;
            public static LicensedAppStatus Status => LicensedAppStatus.AppActive;
            public static int NumberOfLicenses => 2;
            public static int AdditionalNumberOfLicenses => 0;
        }
        
        public static class SingleLicenseWithinBundle
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("D02DCF89-1435-420E-99AE-F07BA1EF8E01");
            public static string Name => nameof(SingleLicenseWithinBundle);
            public static string Identifier => "M1_1";
            public static Guid? LicensedBundleId => Bundles.Simple.Id;
            public static LicensedAppStatus Status => LicensedAppStatus.AppActive;
            public static int NumberOfLicenses => 1;
            public static int AdditionalNumberOfLicenses => 0;
        }

        public static class MultipleLicenseWithinBundle
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("D02DCF89-1435-420E-99AE-F07BA1EF8E01");
            public static string Name => nameof(SingleLicenseWithinBundle);
            public static string Identifier => "M1_1_S";
            public static Guid? LicensedBundleId => Bundles.Multiple.Id;
            public static LicensedAppStatus Status => LicensedAppStatus.AppActive;
            public static int NumberOfLicenses => 2;
            public static int AdditionalNumberOfLicenses => 0;
        }

        public static class BundledPlusSingleAdditionalLicense
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("060A1D61-8E0B-4095-9FC2-F39FBF392E05");
            public static string Name => nameof(BundledPlusSingleAdditionalLicense);
            public static string Identifier => "M2";
            public static Guid? LicensedBundleId => Bundles.Simple.Id;
            public static LicensedAppStatus Status => LicensedAppStatus.AppActive;
            public static int NumberOfLicenses => Bundles.Simple.NumberOfLicenses;
            public static int AdditionalNumberOfLicenses => 1;
        }
        
        public static class BlockedLicense
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("770D6D1C-7399-4D9C-83BA-08C973F8107C");
            public static string Name => nameof(BlockedLicense);
            public static string Identifier => "M3";
            public static Guid? LicensedBundleId => null;
            public static LicensedAppStatus Status => LicensedAppStatus.AppBlocked;
            public static int NumberOfLicenses => 1;
            public static int AdditionalNumberOfLicenses => 1;
        }
        
        public static class BundledWithNoAdditionalLicense
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("244C7152-3F8E-44FD-9F15-58989F6BF928");
            public static string Name => nameof(BundledWithNoAdditionalLicense);
            public static string Identifier => "M4";
            public static Guid? LicensedBundleId => Bundles.Simple.Id;
            public static LicensedAppStatus Status => LicensedAppStatus.AppActive;
            public static int NumberOfLicenses => Bundles.Simple.NumberOfLicenses;
            public static int AdditionalNumberOfLicenses => 0;
        }
        
        public static class BlockedAdditionalLicense
        {
            private static Guid _id;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("15A96A27-59FD-432A-8234-99E005323FBF");
            public static string Name => nameof(BlockedAdditionalLicense);
            public static string Identifier => "M5";
            public static Guid? LicensedBundleId => Bundles.Simple.Id;
            public static LicensedAppStatus Status => LicensedAppStatus.AppBlocked;
            public static int NumberOfLicenses => Bundles.Simple.NumberOfLicenses;
            public static int AdditionalNumberOfLicenses => 1;
        }
    }
}