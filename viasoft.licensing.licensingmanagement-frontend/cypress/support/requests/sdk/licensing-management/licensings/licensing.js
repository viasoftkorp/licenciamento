export const createLicense = {
  url: '/Licensing/LicensingManagement/Licensing/Create',
  method: 'POST',
  response: {
    id: '1',
    accountId: '2',
    status: 3,
    identifier: '107071d6-5d3d-a8a9-9fcb-471c16f7605d',
    expirationDateTime: '2020-08-01',
    notes: 'Testando',
    administratorEmail: 'contato@maribebidas.com.br',
    licensedCnpjs: '98483050000171',
    operationValidation: 0,
    operationValidationDescription: 'NoError'
  }
}

export const createLicenseWithInvalidAdministratorEmail = {
  url: '/Licensing/LicensingManagement/Licensing/Create',
  method: 'POST',
  response: {
    identifier: '4F318797-0C15-4C3C-A3F4-0910842574F0',
    operationValidation: 11,
    operationValidationDescription: 'AdministrationEmailAlreadyInUse'
  }
}

export const createLicenseWithInvalidAccount = {
  url: '/Licensing/LicensingManagement/Licensing/Create',
  method: 'POST',
  response: {
    identifier: '4F318797-0C15-4C3C-A3F4-0910842574F0',
    operationValidation: 9,
    operationValidationDescription: 'AccountIdAlreadyInUse'
  }
}

export const createLicenseWithInvalidIdentifier = {
  url: '/Licensing/LicensingManagement/Licensing/Create',
  method: 'POST',
  response: {
    operationValidation: 2,
    operationValidationDescription: 'DuplicatedIdentifier'
  }
}

export const getLicenseById = {
  url: '/Licensing/LicensingManagement/Licensing/GetById?id=1',
  method: 'GET',
  response: {
    id: '1',
    accountId: '2',
    accountName: 'Mariana Comercio de Bebidas Ltda',
    status: 3,
    identifier: '107071d6-5d3d-a8a9-9fcb-471c16f7605d',
    expirationDateTime: '2050-12-01',
    licensedCnpjs: '98483050000171',
    administratorEmail: 'contato@maribebidas.com.br',
    operationValidation: 0,
    operationValidationDescription: 'NoError',
    notes: 'Testando',
    licenseConsumeType: 0,
    licenseConsumeDescription: 'Connection',
    sagaInfo: {
      amCreatingNewLicensedTenant: true,
      errorDetails: null,
      status: 2
    }
  }
}

export const getInfrastructureConfigurations = {
  url: '/Licensing/LicensingManagement/InfrastructureConfiguration/GetByTenantId?**',
  method: 'GET',
  response: {
    desktopDatabaseName: "kokaodkasoLLL",
    errors: null,
    gatewayAddress: "192.168.0.100:9999",
    licensedTenantId: "df266e58-0d29-b739-292f-9faad083752b",
    success: false
  }
}

export const getAllLooseLicensedApps = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLooseLicensedApps?**',
  method: 'GET',
  response: {
    totalCount: 2,
    items: [{
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: null,
      id: '55fd4830-256d-df92-68f8-3a20445d10c5',
      name: 'Personalização da Base de Dados Viasoft',
      numberOfLicenses: 2147483647,
      numberOfTemporaryLicenses: 0,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    },
    {
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: null,
      id: 'ce448604-2409-a223-ceda-4365629bd447',
      name: 'Viasoft Gerente',
      numberOfLicenses: 2147483647,
      numberOfTemporaryLicenses: 0,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    }]
  }
}

var currentDate = new Date('2020-06-10 00:00');
var fullDate = currentDate.getFullYear() + '-' + (currentDate.getMonth() + 1) + '-' + currentDate.getDate();

export const getAllLooseLicensedAppsUpdated = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLooseLicensedApps?**',
  method: 'GET',
  response: {
    totalCount: 2,
    items: [{
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: fullDate,
      id: '55fd4830-256d-df92-68f8-3a20445d10c5',
      name: 'Personalização da Base de Dados Viasoft',
      numberOfLicenses: 20,
      numberOfTemporaryLicenses: 5,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    },
    {
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: null,
      id: 'ce448604-2409-a223-ceda-4365629bd447',
      name: 'Viasoft Gerente',
      numberOfLicenses: 2147483647,
      numberOfTemporaryLicenses: 0,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    }]
  }
}

export const removeAppFromLicenseError = {
  url: '/Licensing/LicensingManagement/Licensing/RemoveAppFromLicense',
  method: 'POST',
  response: {
    success: false,
    errors: [{
      message: null,
      errorCode: 6,
      errorCodeReason: 'CantRemoveFromLicenseDefaultApp'
    }]
  }
}

export const removeAppFromLicenseSuccess = {
  url: '/Licensing/LicensingManagement/Licensing/RemoveAppFromLicense',
  method: 'POST',
  response: {
    success: true,
    errors: null
  }
}

export const getAllLicensedApps = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLicensedApps?LicensedTenantId=1',
  method: 'GET',
  response: {
    totalCount: 0,
    items: []
  }
}

export const addAppToLicense = {
  url: '/Licensing/LicensingManagement/Licensing/AddAppToLicense',
  method: 'POST',
  response: {
    licensedTenantId: '1',
    licensedBundleId: null,
    appId: '30',
    status: 1,
    numberOfLicenses: 15,
    additionalNumberOfLicenses: 0,
    numberOfTemporaryLicenses: 4,
    expirationDateOfTemporaryLicenses: fullDate,
    operationValidation: 0,
    operationValidationDescription: 'NoError',
  }
}

export const getAllLooseLicensedAppsAfterInsert = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLooseLicensedApps?**',
  method: 'GET',
  response: {
    totalCount: 3,
    items: [{
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: null,
      id: '55fd4830-256d-df92-68f8-3a20445d10c5',
      name: 'Personalização da Base de Dados Viasoft',
      numberOfLicenses: 2147483647,
      numberOfTemporaryLicenses: 0,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    },
    {
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: null,
      id: 'ce448604-2409-a223-ceda-4365629bd447',
      name: 'Viasoft Gerente',
      numberOfLicenses: 2147483647,
      numberOfTemporaryLicenses: 0,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    },
    {
      additionalNumberOfLicenses: 0,
      domain: 15,
      expirationDateOfTemporaryLicenses: fullDate,
      id: '30',
      name: 'New',
      numberOfLicenses: 15,
      numberOfTemporaryLicenses: 4,
      softwareId: '111',
      softwareName: 'Viasoft',
      status: 1
    }]
  }
}

export const updateLooseAppFromLicense = {
  url: '/Licensing/LicensingManagement/Licensing/UpdateLooseAppFromLicense',
  method: 'POST',
  response: {
    additionalNumberOfLicenses: 0,
    appId: '30',
    expirationDateOfTemporaryLicenses: fullDate,
    licensedBundleId: null,
    licensedTenantId: '1',
    numberOfLicenses: 50,
    numberOfTemporaryLicenses: 10,
    operationValidation: 0,
    operationValidationDescription: 'NoError',
    status: 1
  }
}

export const getAllLooseLicensedAppsAfterUpdate = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLooseLicensedApps?**',
  method: 'GET',
  response: {
    totalCount: 3,
    items: [{
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: null,
      id: '55fd4830-256d-df92-68f8-3a20445d10c5',
      name: 'Personalização da Base de Dados Viasoft',
      numberOfLicenses: 2147483647,
      numberOfTemporaryLicenses: 0,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    },
    {
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: null,
      id: 'ce448604-2409-a223-ceda-4365629bd447',
      name: 'Viasoft Gerente',
      numberOfLicenses: 2147483647,
      numberOfTemporaryLicenses: 0,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    },
    {
      additionalNumberOfLicenses: 0,
      domain: 15,
      expirationDateOfTemporaryLicenses: fullDate,
      id: '30',
      name: 'New',
      numberOfLicenses: 50,
      numberOfTemporaryLicenses: 10,
      softwareId: '111',
      softwareName: 'Viasoft',
      status: 1
    }]
  }
}

export const getAllLooseLicensedAppsAfterDelete = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLooseLicensedApps?**',
  method: 'GET',
  response: {
    totalCount: 2,
    items: [{
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: null,
      id: '55fd4830-256d-df92-68f8-3a20445d10c5',
      name: 'Personalização da Base de Dados Viasoft',
      numberOfLicenses: 2147483647,
      numberOfTemporaryLicenses: 0,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    },
    {
      additionalNumberOfLicenses: 0,
      domain: 13,
      expirationDateOfTemporaryLicenses: null,
      id: 'ce448604-2409-a223-ceda-4365629bd447',
      name: 'Viasoft Gerente',
      numberOfLicenses: 2147483647,
      numberOfTemporaryLicenses: 0,
      softwareId: '52246dd6-30cc-47e4-a934-7158243c7c89',
      softwareName: 'VIASOFT - DEPARTAMENTO DE TECNOLOGIAS',
      status: 1
    }]
  }
}

export const addBundleToLicense = {
  url: '/Licensing/LicensingManagement/Licensing/AddBundleToLicense',
  method: 'POST',
  response: {
    licensedTenantId: '1',
    bundleId: 'xaea12',
    status: 1,
    numberOfLicenses: 100,
    numberOfTemporaryLicenses: 2,
    expirationDateOfTemporaryLicenses: fullDate,
  }
}

export const getAllLicensedAppsInBundle = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLicensedAppsInBundle?**',
  method: 'GET',
  response: {
    totalCount: 3,
    items: [{
      id: '767',
      name: 'First Bundle app',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web',
      bundleName: 'Three Bundle',
      status: 1,
      numberOfLicenses: 100,
      additionalNumberOfLicenses: null,
      licensedBundleId: 'xaea12',
      domain: 14
    },
    {
      id: '233',
      name: 'Second Bundle app',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web',
      bundleName: 'Three Bundle',
      status: 1,
      numberOfLicenses: 100,
      additionalNumberOfLicenses: null,
      licensedBundleId: 'xaea12',
      domain: 12
    },
    {
      id: '983',
      name: 'Third Bundle app',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web',
      bundleName: 'Three Bundle',
      status: 1,
      numberOfLicenses: 100,
      additionalNumberOfLicenses: null,
      licensedBundleId: 'xaea12',
      domain: 14
    }]
  }
}

export const updateBundledAppFromLicense = {
  url: '/Licensing/LicensingManagement/Licensing/UpdateBundledAppFromLicense',
  method: 'POST',
  response: {
    licensedTenantId: '1',
    LicensedBundleId: 'xaea12',
    AppId: '233',
    Status: 1,
    NumberOfLicenses: 100,
    NumberOfTemporaryLicenses: 2,
    additionalNumberOfLicenses: 6,
    ExpirationDateOfTemporaryLicenses: fullDate
  }
}

export const getAllLicensedAppsInBundleAfterUpdate = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLicensedAppsInBundle?**',
  method: 'GET',
  response: {
    totalCount: 3,
    items: [{
      id: '767',
      name: 'First Bundle app',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web',
      bundleName: 'Three Bundle',
      status: 1,
      numberOfLicenses: 100,
      additionalNumberOfLicenses: null,
      licensedBundleId: 'xaea12',
      domain: 14
    },
    {
      id: '233',
      name: 'Second Bundle app',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web',
      bundleName: 'Three Bundle',
      status: 1,
      numberOfLicenses: 100,
      additionalNumberOfLicenses: 6,
      licensedBundleId: 'xaea12',
      domain: 12
    },
    {
      id: '983',
      name: 'Third Bundle app',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web',
      bundleName: 'Three Bundle',
      status: 1,
      numberOfLicenses: 100,
      additionalNumberOfLicenses: null,
      licensedBundleId: 'xaea12',
      domain: 14
    }]
  }
}

export const getAllLicensedAppsInBundleAfterDelete = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLicensedAppsInBundle?**',
  method: 'GET',
  response: {
    totalCount: 2,
    items: [{
      id: '767',
      name: 'First Bundle app',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web',
      bundleName: 'Three Bundle',
      status: 1,
      numberOfLicenses: 100,
      additionalNumberOfLicenses: null,
      licensedBundleId: 'xaea12',
      domain: 14
    },
    {
      id: '983',
      name: 'Third Bundle app',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web',
      bundleName: 'Three Bundle',
      status: 1,
      numberOfLicenses: 100,
      additionalNumberOfLicenses: null,
      licensedBundleId: 'xaea12',
      domain: 14
    }]
  }
}

export const removeBundleFromLicenseError = {
  url: '/Licensing/LicensingManagement/Licensing/RemoveBundleFromLicense',
  method: 'POST',
  response: {
    licensedTenantId: '1',
    bundleId: 'xaea12',
    success: false,
    errors: [{
      message: null,
      errorCode: 3,
      errorCodeReason: 'UsedByOtherRegister'
    }]
  }
}

export const removeBundleFromLicenseSuccess = {
  url: '/Licensing/LicensingManagement/Licensing/RemoveBundleFromLicense',
  method: 'POST',
  response: {
    licensedTenantId: '1',
    bundleId: 'xaea12',
    success: true,
    errors: null
  }
}

export const getAllLicensedAppsInBundleEmpty = {
  url: '/Licensing/LicensingManagement/Licensing/GetAllLicensedAppsInBundle?**',
  method: 'GET',
  response: {
    totalCount: 0,
    items: []
  }
}

export const updateLicense = {
  url: '/Licensing/LicensingManagement/Licensing/Update',
  method: 'POST',
  response: {
    id: '1',
    accountId: '2',
    administratorEmail: 'admin@maribebidas.com.br',
    expirationDateTime: '2020-12-25',
    identifier: '107071d6-5d3d-a8a9-9fcb-471c16f7605d',
    licensedCnpjs: '98483050000171',
    status: 1,
    notes: 'Observações Editadas'
  }
}

export const fileTenantQuota = {
  url: '/Licensing/LicensingManagement/TenantQuota/GetTenantQuota?licenseTenantId=1',
  method: 'GET',
  response: {
    id: '123',
    licenseTenantIdentifier: '1',
    quotaLimit: 1024
  }
}

export const getAllLicensedAppsForFileQuota = {
  url: '/Licensing/LicensingManagement/AppQuota/GetAll?LicensedTenantId=1&SkipCount=0&MaxResultCount=25',
  method: 'GET',
  response: {
    totalCount: 3,
    items: [{
      id: '767',
      appName: 'First Bundle app',
      appId: '1aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      licensedTenantId: '2aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      quotaLimit: 1024
    },
    {
      id: '222',
      appName: 'Second Bundle app',
      appId: '2aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      licensedTenantId: '3aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      quotaLimit: 20000
    },
    {
      id: '333',
      appName: 'Last Bundle app',
      appId: '3ba843d7-c2bc-4123-86f1-0d5eec0f7351',
      licensedTenantId: '4ca843d7-c2bc-4123-86f1-0d5eec0f7351',
      quotaLimit: 0
    }]
  }
}

export const fileAppQuota = {
  url: '/Licensing/LicensingManagement/AppQuota/GetAppQuota?licensedTenantId=1&appId=1aa843d7-c2bc-4123-86f1-0d5eec0f7351',
  method: 'GET',
  response: {
    id: '1234',
    licensedTenantId: '1',
    appId: '767',
    appName: 'AppName',
    quotaLimit: 1024
  }
}

export const getOrganizationUnitRequest = {
  url: '/Licensing/LicensingManagement/licensed-tenant/107071d6-5d3d-a8a9-9fcb-471c16f7605d/organization-units?OrganizationId=1&SkipCount=0&MaxResultCount=25',
  method: 'GET',
  response: { "totalCount": 3, "items": [{ "name": "Teste", "description": "Unidade destinada para testes", "organizationId": "1", "isActive": true, "id": "9c99df32-35a8-4a72-8388-08d8a29dcfc0" }, { "name": "Desktop", "description": "", "organizationId": "1", "isActive": true, "id": "5077c102-8da2-4a53-2d54-08d8a2a8c282" }, { "name": "xaxada", "description": "xaxada", "organizationId": "1", "isActive": true, "id": "bf84a7a8-60e7-40c1-2f1a-08d8a2bc1e92" }] }
}
export const editOrganizationUnitRequest = {
  url: '/Licensing/LicensingManagement/licensed-tenant/107071d6-5d3d-a8a9-9fcb-471c16f7605d/organization-units/9c99df32-35a8-4a72-8388-08d8a29dcfc0',
  method: 'GET',
  response: { "name": "Teste", "description": "Unidade destinada para testes", "organizationId": "1", "isActive": true, "id": "9c99df32-35a8-4a72-8388-08d8a29dcfc0" }
}

export const getOrganizationEnvironmentRequest = {
  url: '/Licensing/LicensingManagement/licensed-tenant/107071d6-5d3d-a8a9-9fcb-471c16f7605d/organization-units/9c99df32-35a8-4a72-8388-08d8a29dcfc0/organization-environments?UnitId=9c99df32-35a8-4a72-8388-08d8a29dcfc0&ActiveOnly=false&SkipCount=0&MaxResultCount=25',
  method: 'GET',
  response: { "totalCount": 2, "items": [{ "name": "a1", "description": "d1", "isProduction": false, "isMobile": false, "isWeb": true, "isDesktop": false, "databaseName": "", "organizationUnitId": "9c99df32-35a8-4a72-8388-08d8a29dcfc0", "isActive": true, "id": "e7d2edc2-97f3-425d-84d0-08d8a2a5e4c6" }, { "name": "xxx", "description": "", "isProduction": false, "isMobile": false, "isWeb": false, "isDesktop": true, "databaseName": "xxx", "organizationUnitId": "9c99df32-35a8-4a72-8388-08d8a29dcfc0", "isActive": false, "id": "b571faff-a86d-46f0-a787-08d8a2a91f54" }] }
}
export const editOrganizationEnvironmentRequest = {
  url: '/Licensing/LicensingManagement/licensed-tenant/107071d6-5d3d-a8a9-9fcb-471c16f7605d/organization-environments/e7d2edc2-97f3-425d-84d0-08d8a2a5e4c6',
  method: 'GET',
  response: { "name": "a1", "description": "d1", "isProduction": false, "isMobile": false, "isWeb": true, "isDesktop": false, "databaseName": "", "organizationId": "27D2710D-8BBF-4348-853F-43475A795797", "organizationUnitId": "9c99df32-35a8-4a72-8388-08d8a29dcfc0", "isActive": true, "id": "e7d2edc2-97f3-425d-84d0-08d8a2a5e4c6" }
}