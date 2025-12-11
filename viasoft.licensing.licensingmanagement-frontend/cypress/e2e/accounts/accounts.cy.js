import {
  createAccountRequest,
  deleteAccount,
  getAllEmptyRequest,
  getAllRequest,
  getAllRequestNew,
  getAllRequestNewInverted,
  getExistingAccount,
  randomAccountsRequest,
  randomAccountsRequest100Items,
  updateAccountRequest
} from '../../support/requests/sdk/licensing-management/accounts/account';
import {
  companyAlreadyRegistered,
  companyDoesNotExist,
  getCompanyByCnpj,
  getNewCompanyByCnpj
} from '../../support/requests/sdk/licensing-management/accounts/cnpj';
import {
  getAddressByZipcode,
  getNewAddressByZipcode
} from '../../support/requests/sdk/licensing-management/accounts/zipCode';

describe('License Management Accounts Test', () => {
  it('open accounts page', () => {
    cy.batchRequestStub(getAllRequest);
    cy.visit('/license-management/accounts');
    cy.wait(2000);
    cy.matchImageSnapshot('accounts-page-default');
  });

  it('display error messages when all inputs are cleared, retrieve CNPJ and address, and update the item', () => {
    cy.batchRequestStub([getAllRequest, getExistingAccount]);
    cy.visit('/license-management/accounts');
    cy.get('[ng-reflect-tooltip="Editar"]').should('be.visible');
    cy.get('[ng-reflect-tooltip="Editar"]').click();
    cy.getVsInput('companyName').clear();
    cy.getVsInput('cnpjCpf').clear();
    cy.get('vs-select[formControlName=status]').click(); // status select input
    cy.get('mat-option').contains('Ativa').click(); // status active option
    cy.get('vs-select[formControlName=status] div.mat-mdc-form-field-flex > div > vs-button[icon="xmark"]').click(); // click on X icon to clear status
    cy.getVsInput('tradingName').clear();
    cy.getVsInput('phone').clear().clear();
    cy.getVsInput('email').clear();
    cy.getVsInput('zipCode').clear();
    cy.getVsInput('state').clear();
    cy.getVsInput('city').clear();
    cy.getVsInput('neighborhood').clear();
    cy.getVsInput('street').clear();
    cy.getVsInput('number').clear();
    cy.getVsInput('detail').clear();
    cy.getVsInput('billingEmail').clear().blur();
    checkIfSaveButtonIsDisabled();
    cy.get('#mat-mdc-error-1').should('exist');
    cy.get('#mat-mdc-error-2').should('exist');
    cy.get('#mat-mdc-error-3').should('exist');
    cy.get('#mat-mdc-error-0').should('exist');
    cy.get('vs-dialog vs-layout[fill] div[content]').eq(0).scrollTo('top');
    cy.waitForAllAnimations();
    cy.wait(500);
    cy.matchImageSnapshot('edit-account-empty-fields');

    cy.awaitableBatchRequestStub([getAddressByZipcode, getCompanyByCnpj, updateAccountRequest, getAllRequest, companyDoesNotExist]).then(() => {
      cy.getVsInput('cnpjCpf').type('00000000000000');
      clickSearchCnpjIcon();
      cy.wait(500);
      cy.matchImageSnapshot('no-cnpj-register-update');
      clickOkButton();
      cy.getVsInput('cnpjCpf').clear();
      cy.typeAndValidateVsInput('cnpjCpf', '55335198000156', '55.335.198/0001-56');
      clickSearchCnpjIcon();
      cy.get('.mat-mdc-select-value').click(); // status select input
      cy.get('#mat-option-1 > .mdc-list-item__primary-text').click(); // status active option
      checkIfSaveButtonIsEnabled();
      cy.typeAndValidateVsInput('zipCode', '80010130', '80010-130');
      cy.get('vs-form:nth-child(2) > div > form > vs-input:nth-child(1) vs-icon').click({ force: true }); // zip code search icon
      cy.get('vs-input[formcontrolname="city"] input').should('have.value', 'Curitiba');
      cy.typeAndValidateVsInput('number', '180', '180');
      cy.waitForAllAnimations();
      cy.getVsInput('billingEmail').type('financeiro').blur({ force: true });
      cy.wait(700);
      cy.get('.mat-mdc-form-field-error-wrapper mat-error').should('have.text', 'Email inválido.');
      cy.matchImageSnapshot('invalid-billing-email-error');
      checkIfSaveButtonIsDisabled();
      cy.get('#mat-mdc-error-14').should('exist'); // invalid billing email error
      cy.typeAndValidateVsInput('billingEmail', '@farmac.com', 'financeiro@farmac.com').blur();
      checkIfSaveButtonIsEnabled();
      cy.wait(500);
      cy.matchImageSnapshot('edit-account-filled-fields');
      clickSaveButton();
      closeModal();
    });
  });

  const newCnpjAndZipCodeRequests = [getNewAddressByZipcode, getNewCompanyByCnpj, companyAlreadyRegistered];
  const saveAndRefreshGridRequests = [createAccountRequest, getAllRequestNew];

  it('create new account, test inputs and refresh grid', () => {
    cy.batchRequestStub([...newCnpjAndZipCodeRequests, ...saveAndRefreshGridRequests]);
    cy.visit('/license-management/accounts');
    cy.get('[icon="plus"] > .nav-mode').should('be.visible');
    cy.get('[icon="plus"] > .nav-mode').click({force : true}); // add button
    cy.wait(500);
    cy.getVsInput('companyName').blur({ force: true});
    cy.wait(500);
    cy.matchImageSnapshot('create-account-empty-fields');
    cy.typeAndValidateVsInput('companyName', 'Mariana Comercio de Bebidas Ltda', 'Mariana Comercio de Bebidas Ltda');
    cy.getVsInput('cnpjCpf').type('55335198000156');
    clickSearchCnpjIcon();
    cy.wait(500);
    cy.matchImageSnapshot('cnpj-already-in-use');
    clickOkButton();
    cy.getVsInput('cnpjCpf').clear({ force: true });
    cy.typeAndValidateVsInput('cnpjCpf', '98483050000171', '98.483.050/0001-71').blur();
    cy.wait(500);
    cy.matchImageSnapshot('no-cnpj-register-create');
    clickOkButton();
    cy.validateVsInput('companyName', '');
    cy.getVsInput('companyName').type('Mariana Comercio de Bebidas Ltda');
    cy.get('.mat-mdc-select-value').click(); // status select input
    cy.get('#mat-option-1 > .mdc-list-item__primary-text').click(); // status active option
    cy.typeAndValidateVsInput('tradingName', 'Mari Bebidas', 'Mari Bebidas');
    checkIfSaveButtonIsEnabled();
    cy.getVsInput('phone').type('4128').blur();
    checkIfSaveButtonIsDisabled();
    cy.get('#mat-mdc-error-4').should('exist'); // invalid phone error
    cy.wait(500);
    cy.matchImageSnapshot('invalid-phone-error');
    cy.typeAndValidateVsInput('phone', '402518', '(41) 2840-2518');
    checkIfSaveButtonIsEnabled();
    cy.typeAndValidateVsInput('webSite', 'maribebidas.com.br', 'maribebidas.com.br');
    cy.getVsInput('email').type('contato').blur();
    checkIfSaveButtonIsDisabled();
    cy.get('#mat-mdc-error-6').should('exist');
    cy.wait(500);
    cy.matchImageSnapshot('invalid-email-error');
    cy.typeAndValidateVsInput('email', '@maribebidas.com.br', 'contato@maribebidas.com.br');
    checkIfSaveButtonIsEnabled();
    cy.typeAndValidateVsInput('zipCode', '85035025', '85035-025');
    cy.typeAndValidateVsInput('number', '220', '220');
    cy.typeAndValidateVsInput('detail', 'Lote 1', 'Lote 1');
    cy.typeAndValidateVsInput('billingEmail', 'contato@maribebidas.com.br', 'contato@maribebidas.com.br').blur();
    cy.wait(500);
    cy.matchImageSnapshot('create-account-filled-fields');
    clickSaveButton();
    closeModal();
  });

  it('sort by name field and delete the two items', () => {
    cy.batchRequestStub(getAllRequestNew);
    cy.visit('/license-management/accounts');
    cy.get('th[ng-reflect-field="companyName"]').should('be.visible');
    cy.batchRequestStub(getAllRequestNewInverted);
    cy.reload();
    cy.get('th[ng-reflect-field="companyName"]').should('be.visible');
    cy.get('th[ng-reflect-field="companyName"]').dblclick({ force: true });
    cy.get('.vs-header-mode-nav').click({ force: true}); // click anywhere to blur
    cy.wait(500);
    cy.matchImageSnapshot('accounts-page-inverted');

    cy.get('.vs-grid-actions-column-header vs-button').should('be.visible');
    cy.get('.vs-grid-actions-column-header vs-button').click({ force: true });
    cy.get('[ng-reflect-tooltip="Apagar"] button').eq(0).click({ force: true });
    cy.wait(500);
    cy.matchImageSnapshot('delete-first-account-modal');
    cy.get('[label="SDK.MessageDialog.Yes"]').click();
    cy.batchRequestStub([getAllRequest, deleteAccount]);
    cy.get('[ng-reflect-tooltip="Apagar"] button').should('have.length', 1)
    cy.get('[ng-reflect-tooltip="Apagar"] button').click({ force: true });
    cy.wait(500);
    cy.matchImageSnapshot('delete-second-account-modal');
    cy.batchRequestStub(getAllEmptyRequest);
    cy.get('[label="SDK.MessageDialog.Yes"]').click();
    cy.wait(500);
    cy.waitForAllAnimations();
    cy.matchImageSnapshot('accounts-page-empty');
  });

  it('generate 150 items and show different item quantities', () => {
    cy.batchRequestStub([randomAccountsRequest, randomAccountsRequest100Items]);
    cy.visit('/license-management/accounts');
    cy.get('.p-dropdown-trigger-icon').should('be.visible')
    cy.get('.p-dropdown-trigger-icon').click();
    cy.get(':nth-child(4) > .p-dropdown-item').click();
    cy.wait(500);
    cy.matchImageSnapshot('accounts-page-100-items');
  });
});

function checkIfSaveButtonIsEnabled() {
  cy.checkIfButtonIsEnabled('[footer] > vs-button > .vs-button-style');
}

function checkIfSaveButtonIsDisabled() {
  cy.checkIfButtonIsDisabled('[footer] > vs-button > .vs-button-style');
}

function closeModal() {
  cy.get('div[actions] > vs-button[icon="xmark"] > button').click();
}

function clickSaveButton() {
  cy.get('div[footer] > vs-button button').click({ force: true });
}

function clickSearchCnpjIcon() {
  cy.get('vs-input:nth-child(2) vs-button > button > span > vs-icon').click();
}

function clickOkButton() {
  cy.get('[modal-footer-actions] > vs-button button').click({ force: true });
}
