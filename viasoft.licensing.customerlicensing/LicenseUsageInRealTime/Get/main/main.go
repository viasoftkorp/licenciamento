package main

import (
	"LicenseUsageInRealTime/consts"
	"LicenseUsageInRealTime/services"
	"encoding/json"
	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-lambda-go/lambda"
	"log"
)

func Handler(request events.APIGatewayProxyRequest) (events.APIGatewayProxyResponse, error) {

	var tenantId = request.QueryStringParameters[consts.TenantIdQueryParameter]
	var tenantIdValidation = services.ValidateTenant(tenantId)
	if tenantIdValidation != "" {
		log.Printf(tenantIdValidation)
		return events.APIGatewayProxyResponse{
			Body:       tenantIdValidation,
			StatusCode: 400,
		}, nil
	}

	var licensesUsageInRealTime = services.GetLicensesUsageInRealTime(tenantId)

	if licensesUsageInRealTime == nil {
		return events.APIGatewayProxyResponse{
			Body:       "[]",
			StatusCode: 200,
		}, nil
	}

	var jsonAsBytes, err = json.Marshal(licensesUsageInRealTime)
	if err != nil {
		log.Printf(err.Error())
		return events.APIGatewayProxyResponse{StatusCode: 400, Body: err.Error()}, nil
	}

	var jsonAsString = string(jsonAsBytes)

	return events.APIGatewayProxyResponse{
		Body:       jsonAsString,
		StatusCode: 200,
	}, nil
}

func main() {
	lambda.Start(Handler)
}
