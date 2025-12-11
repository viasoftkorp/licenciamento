import { defineConfig } from 'cypress';
const setupNodeEvents = (on, config) => {
    return require('./cypress/plugins/index')(on, config);
};

// DO NOT ALTER ANYTHING BESIDES e2e.baseUrl PROPERTY
// VS-TEST WON'T READ ANY OTHER PROPERTY FROM THIS FILE

export default defineConfig({
    e2e: {
        baseUrl: 'http://host.docker.internal:4200',
        setupNodeEvents,
        viewportWidth: 1050,
        viewportHeight: 720,
        video: false,
        specPattern: [
          "cypress/e2e/**"
        ],
    },
    env: {
        updateSnapshots: false,
    }
});
