package services

import (
	"LicenseUsageInRealTime/classes"
	"LicenseUsageInRealTime/consts"
	"LicenseUsageInRealTime/extensions"
	"context"
	uuid "github.com/satori/go.uuid"
	"log"
	"strconv"
	"strings"
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
)

func RemoveLicensesUsageInRealTime(tenantId string) int {
	var removedCount = 0
	UsingCollection(consts.LicensingDatabaseName, consts.LicenseUsageInRealTimeCollectionName, func(collection *mongo.Collection) {

		filter := bson.M{consts.TenantIdColumnName: strings.ToUpper(tenantId)}
		deleteResult, err := collection.DeleteMany(context.TODO(), filter)
		if err != nil {
			log.Fatal(err)
		}
		removedCount = int(deleteResult.DeletedCount)
	})
	return removedCount
}

func InsertLicensesUsageInRealTime(licenseUsageBehaviourDetails []classes.LicenseUsageInRealTimeDetails) int {
	if len(licenseUsageBehaviourDetails) == 0 {
		return 0
	}

	var lastUpdate = time.Now().UTC()
	for i := 0; i < len(licenseUsageBehaviourDetails); i++ {
		var usageDetails *classes.LicenseUsageInRealTimeDetails
		usageDetails = &licenseUsageBehaviourDetails[i]
		usageDetails.TenantId = strings.ToUpper(usageDetails.TenantId)
		usageDetails.LastUpdate = lastUpdate
		usageDetails.AdditionalLicensesAvailable = usageDetails.AdditionalLicenses - usageDetails.AdditionalLicensesConsumed
		usageDetails.AppLicensesAvailable = usageDetails.AppLicenses - usageDetails.AppLicensesConsumed
	}

	var insertedCount = 0
	listAsInterface := extensions.TransformListToInterface(licenseUsageBehaviourDetails)
	UsingCollection(consts.LicensingDatabaseName, consts.LicenseUsageInRealTimeCollectionName, func(collection *mongo.Collection) {

		insertManyResult, err := collection.InsertMany(context.TODO(), listAsInterface)
		if err != nil {
			log.Fatal(err)
		}
		insertedCount = len(insertManyResult.InsertedIDs)
	})

	return insertedCount
}

func ValidateLicensesUsageInRealTimeInput(licenseUsageInRealTime classes.LicenseUsageInRealTime) string {

	var tenantIdValidation = ValidateTenant(licenseUsageInRealTime.TenantId)
	if tenantIdValidation != "" {
		return tenantIdValidation
	}

	for index, value := range licenseUsageInRealTime.LicenseUsageInRealTimeDetails {
		tenantIdValidation = ValidateTenant(value.TenantId)
		if tenantIdValidation != "" {
			return tenantIdValidation + " Object index " + strconv.Itoa(index) + "."
		}
	}
	return ""
}

func ValidateTenant(tenantId string) string {
	if tenantId == "" || tenantId == consts.GuidEmpty {
		return "TenantId cannot be empty."
	} else {
		var _, err = uuid.FromString(tenantId)
		if err != nil {
			return err.Error() + "."
		}
	}
	return ""
}

func GetLicensesUsageInRealTime(tenantId string) []*classes.LicenseUsageInRealTimeDetails {
	var results []*classes.LicenseUsageInRealTimeDetails

	UsingCollection(consts.LicensingDatabaseName, consts.LicenseUsageInRealTimeCollectionName, func(collection *mongo.Collection) {
		filter := bson.M{consts.TenantIdColumnName: strings.ToUpper(tenantId)}
		var cursor, err = collection.Find(context.TODO(), filter)
		if err != nil {
			log.Fatal(err)
		}

		for cursor.Next(context.TODO()) {
			var elem classes.LicenseUsageInRealTimeDetails
			var err = cursor.Decode(&elem)
			if err != nil {
				log.Fatal(err)
			}
			results = append(results, &elem)
		}

		if err := cursor.Err(); err != nil {
			log.Fatal(err)
		}
		_ = cursor.Close(context.TODO())
	})
	return results
}
