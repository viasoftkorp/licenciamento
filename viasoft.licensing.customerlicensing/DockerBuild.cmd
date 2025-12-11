git pull --prune
dotnet build -c Release .\Viasoft.Licensing.CustomerLicensing.sln
dotnet publish -c Release
cd .\Viasoft.Licensing.CustomerLicensing.Host\
docker build --no-cache -t korp/viasoft.licensing.customerlicensing .
docker push korp/viasoft.licensing.customerlicensing
pause