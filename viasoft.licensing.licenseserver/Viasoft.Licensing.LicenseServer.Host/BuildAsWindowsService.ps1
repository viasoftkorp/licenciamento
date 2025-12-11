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

Set-Location $BasePath  

pause

#>