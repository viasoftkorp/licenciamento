import { addSDKCommands, addMatchImageSnapshotCommand } from '@viasoft/testing/snapshot/commands';

import { addButtonsCommands } from './commands/buttons';

addMatchImageSnapshotCommand();
addSDKCommands();
addButtonsCommands();

Cypress.Commands.add('batchRequestStub', (requestsToStub) => {
  if (Array.isArray(requestsToStub)) {
    requestsToStub.forEach((request) => cy.intercept(request.method, request.url, request.response));
  } else {
    cy.intercept(requestsToStub.method, requestsToStub.url, requestsToStub.response);
  }
});

Cypress.Commands.add('awaitableBatchRequestStub', (requestsToStub) => {
  const aliases = [];

  if (!requestsToStub) {
    return cy.wrap(aliases);
  }

  for (let i = 0; i < requestsToStub.length; i++) {
    const request = requestsToStub[i];
    const requestAlias = `${request.method}-${request.url}-${Math.round(Math.random() * 1000)}`;
    cy.intercept(request.method, request.url, request.response).as(requestAlias);
    aliases.push(`@${requestAlias}`);
  }

  return cy.wrap(aliases);
});
