package classes

import (
	"time"
)

type LicenseUsageInRealTimeDetails struct {
	TenantId                    string    `json:"tenantid" bson:"tenantid"`
	AppName                     string    `json:"appname" bson:"appname"`
	SoftwareName                string    `json:"softwarename" bson:"softwarename"`
	BundleName                  string    `json:"bundlename" bson:"bundlename"`
	User                        string    `json:"user" bson:"user"`
	AppIdentifier               string    `json:"appidentifier" bson:"appidentifier"`
	SoftwareIdentifier          string    `json:"softwareidentifier" bson:"softwareidentifier"`
	BundleIdentifier            string    `json:"bundleidentifier" bson:"bundleidentifier"`
	AdditionalLicense           bool      `json:"additionallicense" bson:"additionallicense"`
	StartTime                   time.Time `json:"starttime" bson:"starttime"`
	AppLicenses                 int       `json:"applicenses" bson:"applicenses"`
	AppLicensesConsumed         int       `json:"applicensesconsumed" bson:"applicensesconsumed"`
	AppLicensesAvailable        int       `json:"applicensesavailable" bson:"applicensesavailable"`
	AdditionalLicenses          int       `json:"additionallicenses" bson:"additionallicenses"`
	AdditionalLicensesConsumed  int       `json:"additionallicensesconsumed" bson:"additionallicensesconsumed"`
	AdditionalLicensesAvailable int       `json:"additionallicensesavailable" bson:"additionallicensesavailable"`
	LastUpdate                  time.Time `json:"lastupdate" bson:"lastupdate"`
	Status                      int       `json:"status" bson"status"`
	StatusDescription           string    `json:"statusdescription" bson"statusdescription"`
	Cnpj                        string    `json:"cnpj" bson"cnpj"`
}
