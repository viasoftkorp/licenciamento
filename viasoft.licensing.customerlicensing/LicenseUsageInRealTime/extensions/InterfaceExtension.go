package extensions

import (
	"LicenseUsageInRealTime/classes"
)

func TransformListToInterface(licenseUsageBehaviourDetails []classes.LicenseUsageInRealTimeDetails) []interface{} {
	s := make([]interface{}, len(licenseUsageBehaviourDetails))
	for i, v := range licenseUsageBehaviourDetails {
		s[i] = v
	}
	return s
}
