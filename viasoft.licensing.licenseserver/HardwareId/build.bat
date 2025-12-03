FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"
dotnet build HardwareId.Host -c Release
dotnet publish HardwareId.Host -c Release --runtime win-x64 -p:PublishSingleFile=true --self-contained true
powershell Compress-Archive -Force .\HardwareId.Host\bin\Release\net6.0\win-x64\publish\HardwareId.Host.exe license-server-hardware-id.zip
pause