$env:GOOS = "linux"
Remove-Item main.zip -ErrorAction Ignore
Write-Host 'Building project..'
go build -o main main.go
Write-Host 'Zipping project..'
~\Go\Bin\build-lambda-zip.exe -o main.zip main
Write-Host 'Done.'
Remove-Item main -ErrorAction Ignore
pause