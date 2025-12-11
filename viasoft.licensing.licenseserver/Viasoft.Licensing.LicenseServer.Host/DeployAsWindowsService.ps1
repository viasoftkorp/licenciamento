#!/usr/bin/env bash

$BasePath = Split-Path $PSCommandPath
set-alias nssm (Join-Path $BasePath nssm.exe)

if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Start-Process powershell.exe "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs; exit
}

nssm stop ViasoftKorpLicenseServer
nssm remove ViasoftKorpLicenseServer confirm

$file = Join-Path $BasePath Viasoft.Licensing.LicenseServer.Host.exe
nssm install ViasoftKorpLicenseServer $file

nssm set ViasoftKorpLicenseServer AppStdout C:\temp\ViasoftKorpLicenseServer.log
nssm set ViasoftKorpLicenseServer AppStderr C:\temp\ViasoftKorpLicenseServer.log
nssm set ViasoftKorpLicenseServer Description Viasoft Korp License Server 
nssm set ViasoftKorpLicenseServer Start SERVICE_AUTO_START
#nssm set ViasoftKorpLicenseServer AppEnvironmentExtra ASPNETCORE_ENVIRONMENT=Development
nssm start ViasoftKorpLicenseServer

Write-Host 'Done.'
pause

#>