export const getAllLicensedBundleEmpty = {
  url: '/Licensing/LicensingManagement/Bundle/GetAllLicensedBundle?**',
  method: 'GET',
  response: {
    totalCount: 0,
    items: []
  }
}

export const getAllBundlesMinusLicensedBundles = {
  url: '/Licensing/LicensingManagement/Bundle/GetAllBundlesMinusLicensedBundles?**',
  method: 'GET',
  response: {
    totalCount: 3,
    items: [{
      id: 'je7f',
      identifier: 'oneBundle',
      isActive: true,
      isCustom: false,
      name: 'One Bundle',
      operationValidation: 0,
      operationValidationDescription: 'NoError',
      softwareId: '75a91e16-0b6d-a0b8-8cda-47ecac8dceb0',
      softwareName: 'Sistema Desktop'
    },
    {
      id: '8ghp',
      identifier: 'twoBundle',
      isActive: true,
      isCustom: false,
      name: 'Two Bundle',
      operationValidation: 0,
      operationValidationDescription: 'NoError',
      softwareId: '75a91e16-0b6d-a0b8-8cda-47ecac8dceb0',
      softwareName: 'Sistema Desktop'
    },
    {
      id: 'xaea12',
      identifier: 'threeBundle',
      isActive: true,
      isCustom: false,
      name: 'Three Bundle',
      operationValidation: 0,
      operationValidationDescription: 'NoError',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web'
    }]
  }
}

export const getSearchedBundle = {
  url: '/Licensing/LicensingManagement/Bundle/GetAllBundlesMinusLicensedBundles?**Three**',
  method: 'GET',
  response: {
    totalCount: 1,
    items: [{
      id: 'xaea12',
      identifier: 'threeBundle',
      isActive: true,
      isCustom: false,
      name: 'Three Bundle',
      operationValidation: 0,
      operationValidationDescription: 'NoError',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web'
    }]
  }
}

var currentDate = new Date('2020-06-10 00:00');
var fullDate = currentDate.getFullYear() + '-' + (currentDate.getMonth()+1) + '-' + currentDate.getDate();

export const getAllLicensedBundleAfterInsert = {
  url: '/Licensing/LicensingManagement/Bundle/GetAllLicensedBundle?**',
  method: 'GET',
  response: {
    totalCount: 1,
    items: [{
      id: 'xaea12',
      identifier: 'threeBundle',
      isActive: true,
      isCustom: false,
      name: 'Three Bundle',
      operationValidation: 0,
      operationValidationDescription: 'NoError',
      softwareId: '0aa843d7-c2bc-4123-86f1-0d5eec0f7351',
      softwareName: 'Sistema Web',
      numberOfLicenses: 100,
      numberOfTemporaryLicenses: 2,
      expirationDateOfTemporaryLicenses: fullDate,
      licensingModel: 0,
      licensingMode: null
    }]
  }
}