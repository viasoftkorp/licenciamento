
export const getCompanyByCnpj = {
  url: '/Licensing/LicensingManagement/Cnpj/GetCompanyByCnpj?cnpj=55.335.198**',
  method: 'GET',
  response: {
    id: '1',
    phone: '4130378764',
    email: 'principal@farmac.com',
    neighborhood: 'Centro',
    efr: null,
    cnpj: '55335198000156',
    name: 'Farmac Inc.',
    tradingName: 'Farmac',
    status: 'OK',
    operationValidation: 0,
    operationValidationDescription: "NoError"
  }
};

export const getNewCompanyByCnpj = {
  url: '/Licensing/LicensingManagement/Cnpj/GetCompanyByCnpj?cnpj=98.483.050**',
  method: 'GET',
  response: {
    status: 'ERROR',
    operationValidation: 0,
    operationValidationDescription: "NoError"
  }
};

export const companyDoesNotExist = {
  url: '/Licensing/LicensingManagement/Cnpj/GetCompanyByCnpj?cnpj=00.000.000**',
  method: 'GET',
  response: {
    status: 'ERROR',
    operationValidation: 0,
    operationValidationDescription: "NoError"
  }
}

export const companyAlreadyRegistered = {
  url: '/Licensing/LicensingManagement/Cnpj/GetCompanyByCnpj?cnpj=55.335.198**',
  method: 'GET',
  response: {
    status: 'ERROR',
    name: 'Farmac Inc.',
    operationValidation: 8,
    operationValidationDescription: "CnpjAlreadyRegistered"
  }
}