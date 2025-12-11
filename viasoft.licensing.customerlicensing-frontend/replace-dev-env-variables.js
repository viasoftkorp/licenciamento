const { readFileSync, writeFileSync } = require('fs');
const path = require('path');

const replaceKey = /\!URL_GATEWAY/g;
const replaceValue = process.env.URL_GATEWAY;
const environmentTsFile = './src/environments/environment.dev-env.ts';
const appsettingsFile = './src/assets/app-settings/appsettings.dev-env.json';
const environmentFiles = [environmentTsFile, appsettingsFile];
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