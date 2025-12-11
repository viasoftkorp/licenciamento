export type AuditingDataType = '0' | '1' | '2' | '3' | '4' | '5'

export const AuditingDataType = {
  0: 'InsertAppInLicenses' as AuditingDataType,
  1: 'RemoveAppInLicenses' as AuditingDataType,
  2: 'InsertMultipleBundlesInMultipleLicenses' as AuditingDataType,
  3: 'InsertMultipleAppsInMultipleLicenses' as AuditingDataType ,
  4: 'InsertMultipleAppsInMultipleBundles' as AuditingDataType,
  5: 'InsertMultipleAppsFromBundlesInLicenses as AuditingDataType' as AuditingDataType
}
