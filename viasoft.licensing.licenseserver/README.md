# Viasoft.Licensing.LicenseServer

## Requisitos
1. Instalar o Inno Setup na versão >= 6.2.0
    - https://jrsoftware.org/isdl.php
2. Instalar o netsparkle-generate-appcast, através do seguinte comando: ``` dotnet tool install --global NetSparkleUpdater.Tools.AppCastGenerator ```
3. Caso esteja gerando um build para produção pegue as chaves do servidor de produção e as coloque na pasta C:\Users\<seu-usuário>\AppData\Local\netsparkle, caso esteja apenas querendo fazer um teste rode o seguinte comando: ``` netsparkle-generate-appcast ```

## Como buildar
1. Rode os seguintes comandos:
    - ```dotnet build Viasoft.Licensing.LicenseServer.Host --no-restore -c Release -p:Version=<versão>;AppCastUrl=<Url>;AutoUpdatePublicKey=<Key>```
    - ```dotnet publish Viasoft.Licensing.LicenseServer.Host --no-restore -c Release -p:Version=<versão>;AppCastUrl=<Url>;AutoUpdatePublicKey=<Key>```
    - Detalhes:
      - AppCastUrl é o paramêtro responsável por definir o url que leva até o appcast.xml, o arquivo appcast.xml é extremamente importante porquê é ele que guarda informações sobre as atualizações disponíveis.
      - AutoUpdatePublicKey é o paramêtro responsável por setar a chave pública usada pelo netsparkle para verificar a autenticidade das atualizações, essa chave comumente é encontrada dentro do arquivo: ``` C:\Users\<usuário>\AppData\Local\netsparkle\NetSparkle_Ed25519.pub ``` para ler o arquivo abra ele com qualquer editor de texto e pegue a chave pública.
2. Compile o instalador da nova versão com o seguinte comando:
    - ```iscc "./ViasoftLicensingLicensingServerSetupScript.iss"```
3. Modifique o nome do seu novo instalador para que ele fique assim: Viasoft_Licensing_LicensingServer_x.y.z.exe sendo que x, y e z são os números que representam a sua versão. Exemplo: A minha versão é 2.0.1 então meu instalador deve se chamar Viasoft_Licensing_LicensingServer_2.0.1.exe
4. Gere um novo AppCast para esse instalador através do seguinte comando (O AppCast é um arquivo xml, responsável por fazer o controle das versões):
- ```netsparkle-generate-appcast -a <pasta onde os casts vão ser gerados> -e exe -b <pasta com os binários dos instaladores> -o windows -u <url direcionado a pasta com os instaladores no servidor> -f true -n "Viasoft.Licensing.LicenseServer"```
5. Faça upload do arquivo appCast e do instalador para o servidor, mantenha os dois arquivos em pastas separadas


## Build para Portal-Interno - WINDOWS
A principal diferença entre o build do Portal de Produção e Portal Interno
1. Atualmente não temos um segundo CDN para o auto update do portal interno, portanto precisaremos de SkipAutoUpdate=true
    - ```dotnet restore Viasoft.Licensing.LicenseServer.Host\Viasoft.Licensing.LicenseServer.Host.csproj --runtime win-x64```
    - ```dotnet build --no-restore --runtime win-x64 --self-contained true -c Release Viasoft.Licensing.LicenseServer.Host\Viasoft.Licensing.LicenseServer.Host.csproj /p:"SkipAutoUpdate=true;AppCastUrl=https://cdn-connect.korp.com.br/license-server-updates/appcast.xml;AutoUpdatePublicKey=;Authority=https://gateway-interno.korp.com.br/oauth;LicensingManagementSecret=FD45E667-1449-4F30-A506-0D30E23E209B;CustomerLicensingSecret=FD45E667-1449-4F30-A506-0D30E23E209B"```
	- ```dotnet publish --no-restore --runtime win-x64 --self-contained true -c Release Viasoft.Licensing.LicenseServer.Host\Viasoft.Licensing.LicenseServer.Host.csproj /p:"SkipAutoUpdate=true;AppCastUrl=https://cdn-connect.korp.com.br/license-server-updates/appcast.xml;AutoUpdatePublicKey=;Authority=https://gateway-interno.korp.com.br/oauth;LicensingManagementSecret=FD45E667-1449-4F30-A506-0D30E23E209B;CustomerLicensingSecret=FD45E667-1449-4F30-A506-0D30E23E209B"```
    - Apagar o arquivo LicenseServerSettings.json gerado em Viasoft.Licensing.LicenseServer.Host\bin\Release\net6.0\win-x64\publish\

## Explicação de algumas propriedades do LicenseServerSettings
- AutoUpdateSettings:UpdateTime
    - Propriedade opcional, coloque aqui um horário, o serviço vai todos os dias buscar e fazer atualizações (se necessário e possível) neste mesmo horário
- AutoUpdateSettings:InstallationPath
    - Propriedade opcional, usada para definir onde o licenseServer está instalado e onde as suas atualizações devem ser instaladas, caso essa propriedade fique vazia ela sera definida automaticamente para C:\Korp\ViasoftKorpServices\Services\LicenseServer. É extremamente importante que essa propriedade condiza com o local da instalação, se não o autoUpdate não vai funcionar corretamente.
    