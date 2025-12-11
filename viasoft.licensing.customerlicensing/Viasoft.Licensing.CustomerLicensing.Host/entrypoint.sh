#!/bin/bash

#ON_PREMISE_MODE=True
#CERT_FILE_PATH=/etc/korp/certs/cert.crt

CertFilePath="${CERT_FILE_PATH:-/etc/korp/certs/cert.crt}"
AddCertificate="${ON_PREMISE_MODE:-false}"

if [ "${AddCertificate,,}" = "true" ]; then
  if [[ -f "${CertFilePath}" ]]; then  
    cp "${CertFilePath}" /usr/local/share/ca-certificates
    update-ca-certificates  
  else
    echo -e "\e[1;33mWARNING\e[0m ${CertFilePath} is not a valid certificate"
  fi    
fi

exec dotnet Viasoft.Licensing.CustomerLicensing.Host.dll
