$Path = ((Get-Location).toString() + "/").replace("C:", "/c")
$NewPath = $Path.replace("\", "/")

docker rm -f cypress
docker run --name "cypress" -v ${NewPath}:/e2e -w /e2e korp/cypress-docker:node14.16.0-chrome89-ff86
docker rm -f cypress
