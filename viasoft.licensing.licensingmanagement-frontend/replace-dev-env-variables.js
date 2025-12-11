const { readFileSync, writeFileSync } = require('fs');
const path = require('path');
const { glob } = require("glob");

const workspacePath = __dirname.replace(/\\/g, '/');

const environmentTsFiles = findEnvironmentTsFiles();
const appsettingsFiles = findAppSettingsFiles();

if (environmentTsFiles.length == 0) {
  throw new Error("Unable to find 'environment.dev-env.ts'");
}
if (appsettingsFiles.length == 0) {
  throw new Error("Unable to find 'appsettings.dev-env.json'");
}

const replaceKey = /\!URL_GATEWAY/g;
const replaceValue = process.env.URL_GATEWAY;
const environmentFiles = [...environmentTsFiles, ...appsettingsFiles];
const environmentFilesFormat = 'utf-8';

for (const environmentFile of environmentFiles) {
  const fileContent = readFileSync(environmentFile, environmentFilesFormat);
  writeFileSync(
    environmentFile,
    fileContent.replace(replaceKey, replaceValue),
    environmentFilesFormat
  );
}

console.info(`Environment Variables were replaced in ${environmentFiles.map(filePath => `"${path.basename(filePath)}"`).join(', ')} files`);

function findEnvironmentTsFiles() {
  let environmentDevEnvFiles = glob.sync(workspacePath + '/apps/**/environment.dev-env.ts');
  return environmentDevEnvFiles;
}

function findAppSettingsFiles() {
  let appSettingsDevEnvFiles = glob.sync(workspacePath + '/apps/**/appsettings.dev-env.json');
  return appSettingsDevEnvFiles;
}