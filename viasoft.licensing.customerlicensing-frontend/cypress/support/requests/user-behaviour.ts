export const GetUserBehaviourFromProductFloating: CypressRequest = {
    url: 'licensing/customer-licensing/user-behaviour/products/**/floating?**',
    method: 'GET',
    response: {
        totalCount:1,
        items:[
           {
              licensingIdentifier:"ee19958d-5f6a-4201-b989-08d7468f1643",
              appIdentifier:"LS01",
              appName:"Gerenciador de Licenças",
              bundleIdentifier:"Default",
              bundleName:"Pacote Default",
              softwareName:"Sistema web",
              softwareIdentifier:"WEB",
              user:"admin@korp.com.br",
              softwareVersion:null,
              hostName:null,
              hostUser:null,
              localIpAddress:null,
              language:null,
              osInfo:null,
              browserInfo:null,
              databaseName:null,
              startTime:"2021-09-22T16:53:12Z",
              lastUpdate:"2021-09-22T16:55:12.0613158Z",
              accountName:"Default Company",
              domain:0,
              accessDuration:"07:23:08.9642141",
              accessDurationFormatted:"07:23:08"
           }
        ]
     }
}

export const GetUserBehaviourFromProductNamedOnline: CypressRequest = {
   url: 'licensing/customer-licensing/user-behaviour/products/**/named/online?**',
   method: 'GET',
   response: {
      totalCount:3,
      items:[
         {
            licensingIdentifier:"ee19958d-5f6a-4201-b989-08d7468f1643",
            appIdentifier:"LS01",
            appName:"Gerenciador de Licenças",
            bundleIdentifier:"pacote2",
            bundleName:"Pacote 2",
            softwareName:"Sistema web",
            softwareIdentifier:"WEB",
            user:"admin@korp.com.br",
            softwareVersion:null,
            hostName:null,
            hostUser:null,
            localIpAddress:null,
            language:null,
            osInfo:null,
            browserInfo:null,
            databaseName:null,
            startTime:"2021-10-06T12:39:30Z",
            lastUpdate:"2021-10-06T13:54:49.1046897Z",
            accountName:"Default Company",
            domain:0,
            status:0,
            accessDuration:"01:18:35.1782670",
            accessDurationFormatted:"01:18:35"
         },
         {
            licensingIdentifier:"ee19958d-5f6a-4201-b989-08d7468f1643",
            appIdentifier:"ADM01",
            appName:"Administração",
            bundleIdentifier:"pacote2",
            bundleName:"Pacote 2",
            softwareName:"Sistema web",
            softwareIdentifier:"WEB",
            user:"admin@korp.com.br",
            softwareVersion:null,
            hostName:null,
            hostUser:null,
            localIpAddress:null,
            language:null,
            osInfo:null,
            browserInfo:null,
            databaseName:null,
            startTime:"2021-10-06T13:54:14Z",
            lastUpdate:"2021-10-06T13:54:49.1046897Z",
            accountName:"Default Company",
            domain:0,
            status:0,
            accessDuration:"00:03:51.1782678",
            accessDurationFormatted:"00:03:51"
         },
         {
            licensingIdentifier:"00000000-0000-0000-0000-000000000000",
            appIdentifier:null,
            appName:null,
            bundleIdentifier:null,
            bundleName:null,
            softwareName:null,
            softwareIdentifier:null,
            user:"admin.user@korp.com.br",
            softwareVersion:null,
            hostName:null,
            hostUser:null,
            localIpAddress:null,
            language:null,
            osInfo:null,
            browserInfo:null,
            databaseName:null,
            startTime:"0001-01-01T00:00:00Z",
            lastUpdate:"0001-01-01T00:00:00Z",
            accountName:null,
            domain:0,
            status:1,
            accessDuration:"00:00:00",
            accessDurationFormatted:"00:00:00"
         }
      ]
   }
}