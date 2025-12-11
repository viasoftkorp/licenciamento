export const GetAllUsers: CypressRequest = {
    url: '/licensing/customer-licensing/licenses/ee19958d-5f6a-4201-b989-08d7468f1643/users?MaxResultCount=5',
    method: 'GET',
    response: {
        totalCount:2,
        items:[
           {
              firstName:"admin",
              secondName:"admin",
              login:"admin",
              urlImg:null,
              email:"admin@korp.com.br",
              phoneNumber:null,
              id:"4e6296a4-db80-464c-8957-9a380c79369b",
              isActive:false,
              creationTime:"2021-09-06T16:44:54.271443Z"
           },
           {
              firstName:"Admin",
              secondName:"User",
              login:"admin.user",
              urlImg:null,
              email:"admin.user@korp.com.br",
              phoneNumber:null,
              id:"3f117b7f-c5f3-4dd7-bb52-8f189f9cfe21",
              isActive:false,
              creationTime:"2021-09-06T16:46:40.530369Z"
           }
        ]
     }
}

export const AddNamedUserToProductSuccessfully: CypressRequest = {
    url: '/licensing/customer-licensing/licenses/**/products/**/named-user',
    method: 'POST',
    response: {
        id:"3ff0346d-53f7-4ca2-88b3-46168c6ff29f",
        tenantId:"ee19958d-5f6a-4201-b989-08d7468f1643",
        licensedTenantId:"7a160561-58fe-43f8-9708-6338e6ef6475",
        licensedBundleId:"004a1244-05f4-4e23-fa6d-08d97866fc96",
        namedUserId:"3f117b7f-c5f3-4dd7-bb52-8f189f9cfe21",
        namedUserEmail:"admin.user@korp.com.br",
        deviceId:"",
        operationValidation:0
     }
}

export const UpdateNamedUserFromProductSuccessfully: CypressRequest = {
    url: '/licensing/customer-licensing/licenses/**/products/**/named-user/**',
    method: 'PUT',
    response: {
        success:true,
        validationCode:0
    }
}

export const RemoveNamedUserFromProductSuccessfully: CypressRequest = {
    url: '/licensing/customer-licensing/licenses/**/products/**/named-user/**',
    method: 'DELETE',
    response: {}
}

