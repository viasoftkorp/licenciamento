export const getAllRequest = {
  url: '/Licensing/LicensingManagement/Account/GetAll?**',
  method: 'GET',
  response: {
    totalCount: 1,
    items: [{
      id: '1',
      phone: '4130378764',
      email: 'principal@farmac.com',
      billingEmail: 'billing@farmac.com',
      tradingName: 'Farmac',
      companyName: 'Farmac Inc.',
      cnpjCpf: '55335198000156',
      status: 1,
      street: 'Avenida Marechal Floriano Peixoto',
      number: '180',
      detail: null,
      neighborhood: 'Centro',
      city: 'Curitiba',
      state: 'PR',
      country: 'Brasil',
      zipCode: '80010130',
      operationValidation: 'NoError',
      operationValidationDescription: null
    }]
  }
};

export const getExistingAccount = {
  url: '/Licensing/LicensingManagement/Account/GetById/1',
  method: 'GET',
  response: {
    id: '1',
    phone: '4130378764',
    email: 'principal@farmac.com',
    billingEmail: 'billing@farmac.com',
    tradingName: 'Farmac',
    companyName: 'Farmac Inc.',
    cnpjCpf: '55335198000156',
    status: 1,
    street: 'Avenida Marechal Floriano Peixoto',
    number: '180',
    detail: null,
    neighborhood: 'Centro',
    city: 'Curitiba',
    state: 'PR',
    country: 'Brasil',
    zipCode: '80010130',
    operationValidation: 'NoError',
    operationValidationDescription: null
  }
};


export const createAccountRequest = {
  url: '/Licensing/LicensingManagement/Account/Create',
  method: 'POST',
  response: {
    id: '2',
    phone: '4128402518',
    webSite: 'maribebidas.com.br',
    email: 'contato@maribebidas.com.br',
    billingEmail: 'contato@maribebidas.com.br',
    tradingName: 'Mari Bebidas',
    companyName: 'Mariana Comercio de Bebidas Ltda',
    cnpjCpf: '98483050000171',
    status: 1,
    street: 'Rua Visconde de Guarapuava',
    number: '220',
    detail: 'Lote 1',
    neighborhood: 'Dos Estados',
    city: 'Guarapuava',
    state: 'PR',
    country: 'Brasil',
    zipCode: '85035025'
  }
}

export const getAllRequestNew = {
  url: '/Licensing/LicensingManagement/Account/GetAll?**',
  method: 'GET',
  response: {
    totalCount: 2,
    items: [{
      id: '1',
      phone: '4130378764',
      email: 'principal@farmac.com',
      billingEmail: 'billing@farmac.com',
      tradingName: 'Farmac',
      companyName: 'Farmac Inc.',
      cnpjCpf: '55335198000156',
      status: 1,
      street: 'Avenida Marechal Floriano Peixoto',
      number: '180',
      detail: null,
      neighborhood: 'Centro',
      city: 'Curitiba',
      state: 'PR',
      country: 'Brasil',
      zipCode: '80010130',
      operationValidation: 'NoError',
      operationValidationDescription: null
    },
    {
      id: '2',
      phone: '4128402518',
      webSite: 'maribebidas.com.br',
      email: 'contato@maribebidas.com.br',
      billingEmail: 'contato@maribebidas.com.br',
      tradingName: 'Mari Bebidas',
      companyName: 'Mariana Comercio de Bebidas Ltda',
      cnpjCpf: '98483050000171',
      status: 1,
      street: 'Rua Visconde de Guarapuava',
      number: '220',
      detail: 'Lote 1',
      neighborhood: 'Dos Estados',
      city: 'Guarapuava',
      state: 'PR',
      country: 'Brasil',
      zipCode: '85035025',
      operationValidation: 'NoError',
      operationValidationDescription: null
    }]
  }
}

export const getAllRequestNewInverted = {
  url: '/Licensing/LicensingManagement/Account/GetAll?**',
  method: 'GET',
  response: {
    totalCount: 2,
    items: [{
      id: '2',
      phone: '4128402518',
      webSite: 'maribebidas.com.br',
      email: 'contato@maribebidas.com.br',
      billingEmail: 'contato@maribebidas.com.br',
      tradingName: 'Mari Bebidas',
      companyName: 'Mariana Comercio de Bebidas Ltda',
      cnpjCpf: '98483050000171',
      status: 1,
      street: 'Rua Visconde de Guarapuava',
      number: '220',
      detail: 'Lote 1',
      neighborhood: 'Dos Estados',
      city: 'Guarapuava',
      state: 'PR',
      country: 'Brasil',
      zipCode: '85035025',
      operationValidation: 'NoError',
      operationValidationDescription: null
    },
    {
      id: '1',
      phone: '4130378764',
      email: 'principal@farmac.com',
      billingEmail: 'billing@farmac.com',
      tradingName: 'Farmac',
      companyName: 'Farmac Inc.',
      cnpjCpf: '55335198000156',
      status: 1,
      street: 'Avenida Marechal Floriano Peixoto',
      number: '180',
      detail: null,
      neighborhood: 'Centro',
      city: 'Curitiba',
      state: 'PR',
      country: 'Brasil',
      zipCode: '80010130',
      operationValidation: 'NoError',
      operationValidationDescription: null
    }]
  }
}

export const updateAccountRequest = {
  url: '/Licensing/LicensingManagement/Account/Update',
  method: 'POST',
  response: {
    id: '1',
    phone: '4130378764',
    email: 'principal@farmac.com',
    billingEmail: 'financeiro@farmac.com',
    tradingName: 'Farmac',
    companyName: 'Farmac Inc.',
    cnpjCpf: '55335198000156',
    status: 1,
    street: 'Avenida Marechal Floriano Peixoto',
    number: '180',
    detail: null,
    neighborhood: 'Centro',
    city: 'Curitiba',
    state: 'PR',
    country: 'Brasil',
    zipCode: '80010130',
  }
}

const itemsSize = 150;
const items = [];
var generatedItem;

for (var i = 0; i < itemsSize; i++){
  generatedItem = {
    id: i,
    phone: '4199999999',
    email: 'contato@teste.com.br',
    billingEmail: 'financeiro@teste.com.br',
    tradingName: 'Comercio de Carnes',
    companyName: 'Comercio de Carnes LTDA.',
    cnpjCpf: '30397928000112',
    status: 1,
    operationValidation: 'NoError',
    operationValidationDescription: null
  }
  items.push(generatedItem);
}


export const randomAccountsRequest = {
  url: '/Licensing/LicensingManagement/Account/GetAll?SkipCount=0&MaxResultCount=25',
  method: 'GET',
  response: {
    totalCount: 25,
    items: items.slice(0, 24)
  }
}

export const randomAccountsRequest100Items = {
  url: '/Licensing/LicensingManagement/Account/GetAll?SkipCount=0&MaxResultCount=100',
  method: 'GET',
  response: {
    totalCount: itemsSize,
    items: items.slice(0, 99)
  }
}

export const deleteAccount = {
  url: '/Licensing/LicensingManagement/Account/Delete?**',
  method: 'DELETE',
  response: {
    success: true,
    errors: null
  }
}

export const getAllEmptyRequest = {
  url: '/Licensing/LicensingManagement/Account/GetAll?**',
  method: 'GET',
  response: {
    totalCount: 0,
    items: []
  }
}

export const getOnlyActiveAccounts = {
  url: '/Licensing/LicensingManagement/Account/GetAll?**',
  method: 'GET',
  response : {
    totalCount: 2,
    items: [{
      id: '1',
      phone: '4130378764',
      email: 'principal@farmac.com',
      billingEmail: 'billing@farmac.com',
      tradingName: 'Farmac',
      companyName: 'Farmac Inc.',
      cnpjCpf: '55335198000156',
      status: 1,
      street: 'Avenida Marechal Floriano Peixoto',
      number: '180',
      detail: null,
      neighborhood: 'Centro',
      city: 'Curitiba',
      state: 'PR',
      country: 'Brasil',
      zipCode: '80010130',
      operationValidation: 'NoError',
      operationValidationDescription: null
    },
    {
      id: '2',
      phone: '4128402518',
      webSite: 'maribebidas.com.br',
      email: 'contato@maribebidas.com.br',
      billingEmail: 'contato@maribebidas.com.br',
      tradingName: 'Mari Bebidas',
      companyName: 'Mariana Comercio de Bebidas Ltda',
      cnpjCpf: '98483050000171',
      status: 1,
      street: 'Rua Visconde de Guarapuava',
      number: '220',
      detail: 'Lote 1',
      neighborhood: 'Dos Estados',
      city: 'Guarapuava',
      state: 'PR',
      country: 'Brasil',
      zipCode: '85035025',
      operationValidation: 'NoError',
      operationValidationDescription: null
    }]
  }
}

export const getSearchedAccount = {
  url: '/Licensing/LicensingManagement/Account/GetAll?**Mari**',
  method: 'GET',
  response: {
    totalCount: 1,
    items: [{
      id: '2',
      phone: '4128402518',
      webSite: 'maribebidas.com.br',
      email: 'contato@maribebidas.com.br',
      billingEmail: 'contato@maribebidas.com.br',
      tradingName: 'Mari Bebidas',
      companyName: 'Mariana Comercio de Bebidas Ltda',
      cnpjCpf: '98483050000171',
      status: 1,
      street: 'Rua Visconde de Guarapuava',
      number: '220',
      detail: 'Lote 1',
      neighborhood: 'Dos Estados',
      city: 'Guarapuava',
      state: 'PR',
      country: 'Brasil',
      zipCode: '85035025',
      operationValidation: 'NoError',
      operationValidationDescription: null
    }]
  }
}



