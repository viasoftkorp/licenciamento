export const GetAllProducts: CypressRequest = {
    url: `licensing/customer-licensing/licenses/**/products?**`,
    method: 'GET',
    response: {
      totalCount:1,
      items:[
            {
              id:"f82c6efb-399b-46ca-8f3e-5dd35f2f7bda",
              name:"Pacote Default",
              identifier:"Default",
              isActive:true,
              softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
              softwareName:"Sistema web",
              numberOfLicenses:50,
              licensingModel:0,
              licensingMode:null,
              licensedBundleId:"00000000-0000-0000-0000-000000000000",
              status:0,
              numberOfUsedLicenses:23,
              poroductType: 0
            },
            {
              id:"a083730b-2bcc-405b-b6bf-d261d321b03a",
              name:"Pacote Online",
              identifier:"ee19958d-5f6a-4201-b989-08d7468f1643",
              isActive:true,
              softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
              softwareName:"Sistema web",
              numberOfLicenses:25,
              licensingModel:1,
              licensingMode:0,
              licensedBundleId:"00000000-0000-0000-0000-000000000000",
              status:1,
              numberOfUsedLicenses:10,
              poroductType: 0
            },
            {
              id:"e8a44a0b-9d53-4fa1-bb84-dde635020ede",
              name:"Pacote Offline",
              identifier:"3802ac31-08c2-435a-8e47-7bf558a33345",
              isActive:true,
              softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
              softwareName:"Sistema web",
              numberOfLicenses:25,
              licensingModel:1,
              licensingMode:1,
              licensedBundleId:"00000000-0000-0000-0000-000000000000",
              status:1,
              numberOfUsedLicenses:10,
              poroductType: 0
            }
        ]
    }
}

export const GetProductFloatingById: CypressRequest = {
  url: 'licensing/customer-licensing/licenses/**/products/**',
  method: 'GET',
  response: {
    id:"f82c6efb-399b-46ca-8f3e-5dd35f2f7bda",
    name:"Pacote Default",
    identifier:"Default",
    isActive:true,
    softwareId:"00000000-0000-0000-0000-000000000000",
    softwareName:null,
    numberOfLicenses:20,
    licensingModel:0,
    licensingMode:null,
    licensedBundleId:"f05b29a2-4dd1-4466-fa6c-08d97866fc96",
    status:1,
    numberOfUsedLicenses:1,
    poroductType: 0
 }
}

export const GetProductNamedOnlineById: CypressRequest = {
  url: 'licensing/customer-licensing/licenses/**/products/**',
  method: 'GET',
  response: {
    id:"f82c6efb-399b-46ca-8f3e-5dd35f2f7bda",
    name:"Pacote Default",
    identifier:"Default",
    isActive:true,
    isCustom:false,
    softwareId:"00000000-0000-0000-0000-000000000000",
    softwareName:null,
    numberOfLicenses:20,
    numberOfTemporaryLicenses:0,
    expirationDateOfTemporaryLicenses:null,
    licensingModel:1,
    licensingMode:0,
    licensedBundleId:"f05b29a2-4dd1-4466-fa6c-08d97866fc96",
    status:1,
    numberOfUsedLicenses:2
 }
}

export const GetAllLicenseUsage: CypressRequest = {
  url: `licensing/customer-licensing/licenses/**/products/license-usage**`,
  method: 'GET',
  response: [
    {
      productIdentifier: "Default",
      appLicensesConsumed: 5
    }
  ]
}