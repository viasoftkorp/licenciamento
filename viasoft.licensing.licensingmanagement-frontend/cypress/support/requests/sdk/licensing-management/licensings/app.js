export const getAllAppsRequest = {
  url: '/Licensing/LicensingManagement/App/GetAll?**',
  method: 'GET',
  response: {
    totalCount: 2,
    items:[{
      domain: 15,
      id: '25',
      identifier: '00-ITEM-TESTE',
      isActive: true,
      isDefault: false,
      name: 'App de teste',
      operationValidation: 0,
      operationValidationDescription: 'NoError',
      softwareId: '456',
      softwareName: 'Viasoft',
    },
    {
      domain: 15,
      id: '30',
      identifier: '01-ITEM-TESTE',
      isActive: true,
      isDefault: false,
      name: 'New',
      operationValidation: 0,
      operationValidationDescription: 'NoError',
      softwareId: '111',
      softwareName: 'Viasoft',
    }]
  }
}

export const getSearchedApp = {
  url: '/Licensing/LicensingManagement/App/GetAll?**new**',
  method: 'GET',
  response: {
    totalCount: 2,
    items:[{
      domain: 15,
      id: '30',
      identifier: '01-ITEM-TESTE',
      isActive: true,
      isDefault: false,
      name: 'New',
      operationValidation: 0,
      operationValidationDescription: 'NoError',
      softwareId: '111',
      softwareName: 'Viasoft',
    }]
  }
}