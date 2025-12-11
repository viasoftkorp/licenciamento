export const getAllProducts = {
    url: 'licensing/licensing-management/licenses/**/products?**',
    method: 'GET',
    response: {
        totalCount:2,
        items:[
           {
              id:"158d9499-f1bf-4695-930a-08d99ecfe953",
              name:"Pacote Default",
              identifier:"Default",
              isActive:true,
              softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
              softwareName:"Sistema web",
              numberOfLicenses:1,
              licensingModel:1,
              licensingMode:0,
              status:1,
              numberOfUsedLicenses:0,
              productType:0,
              productId:"f82c6efb-399b-46ca-8f3e-5dd35f2f7bda"
           },
           {
            id:"46c663ca-73f2-44e3-80c0-08d99ad24be8",
            name:"Administração",
            identifier:"ADM01",
            isActive:true,
            softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
            softwareName:"Sistema web",
            numberOfLicenses:2,
            licensingModel:0,
            licensingMode:null,
            status:1,
            numberOfUsedLicenses:0,
            productType:1,
            productId:"41904eda-ff7c-467c-93a4-0ac6b07d2e4b"
         }
        ]
     }
}

export const getAllProductsEmpty = {
    url: 'licensing/licensing-management/licenses/**/products?**',
    method: 'GET',
    response: {
        totalCount:0,
        items:[]
     }
}

export const getAllProductsAfterDeleteApp = {
    url: 'licensing/licensing-management/licenses/**/products?**',
    method: 'GET',
    response: {
        totalCount:2,
        items:[
           {
              id:"158d9499-f1bf-4695-930a-08d99ecfe953",
              name:"Pacote Default",
              identifier:"Default",
              isActive:true,
              softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
              softwareName:"Sistema web",
              numberOfLicenses:1,
              licensingModel:1,
              licensingMode:0,
              status:1,
              numberOfUsedLicenses:0,
              productType:0,
              productId:"f82c6efb-399b-46ca-8f3e-5dd35f2f7bda"
           }
        ]
     }
}


export const getAllProductsAfterInsertApp = {
    url: 'licensing/licensing-management/licenses/**/products?**',
    method: 'GET',
    response: {
        totalCount:2,
        items:[
           {
              id:"158d9499-f1bf-4695-930a-08d99ecfe953",
              name:"Pacote Default",
              identifier:"Default",
              isActive:true,
              softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
              softwareName:"Sistema web",
              numberOfLicenses:1,
              licensingModel:1,
              licensingMode:0,
              status:1,
              numberOfUsedLicenses:0,
              productType:0,
              productId:"f82c6efb-399b-46ca-8f3e-5dd35f2f7bda"
           },
           {
            id:"46c663ca-73f2-44e3-80c0-08d99ad24be8",
            name:"Administração",
            identifier:"ADM01",
            isActive:true,
            softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
            softwareName:"Sistema web",
            numberOfLicenses:2,
            licensingModel:0,
            licensingMode:null,
            status:1,
            numberOfUsedLicenses:0,
            productType:1,
            productId:"41904eda-ff7c-467c-93a4-0ac6b07d2e4b"
            },
           {
            id: '30',
            name: 'New',
            numberOfLicenses: 15,
            softwareId: '111',
            softwareName: 'Viasoft',
            status: 1,
            identifier:"New",
            isActive:true,
            softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
            softwareName:"Sistema web",
            numberOfLicenses:1,
            licensingModel:1,
            licensingMode:0,
            numberOfUsedLicenses:0,
            productType:1,
            productId:"f82c6efb-399b-46ca-8f3e-5dd35f2f7bda",
         }
        ]
     }
}

export const getAllProductsAfterDeleteProduct = {
    url: 'licensing/licensing-management/licenses/**/products?**',
    method: 'GET',
    response: {
        totalCount:2,
        items:[
           {
            id:"46c663ca-73f2-44e3-80c0-08d99ad24be8",
            name:"Administração",
            identifier:"ADM01",
            isActive:true,
            softwareId:"c4287bd6-7422-45cc-9cde-35d59e7f551c",
            softwareName:"Sistema web",
            numberOfLicenses:2,
            licensingModel:0,
            licensingMode:null,
            status:1,
            numberOfUsedLicenses:0,
            productType:1,
            productId:"41904eda-ff7c-467c-93a4-0ac6b07d2e4b"
         }
        ]
     }
}
