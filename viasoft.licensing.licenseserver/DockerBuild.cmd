git pull --prune
dotnet build -c Release .\Viasoft.Licensing.LicenseServer.sln
dotnet publish -c Release
cd .\Viasoft.Licensing.Core.Host\
docker build --no-cache -t korpcicd/viasoft.licensing.licenseserver .
docker push korpcicd/viasoft.licensing.licenseserver
pause