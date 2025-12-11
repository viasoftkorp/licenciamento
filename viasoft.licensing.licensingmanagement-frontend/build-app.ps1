Write-Host "------------------------------------------------------"
Write-Host "                  BUILD PORTAL APP                    "
Write-Host "------------------------------------------------------"

Write-Host ""
$CDN_URL = Read-Host "Informe a URL do CDN"
$BACKEND_URL = Read-Host "Informe a URL do Gateway"
$APP_NAME = Read-Host "Informe o nome do APP(Ex.: aps)"
$APP_IDENTIFIER = Read-Host "Informe o Identifier do APP(Ex.: LS01)"

if($CDN_URL.Substring($CDN_URL.Length - 1) -eq '/') {
    $CDN_URL = $CDN_URL -replace ".$"
}

Write-Host ""
Write-Host "Instalando/Atualizando pacotes NPM..."
Write-Host ""
npm install

Write-Host ""
Write-Host "Realizando build do app..."
Write-Host ""
node --max-old-space-size=4096 ./node_modules/@angular/cli/bin/ng build $APP_NAME-portal --configuration production --deployUrl=$CDN_URL/$APP_IDENTIFIER/v1.0.0/

Write-Host ""
Write-Host "Substituindo URL do Gateway..."
Write-Host ""
(Get-Content .\dist\**\assets\app-settings\appsettings.json).replace('!BACKEND_URL', $BACKEND_URL) | Set-Content .\dist\**\assets\app-settings\appsettings.json
(Get-Content .\dist\**\main-es2015.js).replace('!CDN_URL', $CDN_URL) | Set-Content .\dist\**\main-es2015.js

Write-Host ""
Write-Host "Upload para S3..."
Write-Host ""
7z a .\dist\$APP_NAME\$APP_IDENTIFIER .\dist\$APP_NAME\*
aws s3 cp .\dist\$APP_NAME\$APP_IDENTIFIER.7z s3://cdn-interno/ --acl public-read
Remove-Item –path .\dist -recurse

Write-Host ""
Write-Host "Build concluído com sucesso!"
Write-Host ""

Start-Sleep 5
