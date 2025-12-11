# Viasoft.Licensing.CustomerLicensing

## Instalação GO:
1. Instalar o GO [https://golang.org/](https://golang.org/ "https://golang.org/")
2. Instalar dependencia da Amazon:
 1. go get [github.com/aws/aws-lambda-go/lambda](github.com/aws/aws-lambda-go/lambda "github.com/aws/aws-lambda-go/lambda")
 2. go get [github.com/aws/aws-lambda-go/events](github.com/aws/aws-lambda-go/events "github.com/aws/aws-lambda-go/events")
 3. go get -u [github.com/aws/aws-lambda-go/cmd/build-lambda-zip](github.com/aws/aws-lambda-go/cmd/build-lambda-zip "github.com/aws/aws-lambda-go/cmd/build-lambda-zip")
3. Por padrão, os projetos do Go ficam em **C:/users/~USER/Go/src/**
 1. Se para executar ou compilar o codigo fora desse diretorio der problema, adicionar ao GOPATH ou coloque seu codigo neste caminho.


## Realizando build e gerando Zip do projeto GO para deploy na Amazon
1. No seu projeto, onde fica seu arquivo e função **main**, copie e cole o arquivo **BuildAndZipForAWS.ps1**.
2. Navegar até a pasta do passso anterior e clicar com o botão da direita no arquivo  **BuildAndZipForAWS.ps1** em seguida em **Executar com o Powershell**.
3. Um novo arquivo **main.zip** vai ser gerado, e este arquivo que faremos o deploy na Amazon.

## Criando a estrutura da função Lambda na Amazon Web Services
1. Acessar o menu **Lambda**
2. Criar uma nova função:
 1. **Function name**: seguir o padrão Module_Domain_Area_Method
 2. **Runtime**: GO
 3. **Execution role**: Selecionar a  ja existente **Lambda-Access**
3. Na tela de configurações da função, preencher:
 1. **Tags**:  Preencher de acordo com o padrão do SDK Web
    1. Area
	2. Domain
	3. Module
 2. **Network**:
    1. Selecionar **VPC CybeleSoftware**
    2. Selecionar subnet **subnet-6218df07**.
    3. Selecionar Security groups **SG Lambda - searchCNPJ**.
 3. **Environment variables**: Verificar na AWS os valores a utilizar
    1. MONGODB_IP
	2. MONGODB_PORT
 4. **Basic settings**:
    1. Preencher a Description
    2. Diminuir o limite de memória

## Realizando o deploy da função GO no Lambda da Amazon Web Services:
1. Acessar a função criada.
2. Na area **Function code**:
 1. Clicar em Upload e selecionar o .zip gerado pelo **BuildAndZipForAWS.ps1**.
 2. No campo **Handler** colocar o nome da função do seu package main que realiza o trecho de codigo **lambda.Start(Handler_Method)**. Normalmente colocamos o nome **main**.
 3. Clicar em **Save**.
3. No canto superior direito, criar um novo evento de teste.
 1. Se necessário um exemplo do input esperado, veja o teste da função "Licensing_CustomerLicensing_LicenseUsageInRealTime_Import"
4. Clicar no botão **Test** para validar sua função e seu input de teste.

## Vinculando a função Lambda a uma URL via API Gateway na Amazon Web Services
1. Acessar o menu **API Gateway**
2. Acessar na lateral esquerda o menus: 
 1. **erp -> Resources**
3. Um lista de todos os endpoint disponiveis será exibida.
4. Seguimos o padrão de URL do SDK Web, ou seja, Module/Domain/Area/Método, ex: Licensing/CustomerLicensing/LicenseUsageInRealTime/Import
5. Para criar um novo endpoint, clicar em: **Actions -> Create Resource**.
 1. Informar o nome do resource.
 2. **Enable API Gateway CORS** ativo.
 3. Clicar em **Save**
 4. Repetir este passo até gerar a url desejada.
6. Com seu caminho/resource criado, clicar em: **Actions -> Create Method**.
 1. Selecionar qual tipo de requisição HTTP será (POST,GET,DELETE,etc..) e confirme.
 2. Nas configurações do método:
    1. **Integration type:** Lambda Function.
    2. **Lambda Proxy integration** ativo.
    3. ** Lambda Region**: sa-east-1.
    4. **Lambda Function**: Começe a escrever o nome da sua função que ela aparecerá para selecionar.
   3. Clicar em **Save**.
   4. Um quadro mostrando o caminho da sua função será exibido, clicar em **Method Request**
    1. **API Key Required** ativo.
    2. Se desejado, é possível ativar o **Request Validator**, e adicionar no grupo URL **Query String Parameters**, os parâmetros da request.

# Realizando deploy da configuração do API Gateway em ambiente de teste na Amazon Web Services
1. Após toda esta configuração, podemos realizar o deploy para teste.
    1. Clicar em **Actions -> Deploy API**.
    2. Selecionar **Deployment Stage**:  **Dev**.
    3. Clicar em **Deploy**.
2. Verificar a **API Key** para utilizar na chamada da sua função:
    1. No menu lateral esquerdo clicar em **API Keys**.
    2. Então clicar em **korp-dev**.
    3. Clicar em **API Key -> Show**.

## Testando a chamada via Postman:
1. Abrir o [Postman](https://www.getpostman.com/downloads/ "Postman").
2. Criar uma nova request.
3. Informar a URL [https://api.korp.com.br/dev/](https://api.korp.com.br/dev/ "https://api.korp.com.br/dev/")
4. Adicionar ao final da URL o caminho do resource criado
5. Selecionar o tipo do seu método (POST,GET,DELETE,etc..).
6. Na aba **headers**, adicionar:
 1. Key: **x-api-key**. 
 2. Value: **API-KEY do passo 8 do passo a passo anterior**.
7. Se seu método for **GET**, adicionar seus parâmetros na aba **Params**.
8. Se seu método por **POST**, adicionar o body da requisição na aba **Body**.
9. Clicar em **Send** e teste seu método.

## Realizando o delpoy para produção na Amazon Web Services
1. No menu **API Gateway**:
    1. Clicar em **Actions -> Deploy API**.
    2. Selecionar **Deployment Stage**: **v1**.
    3. Clicar em **Deploy**.
2. Verificar a **API Key** para utilizar na chamada da sua função:
    1. No menu lateral esquerdo clicar em **API Keys**.
    2. Então clicar em **cnpj-erp**.
    3. Clicar em **API Key -> Show**.
3. A URL base para realizar as chamadas será [https://api.korp.com.br/v1/](https://api.korp.com.br/dev/ "https://api.korp.com.br/v1/") 