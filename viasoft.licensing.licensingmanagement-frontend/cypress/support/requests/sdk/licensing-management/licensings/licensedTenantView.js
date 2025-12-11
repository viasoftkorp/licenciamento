export const getAllRequest = {
  url: '/Licensing/LicensingManagement/LicensedTenantView/GetAll?**',
  method: 'GET',
  response: {
    totalCount: 2,
    items: [{
      accountCompanyName: 'Farmac Inc.',
      accountId: '1',
      administratorEmail: 'admin@farmac.com',
      expirationDateTime: null,
      identifier: '2',
      licensedCnpjs: '55335198000156',
      licensedTenantId: '2',
      status: 3
    },
    {
      accountCompanyName: 'Random',
      accountId: '3',
      administratorEmail: 'admin@random.com',
      expirationDateTime: null,
      identifier: '3',
      licensedCnpjs: '74220135000107',
      licensedTenantId: '3',
      status: 1
    }]
  }
}

export const getAllRequestAfterInsert = {
  url: '/Licensing/LicensingManagement/LicensedTenantView/GetAll?**',
  method: 'GET',
  response: {
    totalCount: 3,
    items: [{
      accountCompanyName: 'Farmac Inc.',
      accountId: '1',
      administratorEmail: 'admin@farmac.com',
      expirationDateTime: null,
      identifier: '2',
      licensedCnpjs: '55335198000156',
      licensedTenantId: '2',
      status: 3
    },
    {
      accountCompanyName: 'Random',
      accountId: '3',
      administratorEmail: 'admin@random.com',
      expirationDateTime: null,
      identifier: '3',
      licensedCnpjs: '74220135000107',
      licensedTenantId: '3',
      status: 1
    },
    {
      accountCompanyName: 'Mariana Comercio de Bebidas Ltda',
      accountId: '2',
      administratorEmail: 'contato@maribebidas.com.br',
      expirationDateTime: '2020-08-01',
      identifier: '107071d6-5d3d-a8a9-9fcb-471c16f7605d',
      licensedCnpjs: '98483050000171',
      licensedTenantId: '1',
      status: 3
    }]
  }
}

export const getAllRequestAfterUpdate = {
  url: '/Licensing/LicensingManagement/LicensedTenantView/GetAll?**',
  method: 'GET',
  response: {
    totalCount: 3,
    items: [{
      accountCompanyName: 'Farmac Inc.',
      accountId: '1',
      administratorEmail: 'admin@farmac.com',
      expirationDateTime: null,
      identifier: '2',
      licensedCnpjs: '55335198000156',
      licensedTenantId: '2',
      status: 3
    },
    {
      accountCompanyName: 'Random',
      accountId: '3',
      administratorEmail: 'admin@random.com',
      expirationDateTime: null,
      identifier: '3',
      licensedCnpjs: '74220135000107',
      licensedTenantId: '3',
      status: 1
    },
    {
      accountCompanyName: 'Mariana Comercio de Bebidas Ltda',
      accountId: '2',
      administratorEmail: 'admin@maribebidas.com.br',
      expirationDateTime: '2020-12-25',
      identifier: '107071d6-5d3d-a8a9-9fcb-471c16f7605d',
      licensedCnpjs: '98483050000171',
      licensedTenantId: '1',
      status: 1
    }]
  }
}

