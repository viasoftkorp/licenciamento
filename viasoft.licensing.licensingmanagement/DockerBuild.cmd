git pull --prune
dotnet build -c Release .\Viasoft.Licensing.LicensingManagement.sln
dotnet publish -c Release
cd .\Viasoft.Licensing.LicensingManagement.Host\
docker build --no-cache -t korp/viasoft.licensing.licensingmanagement:latest .
docker push korp/viasoft.licensing.licensingmanagement:latest
pause