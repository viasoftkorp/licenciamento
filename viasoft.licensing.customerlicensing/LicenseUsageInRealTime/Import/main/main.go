package main

import (
	"LicenseUsageInRealTime/classes"
	"LicenseUsageInRealTime/services"
	"encoding/json"
	"fmt"
	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-lambda-go/lambda"
	"log"
)

func Handler(request events.APIGatewayProxyRequest) (events.APIGatewayProxyResponse, error) {
	if len(request.Body) < 1 {
		return events.APIGatewayProxyResponse{StatusCode: 400, Body: "Body not found."}, nil
	}

	var input classes.LicenseUsageInRealTime
	var err = json.Unmarshal([]byte(request.Body), &input)
	if err != nil {
		log.Printf(err.Error())
		return events.APIGatewayProxyResponse{StatusCode: 400, Body: err.Error()}, nil
	}

	var validationMessage = services.ValidateLicensesUsageInRealTimeInput(input)
	if validationMessage != "" {
		log.Printf(validationMessage)
		return events.APIGatewayProxyResponse{StatusCode: 400, Body: validationMessage}, nil
	}

	var removedCount = services.RemoveLicensesUsageInRealTime(input.TenantId)
	var insertedCount = services.InsertLicensesUsageInRealTime(input.LicenseUsageInRealTimeDetails)

	var output = fmt.Sprintf("LicenseUsageInRealTime: RequestId: '%s'. TenantId: '%s'. Removed count: %d. Inserted count: %d.",
		request.RequestContext.RequestID, input.TenantId, removedCount, insertedCount)

	return events.APIGatewayProxyResponse{StatusCode: 200, Body: output}, nil
}

func main() {
	lambda.Start(Handler)
}
