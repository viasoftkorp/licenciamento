namespace Viasoft.Licensing.LicensingManagement.Domain.Authorizations
{
    public class Policy
    {
        #region BatchOperations

        public const string InsertAppInLicenses = "BatchOperation.InsertAppInLicenses";

        public const string RemoveAppInLicenses = "BatchOperation.RemoveAppInLicenses";
        
        public const string InsertBundlesInLicenses = "BatchOperation.InsertBundlesInLicenses";
        
        public const string InsertAppsInLicenses = "BatchOperation.InsertAppsInLicenses";
        
        public const string InsertAppsInBundles = "BatchOperation.InsertAppsInBundles";
        
        #endregion
        
        #region LicenseRegion
        public const string CreateLicense = "License.Create";
        public const string UpdateLicense = "License.Update";
        public const string DeleteLicense = "License.Delete";
        #endregion
        
        #region BundleRegion

        public const string CreateBundle = "Bundle.Create";
        public const string UpdateBundle = "Bundle.Update";
        public const string DeleteBundle = "Bundle.Delete";
        #endregion
        
        #region AppRegion
        public const string CreateApp = "App.Create";
        public const string UpdateApp = "App.Update";
        public const string DeleteApp = "App.Delete";
        #endregion
        
        #region AccountRegion
        public const string CreateAccount = "Account.Create";
        public const string UpdateAccount = "Account.Update";
        public const string DeleteAccount = "Account.Delete";
        #endregion
        
        #region SoftwareRegion
        public const string CreateSoftware = "Software.Create";
        public const string UpdateSoftware = "Software.Update";
        public const string DeleteSoftware = "Software.Delete";
        #endregion
    }
}