const { addMatchImageSnapshotPlugin } = require('@viasoft/testing/snapshot/plugin');

module.exports = (on, config) => {
  // Visual Regression Test plugin
  addMatchImageSnapshotPlugin(on, config);

  on('before:browser:launch', (browser, launchOptions) => {
    if (browser.name === 'chrome' && browser.isHeadless) {
      launchOptions.args.push('--window-size=1066,815');
    }
    if (browser.name === 'chrome') {
      launchOptions.preferences.default['browser.enable_spellchecking'] = false;
    }
    return launchOptions;
  });
};
