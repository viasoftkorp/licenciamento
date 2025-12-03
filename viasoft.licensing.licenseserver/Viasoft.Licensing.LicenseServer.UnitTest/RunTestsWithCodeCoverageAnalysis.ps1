$solutionName = "Viasoft.Licensing.LicenseServer"
$testProjects = "Viasoft.Licensing.LicenseServer.UnitTest"

# Get the most recent OpenCover NuGet package from the dotnet nuget packages
$nugetOpenCoverPackage = Join-Path -Path $env:USERPROFILE -ChildPath "/.nuget/packages/OpenCover"
$latestOpenCover = Join-Path -Path ((Get-ChildItem -Path $nugetOpenCoverPackage | Sort-Object Fullname -Descending)[0].FullName) -ChildPath "tools/OpenCover.Console.exe"
# Get the most recent OpenCoverToCoberturaConverter from the dotnet nuget packages
$nugetCoberturaConverterPackage = Join-Path -Path $env:USERPROFILE -ChildPath "/.nuget/packages/OpenCoverToCoberturaConverter"
$latestCoberturaConverter = Join-Path -Path (Get-ChildItem -Path $nugetCoberturaConverterPackage | Sort-Object Fullname -Descending)[0].FullName -ChildPath "tools/OpenCoverToCoberturaConverter.exe"
$CONFIG = "Release"

Write-Host $latestOpenCover
Write-Host $latestCoberturaConverter

If (Test-Path "$PSScriptRoot/TestResults/OpenCover.coverageresults"){
	Remove-Item "$PSScriptRoot/TestResults/OpenCover.coverageresults"
}

If (Test-Path "$PSScriptRoot/TestResults/Cobertura.coverageresults"){
	Remove-Item "$PSScriptRoot/TestResults/Cobertura.coverageresults"
}

# & dotnet restore 

$testRuns = 1;
foreach ($testProject in $testProjects){
    # Arguments for running dotnet
    $dotnetArguments = "test -f netcoreapp2.2 -c $CONFIG $PSScriptRoot/$testProject.csproj"
    $Searchdirs = "$PSScriptRoot/bin/$CONFIG/netcoreapp2.2"
    $Output = "$PSScriptRoot/TestResults/OpenCover.coverageresults"
    
    Write-Host $dotnetArguments
    Write-Host $Searchdirs
    Write-Host $Output
    
    "Running tests with OpenCover"
    & $latestOpenCover `
        -target:dotnet.exe `
        -targetargs:$dotnetArguments `
        -output:$Output `
        -mergeoutput `
        -oldStyle `
        -hideskipped:File `
        -excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute `
        "-filter:+[$solutionName*]* -[$testProject.*Tests*]*"  `
        -register:user `
        -searchdirs:$Searchdirs

        $testRuns++
}

"Converting coverage reports to Cobertura format"
& $latestCoberturaConverter `
    -input:"$PSScriptRoot/TestResults/OpenCover.coverageresults" `
    -output:"$PSScriptRoot/TestResults/Cobertura.coverageresults" `
    "-sources:$PSScriptRoot/TestResults/"

Write-Host "Finished"