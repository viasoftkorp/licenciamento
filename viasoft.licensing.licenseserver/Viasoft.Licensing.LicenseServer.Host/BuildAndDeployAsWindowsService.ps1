#!/usr/bin/env bash

$BasePath = Split-Path $PSCommandPath
$PublishPath = (Join-Path $BasePath bin\Release\netcoreapp2.2\win-x64\publish)

if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Start-Process powershell.exe "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs; exit
}

set-alias nssm (Join-Path $PublishPath nssm.exe)

if (Test-Path $PublishPath) {
    Set-Location $PublishPath
    nssm stop ViasoftKorpLicenseServer
}

Set-Location (Split-Path $BasePath -Parent)

#remove bin/obj folders
Get-ChildItem .\ -include bin,obj -Recurse | ForEach-Object ($_) { Remove-Item $_.FullName -Force -Recurse }
dotnet build -c Release --runtime win-x64 .\Viasoft.Licensing.LicenseServer-build-release.sln
dotnet publish -c Release --runtime win-x64 .\Viasoft.Licensing.LicenseServer-build-release.sln

$LogPath = "C:\temp"
If(!(test-path $LogPath))
{
    New-Item -ItemType Directory -Force -Path $LogPath
}

Set-Location $PublishPath
nssm stop ViasoftKorpLicenseServer
nssm remove ViasoftKorpLicenseServer confirm

$file= (Join-Path $PublishPath Viasoft.Licensing.LicenseServer.Host.exe)
nssm install ViasoftKorpLicenseServer $file
nssm set ViasoftKorpLicenseServer AppStdout C:\temp\ViasoftKorpLicenseServer.log
nssm set ViasoftKorpLicenseServer AppStderr C:\temp\ViasoftKorpLicenseServer.log
nssm set ViasoftKorpLicenseServer Description Viasoft Korp License Server 
nssm set ViasoftKorpLicenseServer Start SERVICE_AUTO_START
#nssm set ViasoftKorpLicenseServer AppEnvironmentExtra ASPNETCORE_ENVIRONMENT=Development
nssm start ViasoftKorpLicenseServer

Set-Location $BasePath  

pause

#>