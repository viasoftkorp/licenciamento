export const GetTenantInfoFromId: CypressRequest = {
    url: 'Licensing/CustomerLicensing/LicenseUsageInRealTime/GetTenantInfoFromId?tenantId=**',
    method: 'GET',
    response: {
        licensingStatus: 3,
        cnpj: "03623045000100",
        operationValidation: 0,
        operationValidationDescription:"NoError",
        licensedTenantId: '1BA7A91F-80C0-4975-842F-957A76504CE6'
    }
}