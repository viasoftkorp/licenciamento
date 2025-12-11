$Path = ((Get-Location).toString() + "/").replace("C:", "/c")
$NewPath = $Path.replace("\", "/")

docker rm -f cypress
docker run --name "cypress" -v ${NewPath}:/e2e -w /e2e korp/cypress-docker:cypress-13.8.1-node-20.12.2-chrome-124.0.6367.60-1-ff-125.0.2-edge-124.0.2478.51-1
docker rm -f cypress
