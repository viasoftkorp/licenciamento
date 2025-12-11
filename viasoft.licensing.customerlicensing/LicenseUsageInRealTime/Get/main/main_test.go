package main

import (
	"github.com/aws/aws-lambda-go/events"
	"github.com/stretchr/testify/assert"
	"os"
	"testing"
)

func TestHandler(t *testing.T) {
	_ = os.Setenv("MONGODB_IP", "52.67.135.129")
	_ = os.Setenv("MONGODB_PORT", "8060")
	var tenantId = "16A7571E-7FF6-479E-A6F5-3514414179DD"

	var m = make(map[string]string)
	m["tenantid"] = tenantId

	var tests = []struct {
	request 			events.APIGatewayProxyRequest
	expectResult		events.APIGatewayProxyResponse
	}{
		{
			request: events.APIGatewayProxyRequest{
				HTTPMethod: "GET",
				RequestContext: events.APIGatewayProxyRequestContext{ RequestID: "1" },
				QueryStringParameters: m,
			},
			expectResult: events.APIGatewayProxyResponse{
				StatusCode: 200,
			},
		},
	}

	for _, test := range tests {
		response, _ := Handler(test.request)
		assert.Equal(t, test.expectResult.StatusCode, response.StatusCode)
		assert.NotEqual(t, "", response.Body)
	}

}
