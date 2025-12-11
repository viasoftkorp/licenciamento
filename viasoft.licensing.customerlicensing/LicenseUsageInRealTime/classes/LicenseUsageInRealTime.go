package classes

type LicenseUsageInRealTime struct {
	TenantId                      string                          `json:"tenantid" bson:"tenantid"`
	LicenseUsageInRealTimeDetails []LicenseUsageInRealTimeDetails `json:"licenseusageinrealtimedetails" bson:"licenseusageinrealtimedetails"`
}
