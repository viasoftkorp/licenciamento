package main

import (
	"LicenseUsageInRealTime/classes"
	"encoding/json"
	"fmt"
	"log"
	"os"
	"testing"
	"time"

	"github.com/aws/aws-lambda-go/events"
	"github.com/stretchr/testify/assert"
)

func TestHandler(t *testing.T) {
	_ = os.Setenv("MONGODB_IP", "52.67.135.129")
	_ = os.Setenv("MONGODB_PORT", "8060")
	var tenantId = "16A7571E-7FF6-479E-A6F5-3514414179DD"
	var test = "Test2"
	licenseUsageInRealTime := classes.LicenseUsageInRealTime{
		TenantId: tenantId,
		LicenseUsageInRealTimeDetails: []classes.LicenseUsageInRealTimeDetails{
			{
				TenantId:                    tenantId,
				User:                        test,
				AppIdentifier:               test,
				SoftwareIdentifier:          test,
				BundleIdentifier:            test,
				AdditionalLicense:           false,
				StartTime:                   time.Now().UTC(),
				AppLicenses:                 2,
				AppLicensesConsumed:         1,
				AppLicensesAvailable:        0,
				AdditionalLicenses:          0,
				AdditionalLicensesConsumed:  0,
				AdditionalLicensesAvailable: 0,
				LastUpdate:                  time.Time{},
				Status:                      0,
				StatusDescription:           "Active",
				Cnpj:                        "12345678901234",
			},
			{
				TenantId:                   tenantId,
				User:                       test,
				AppIdentifier:              test,
				SoftwareIdentifier:         test,
				BundleIdentifier:           "",
				AdditionalLicense:          true,
				StartTime:                  time.Now().UTC(),
				AppLicenses:                2,
				AppLicensesConsumed:        1,
				AdditionalLicenses:         0,
				AdditionalLicensesConsumed: 0,
				Status:                     1,
				StatusDescription:          "Trial",
				Cnpj:                       "12345678901234",
			},
		},
	}
	var jsonObject, err = json.Marshal(licenseUsageInRealTime)
	if err != nil {
		log.Fatal(err)
	}

	var tests = []struct {
		request      events.APIGatewayProxyRequest
		expectResult events.APIGatewayProxyResponse
	}{
		{
			request: events.APIGatewayProxyRequest{
				HTTPMethod:     "POST",
				Body:           string(jsonObject),
				RequestContext: events.APIGatewayProxyRequestContext{RequestID: "1"}},
			expectResult: events.APIGatewayProxyResponse{
				StatusCode: 200,
				Body:       fmt.Sprintf("LicenseUsageInRealTime: RequestId: '%s'. TenantId: '%s'. Removed count: %d. Inserted count: %d.", "1", tenantId, 2, 2),
			},
		},
		{
			request: events.APIGatewayProxyRequest{HTTPMethod: "POST", RequestContext: events.APIGatewayProxyRequestContext{RequestID: "3"}},
			expectResult: events.APIGatewayProxyResponse{
				StatusCode: 400,
				Body:       "Body not found.",
			},
		},
	}

	for _, test := range tests {
		response, _ := Handler(test.request)
		assert.Equal(t, test.expectResult.StatusCode, response.StatusCode)
		assert.Equal(t, test.expectResult.Body, response.Body)
	}

}
