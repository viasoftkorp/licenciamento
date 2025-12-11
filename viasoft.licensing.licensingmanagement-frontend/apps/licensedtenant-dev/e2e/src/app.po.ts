import { browser, by, element } from 'protractor';

export class AppPage {
  navigateTo() {
    return browser.get(browser.baseUrl) as Promise<any>;
  }

  getTitleText() {
    return element(by.css('usage-search-tenant-filter-modal-root .content span')).getText() as Promise<string>;
  }
}
