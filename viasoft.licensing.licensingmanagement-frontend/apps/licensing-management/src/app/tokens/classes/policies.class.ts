export class Policies {
  // #region License
  static CreateLicense = 'License.Create';
  static UpdateLicense = 'License.Update';
  static DeleteLicense = 'License.Delete';
  // #endregion  

  // #region Bundle
  static CreateBundle = "Bundle.Create";
  static UpdateBundle = "Bundle.Update";
  static DeleteBundle = "Bundle.Delete";
  // #endregion  

  // #region App
  static CreateApp = "App.Create";
  static UpdateApp = "App.Update";
  static DeleteApp = "App.Delete";
  // #endregion 
  
  // #region Account
  static CreateAccount = "Account.Create";
  static UpdateAccount = "Account.Update";
  static DeleteAccount = "Account.Delete";
  // #endregion 

  // #region Software
  static CreateSoftware = "Software.Create";
  static UpdateSoftware = "Software.Update";
  static DeleteSoftware = "Software.Delete";
  // #endregion 

  // #region Batch Operations
  static BatchOperationInsertAppInLicenses = "BatchOperation.InsertAppInLicenses";
  static BatchOperationRemoveAppInLicenses = "BatchOperation.RemoveAppInLicenses";
  static BatchOperationInsertBundlesInLicenses = "BatchOperation.InsertBundlesInLicenses";
  static BatchOperationInsertAppsInLicenses = "BatchOperation.InsertAppsInLicenses";
  static BatchOperationInsertAppsInBundles = "BatchOperation.InsertAppsInBundles";
  // #endregion
}