/// <reference types="cypress" />

declare namespace Cypress {
  interface Chainable {
    //#region cypress-image-snapshot
    /**
     * Takes a screenshot of the screen and compare with the saved one
     * @repo https://github.com/palmerhq/cypress-image-snapshot
     */
    matchImageSnapshot(name?: string): Chainable<Element>
    matchImageSnapshot(options: object): Chainable<Element>
    matchImageSnapshot(name: string, options: object): Chainable<Element>
    //#endregion cypress-image-snapshot
    // TODO: Add cypress plugins methods here
  }
}

interface CypressRequest {
  url: string;
  method: 'GET' | 'POST' | 'DELETE' | 'PUT';
  response: any;
}
