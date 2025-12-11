//#region Imports
const shell = require('shelljs');
//#endregion

const SERVE_COMMAND = 'npm run cypress:serve'; // Change this to your liking
const TEST_COMMAND = 'npx vs-test run -a --report --jenkins --force-base-url'; // You probably shouldn't change this

//#region App-specific configuration
/*
* If you need to perform any actions before your application is served, you
* should put them here.
* Example:
*   shell.exec('npm run build:theme');
*/
//#endregion

//#region Serve app and run rests
const serve = shell.exec(SERVE_COMMAND, { silent: false, async: true });
let runningTests = false;

const endProcess = (code) => {
    if (serve) {
        serve.kill(code);
    }
    process.exit(code);
}

serve
    .stdout
    .on('data', data => {
        if (String(data).indexOf('Angular Live Development Server') > -1) {
            setTimeout(() => {
                if (!runningTests) {
                    console.log('Error detected, process will exit now.');
                    endProcess(-1);
                }
            }, 5000);
        }
        if (String(data).indexOf('Compiled successfully') > -1) {
            runningTests = true;
            console.log('Running tests...');
            endProcess(shell.exec(TEST_COMMAND).code);
        }

    });
//#endregion
