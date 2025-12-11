const { addMatchImageSnapshotPlugin } = require('cypress-image-snapshot/plugin');

module.exports = (on, config) => {
  // Visual Regression Test plugin
  addMatchImageSnapshotPlugin(on, config);

  on('before:browser:launch', (browser, launchOptions) => {
    if (browser.name === 'chrome' && browser.isHeadless) {
      console.info('Browser is Headless Chrome, setting viewport to 1050x720')
      launchOptions.args.push('--window-size=1050,720')
      return launchOptions
    }
  })
};
