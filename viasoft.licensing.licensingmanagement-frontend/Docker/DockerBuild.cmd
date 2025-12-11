git pull --prune

cd ..
call ng build --configuration production
cd Docker

xcopy /s /y /q /i ..\dist "dist"

docker build --no-cache -t korpcicd/viasoft.Licensing.LicensingManagement-frontend:latest .
docker push korpcicd/viasoft.Licensing.LicensingManagement-frontend:latest

pause
