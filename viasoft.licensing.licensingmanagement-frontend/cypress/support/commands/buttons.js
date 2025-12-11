export function addButtonsCommands() {
  Cypress.Commands.add('checkIfButtonIsEnabled', (elementSelector) => {
    cy.get(elementSelector).should('be.enabled');
  });

  Cypress.Commands.add('checkIfButtonIsDisabled', (elementSelector) => {
    cy.get(elementSelector).should('be.disabled');
  });
}