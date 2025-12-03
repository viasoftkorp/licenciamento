#!/bin/bash

set -e

# Install OpenCover and ReportGenerator, and save the path to their executables.
# nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.519 OpenCover
# nuget install -Verbosity quiet -OutputDirectory packages -Version 2.4.5.0 ReportGenerator

openCoverVersion=$(ls $HOME/.nuget/packages/OpenCover/ -1 | sort -r | head -1)
openCoverPath=$HOME/.nuget/packages/OpenCover/${openCoverVersion}tools/OpenCover.Console.exe
echo openCoverPath: ${openCoverPath}

reportGeneratorVersion=$(ls $HOME/.nuget/packages/ReportGenerator/ -1 | sort -r | head -1)
reportGeneratorPath=$HOME/.nuget/packages/ReportGenerator/${reportGeneratorVersion}tools/net47/ReportGenerator.exe
echo reportGeneratorPath ${reportGeneratorPath}

CONFIG=Debug
# Arguments to use for the build
DOTNET_BUILD_ARGS="-c $CONFIG"
# Arguments to use for the test
DOTNET_TEST_ARGS="$DOTNET_BUILD_ARGS"

echo CLI args: ${DOTNET_BUILD_ARGS}

# echo Restoring
# dotnet restore

# echo Building
# dotnet build $DOTNET_BUILD_ARGS

# echo Testing
# dotnet test -f netcoreapp2.2 $DOTNET_TEST_ARGS Viasoft.Licensing.LicenseServer.UnitTest.csproj

coverage=./bin/${CONFIG}/netcoreapp2.2/coverage

echo ${coverage}

if [[ -d "$coverage" ]]; then
    rm -rf ${coverage}
fi
mkdir -p ${coverage}

echo "Calculating coverage with OpenCover..."
${openCoverPath} \
  -target:"dotnet.exe" \
  -targetargs:"test -f netcoreapp2.2 $DOTNET_TEST_ARGS Viasoft.Licensing.LicenseServer.UnitTest.csproj" \
  -mergeoutput \
  -hideskipped:File \
  -output:${coverage}/coverage.xml \
  -oldStyle \
  -filter:"+[Viasoft.Licensing.LicenseServer*]* -[Viasoft.Licensing.LicenseServer.UnitTest.*Tests*]*" \
  -searchdirs:./bin/${CONFIG}/netcoreapp2.2 \
  -register:user \
  -excludebyattribute:System.CodeDom.Compiler.GeneratedCodeAttribute

echo "Generating HTML report..."
${reportGeneratorPath} \
  -reports:${coverage}/coverage.xml \
  -targetdir:${coverage} \
  -verbosity:Error
  
echo "Opening HTML in browser..."
start ${coverage}/index.htm