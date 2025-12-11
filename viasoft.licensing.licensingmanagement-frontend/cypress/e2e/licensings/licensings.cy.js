import { getOnlyActiveAccounts, getSearchedAccount } from '../../support/requests/sdk/licensing-management/accounts/account';
import { getAllAppsRequest, getSearchedApp } from '../../support/requests/sdk/licensing-management/licensings/app';
import {
  getAllBundlesMinusLicensedBundles,
  getSearchedBundle,
} from '../../support/requests/sdk/licensing-management/licensings/bundle';
import { getDomains } from '../../support/requests/sdk/licensing-management/licensings/domain';
import {
  getAllRequest,
  getAllRequestAfterInsert
} from '../../support/requests/sdk/licensing-management/licensings/licensedTenantView';
import {
  addAppToLicense,
  addBundleToLicense,
  createLicense,
  createLicenseWithInvalidAccount,
  createLicenseWithInvalidAdministratorEmail,
  createLicenseWithInvalidIdentifier,
  editOrganizationEnvironmentRequest,
  editOrganizationUnitRequest,
  fileAppQuota,
  fileTenantQuota,
  getAllLicensedApps,
  getAllLicensedAppsForFileQuota,
  getAllLicensedAppsInBundle,
  getAllLicensedAppsInBundleAfterDelete,
  getAllLooseLicensedApps,
  getInfrastructureConfigurations,
  getLicenseById,
  getOrganizationEnvironmentRequest,
  getOrganizationUnitRequest,
  removeAppFromLicenseError,
  removeAppFromLicenseSuccess,
  removeBundleFromLicenseError,
  removeBundleFromLicenseSuccess,
  updateLicense,
  updateLooseAppFromLicense,
} from '../../support/requests/sdk/licensing-management/licensings/licensing';
import { getAllProducts, getAllProductsAfterDeleteApp, getAllProductsAfterDeleteProduct, getAllProductsAfterInsertApp, getAllProductsEmpty } from '../../support/requests/sdk/licensing-management/licensings/product';

const getPlusButtonProductTab = () => 'vs-layout div[actions] vs-button[icon="plus"] button';

describe('Licensings Page Test', () => {

  it('Visit licenses page', () => {
    cy.batchRequestStub([getAllRequest, getDomains, getAllProducts, getInfrastructureConfigurations]);
    cy.visit('/license-management/licensings');
    cy.get('vs-grid tbody tr').should('exist');
    cy.wait(500);
    //cy.matchImageSnapshot('license-grid-default');
  });

  it('Check form errors, check button status, verify save behavior, and add file quote', () => {
    cy.visit('/license-management/licensings');
    cy.batchRequestStub([getOnlyActiveAccounts, getDomains, getAllProductsEmpty, getInfrastructureConfigurations]);
    cy.get('[icon="plus"] > .nav-mode').click(); // new item
    // check input status
    cy.get('#mat-input-2').click(); // open a modal for account input
    cy.get('vs-layout div[actions] > vs-button[icon="xmark"] > button').click(); // close modal to check required msg
    cy.getVsInput('licensedCnpjs').click({ force: true }).type('1', { force: true }).clear().blur({ force: true });
    cy.get('vs-select[formcontrolname="status"] mat-select').should('have.class', 'mat-mdc-select-disabled'); // status selection is automatic
    //cy.get('vs-select vs-icon').click();
    cy.getVsInput('administratorEmail').type('a').clear().blur({ force: true });
    cy.get('#mat-input-0').type('01/12/1990').blur({ force: true });
    cy.get('[icon="plus"] > .nav-mode').scrollIntoView(); // scroll to top for snap
    cy.wait(500);
    //cy.matchImageSnapshot('create-license-inputs-clear');
    cy.checkIfButtonIsDisabled('vs-header > header div[actions] .scroll-content > vs-button[icon="floppy-disk"] > button'); // save license button
    cy.checkIfButtonIsEnabled('vs-header > header div[actions] .scroll-content > vs-button[icon="plus"] > button'); // plus license button
    cy.get('#mat-input-0').clear();

    cy.batchRequestStub([getOnlyActiveAccounts, getSearchedAccount, createLicense, getLicenseById,
      getDomains, getAllProducts, getInfrastructureConfigurations]);
    cy.get('app-license-detail > vs-form vs-search-input[controlname="accountName"] vs-icon').click(); // open account input
    cy.wait(500);
    cy.get('vs-select-modal > vs-layout vs-search > form > vs-input input').type('Mari').blur({ force: true }); // search account input
    cy.wait(500);
    //cy.matchImageSnapshot('filtered-account-input');
    cy.get('#mat-mdc-dialog-1 .p-selectable-row').eq(0).click(); // select a account
    cy.get('#mat-input-0').type('01/12/2050');
    cy.get('[formControlName="notes"] textarea').type('Testando').blur({ force: true });
    cy.get('[icon="plus"] > .nav-mode').scrollIntoView();  // scroll to top for snap
    cy.wait(500);
    //cy.matchImageSnapshot('create-license-inputs-filled');
    cy.get('[icon="floppy-disk"] > .nav-mode').click();
    cy.wait(500);
    cy.checkIfButtonIsEnabled(getPlusButtonProductTab()); // new bundle button
    cy.checkIfButtonIsDisabled('vs-header > header div[actions] .scroll-content > vs-button[icon="floppy-disk"] > button'); // save license button
    cy.checkIfButtonIsEnabled('vs-header > header div[actions] .scroll-content > vs-button[icon="plus"] > button'); // plus license button

    cy.get('#mat-select-value-3').click(); // status select input
    cy.get('#mat-option-5').click(); // select trial option
    cy.get('vs-datepicker [icon="xmark"]').click(); // clear date input
    cy.checkIfButtonIsDisabled('vs-header > header div[actions] .scroll-content > vs-button[icon="floppy-disk"] > button'); // save license button
    cy.checkIfButtonIsEnabled('vs-header > header div[actions] .scroll-content > vs-button[icon="plus"] > button'); // plus license button
    cy.get('#mat-option-6').should('not.exist');
    cy.get('#mat-input-9').type('01/12/2050');
    cy.checkIfButtonIsEnabled('vs-header > header div[actions] .scroll-content > vs-button[icon="floppy-disk"] > button'); // plus license button
    cy.get('#mat-select-value-3').click(); // status select input
    cy.get('#mat-option-7').click(); // select trial option

    cy.batchRequestStub([getAllLicensedAppsForFileQuota, fileTenantQuota, fileAppQuota, getInfrastructureConfigurations]);
    cy.get('#mat-tab-label-0-1').click(); // file quota tab
    cy.get('vs-layout div[actions] > vs-button[icon="plus"] > button').click(); // new loose app

    cy.wait(500);
    //cy.matchImageSnapshot('add-app-file-quota');
    cy.get('[label="common.cancel"] button').click();
    cy.get('.p-selectable-row > :nth-child(2)').eq(0).click(); // first item
    cy.wait(500);
    //cy.matchImageSnapshot('update-app-file-quota');
    cy.get('[label="common.cancel"]').click();
    cy.batchRequestStub([getAllProductsEmpty, getAllProducts, getInfrastructureConfigurations]);
    cy.get('#mat-tab-label-0-0').click(); // file bundle tab
    cy.wait(500);

    cy.batchRequestStub([getDomains, getAllLicensedApps, getAllAppsRequest, getSearchedApp, addAppToLicense, getInfrastructureConfigurations]);
    cy.get('#mat-tab-label-0-0').click(); // loose apps tab
    cy.get('vs-layout > div[actions] vs-button[icon="plus"] button').click(); // new loose app
    cy.get('.mat-mdc-menu-content > :nth-child(2)').click();
    cy.getVsInput('numberOfLicenses').click({ force: true }).clear({force: true}).blur({ force: true });
    cy.wait(100);
    cy.get('vs-input[formControlName="numberOfLicenses"] .mat-mdc-form-field-error').should('exist'); // input required error
    cy.get('.vs-label-error').should('exist'); // at least one license is required
    cy.checkIfButtonIsDisabled('[label="common.save"] button');
    cy.wait(500);
    //cy.matchImageSnapshot('insert-loose-app-error-2');
    cy.getVsInput('numberOfLicenses').type(10);
    cy.checkIfButtonIsDisabled('[label="common.save"] button');
    cy.wait(500);
    //cy.matchImageSnapshot('insert-loose-app-error-1');
    cy.get('[label="common.cancel"] button').click();
    cy.wait(500);
  });

  it('Save a new loose app', () => {
    cy.batchRequestStub([getDomains, getAllAppsRequest, getAllProducts, getAllLicensedApps, getSearchedApp, addAppToLicense, getInfrastructureConfigurations]);
    cy.visit('/license-management/licensings');
    cy.get('vs-layout > div[actions] vs-button[icon="plus"] button').click({ force: true }); // new loose app
    cy.get('.mat-menu-content > :nth-child(2)').click();
    cy.getVsInput('numberOfLicenses').clear({force: true}).type(15, { force: true });
    cy.checkIfButtonIsDisabled('[label="common.save"] button');
    cy.wait(500);
    //cy.matchImageSnapshot('insert-loose-app-error-3');
    cy.get('[ng-reflect-control-name="appName"] input').click({ force: true }); // search for 'new' app
    cy.wait(500);
    cy.getVsInput('search').type('new');
    cy.wait(500);
    //cy.matchImageSnapshot('find-in-app-grid');
    // select app in grid
    cy.batchRequestStub([getAllProductsAfterInsertApp]);
    cy.get('.select-modal-content tr.p-selectable-row:nth-child(1) > :nth-child(2)').click({ force: true }); // first item
    cy.get('vs-layout vs-select[formControlName="status"]').click();
    cy.get('.mat-option').eq(1).click();
    cy.get('[label="common.save"] button').click({ force: true });
    cy.wait(500);
    //cy.matchImageSnapshot('grid-with-inserted-loose-app');
  });

  // it('Edit new app, verify errors and save', () => {
  //   cy.batchRequestStub([updateLooseAppFromLicense, getAllProducts, getInfrastructureConfigurations]);
  //   cy.get('tr:nth-child(2) [ng-reflect-tooltip="Editar"] > button').click({ force: true });
  //   cy.getVsInput('numberOfLicenses').click().clear({force: true});
  //   // Some inconsistence is happening here
  //   cy.wait(250);
  //   cy.getVsInput('numberOfLicenses').blur({ force: true });
  //   cy.wait(500);
  //   cy.get('vs-input[formControlName="numberOfLicenses"] .mat-error').should('exist'); // input required error
  //   cy.checkIfButtonIsDisabled('[label="common.save"] button');
  //   cy.checkIfButtonIsDisabled('[label="common.save"] button');
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('edit-loose-app-error-1');
  //   cy.get('.vs-label-error').should('exist'); // at least one license is required
  //   cy.checkIfButtonIsDisabled('[label="common.save"] button');
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('edit-loose-app-error-2');
  //   cy.getVsInput('numberOfLicenses').click({ force: true }).type(50).blur({ force: true });
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('edit-loose-app-error-3');
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('edit-loose-app-filled');
  //   cy.get('[label="common.save"] button').click({ force: true });
  //   cy.wait(500);
  // });

  // it('delete one of the loose apps', () => {
  //   cy.batchRequestStub([removeAppFromLicenseError, getAllProducts, getInfrastructureConfigurations]);
  //   // Click in first delete item of grid
  //   cy.get('vs-layout tr').eq(2).get('vs-table-cell-actions > vs-button[ng-reflect-icon="trash-can"] > button').eq(1).click({ force: true });
  //   cy.wait(750);
  //   //cy.matchImageSnapshot('modal-delete-loose-app');
  //   cy.get('[label="SDK.MessageDialog.Yes"] button').click();
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('delete-loose-app-error');
  //   cy.get('mat-dialog-container [modal-footer-actions] button').click(); // ok button
  //   cy.batchRequestStub([removeAppFromLicenseSuccess, getAllProductsAfterDeleteApp, getInfrastructureConfigurations]);
  //   cy.get('tr:nth-child(2) [ng-reflect-tooltip="Apagar"] > button').click({force: true});
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('delete-loose-app-modal');
  //   cy.get('[label="SDK.MessageDialog.Yes"] button').click();
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('grid-after-delete-loose-app');
  // });

  // it('Add new bundle and verify form errors', () => {
  //   cy.batchRequestStub([getDomains, getAllProductsEmpty, getAllBundlesMinusLicensedBundles, getSearchedBundle, getInfrastructureConfigurations]);
  //   cy.get('#mat-tab-label-0-0').click({ force: true }); // bundle tabs
  //   cy.get('vs-layout > div[actions] vs-button[icon="plus"] button').click({ force: true }); // new bundle
  //   cy.get('.mat-menu-content > :nth-child(1)').click();
  //   cy.getVsInput('numberOfLicenses').click({ force: true }).clear({ force: true });
  //   cy.wait(250);
  //   cy.getVsInput('numberOfLicenses').blur({ force: true });
  //   cy.wait(250);
  //   cy.get('vs-input[formControlName="numberOfLicenses"] .mat-error').should('exist'); // input required error
  //   cy.get('.vs-label-error').should('exist'); // at least one license is required
  //   cy.checkIfButtonIsDisabled('[label="common.save"] button');
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('insert-bundle-error-1');
  //   cy.getVsInput('numberOfLicenses').type('100').blur({ force: true });
  //   cy.checkIfButtonIsDisabled('[label="common.save"] button');
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('insert-bundle-error-2');
  //   cy.checkIfButtonIsDisabled('[label="common.save"] button');
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('insert-bundle-error-3');
  //   cy.get('vs-layout > div[content] > vs-form > form > vs-search-input vs-icon').click({ force: true }); // open bundle modal
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('bundle-grid');
  //   cy.get('vs-layout vs-search > form > vs-input input').type('Three').blur({ force: true }); // bundle search input
  //   cy.get('vs-select-modal > vs-layout .p-selectable-row').eq(0).click({ force: true }); // first item
  //   cy.get('vs-layout vs-select[formControlName="status"]').click();
  //   cy.get('.mat-option').eq(1).click();
  //   cy.wait(250);
  //   //cy.matchImageSnapshot('insert-bundle-filled');
  //   cy.batchRequestStub([getAllProducts, getAllLicensedAppsInBundle, addBundleToLicense, getInfrastructureConfigurations]);
  //   cy.get('[label="common.save"] button').click({ force: true });
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('grids-after-new-bundle-inserted');
  // });

  // it('Delete one bundled app', () => {
  //   cy.batchRequestStub([removeAppFromLicenseSuccess, getAllProducts, getAllLicensedAppsInBundle, getDomains, getInfrastructureConfigurations]);
  //   cy.get('tr:nth-child(1) [ng-reflect-tooltip="Aplicativos"] > button').click({ force: true });
  //   cy.get('#mat-dialog-13 tr:nth-child(2) > td:nth-child(1) [ng-reflect-tooltip="Apagar"] > button').click({ force: true });  // delete a exists bundled app
  //   cy.get('h1.main_title').click({ force: true });
  //   cy.get('[label="SDK.MessageDialog.Yes"] > button').blur({ force: true });
  //   cy.batchRequestStub([getAllLicensedAppsInBundleAfterDelete]);
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('delete-bundled-app-modal');
  //   cy.get('[label="SDK.MessageDialog.Yes"] > button').click({ force: true });
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('grid-after-bundled-app-delete');
  //   cy.get('#mat-dialog-13 vs-button[icon="xmark"]').click();
  //   cy.wait(500);
  // });

  // it('Delete bundle', () => {
  //   cy.batchRequestStub([removeBundleFromLicenseError, getAllProducts, getInfrastructureConfigurations]);
  //   // select a exists bundled to delete
  //   cy.get('vs-layout tr').eq(1).get('vs-table-cell-actions > vs-button[ng-reflect-icon="trash-can"] > button').eq(0).click({ force: true });
  //   cy.get('[label="SDK.MessageDialog.Yes"] > button').click({ force: true });
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('delete-bundle-error');
  //   cy.get('[modal-footer-actions] > vs-button.ng-star-inserted > .vs-button-style').click({ force: true });
  //   cy.batchRequestStub([removeBundleFromLicenseSuccess, getAllProductsAfterDeleteProduct, getAllProductsEmpty, getInfrastructureConfigurations]);
  //   // select a exists bundled to delete
  //   cy.get('vs-layout tr').eq(1).get('vs-table-cell-actions > vs-button[ng-reflect-icon="trash-can"] > button').eq(0).click({ force: true });
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('delete-bundle-modal');
  //   cy.get('[label="SDK.MessageDialog.Yes"] > button').click({ force: true });
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('grid-after-bundled-delete');
  // });

  // it('Go to Organization Unit Tab and check grid data', () => {
  //   cy.batchRequestStub([getOrganizationUnitRequest, editOrganizationUnitRequest, getOrganizationEnvironmentRequest, editOrganizationEnvironmentRequest]);
  //   // Go to organization tab
  //   cy.get('#mat-tab-label-0-3').click({ force: true });
  //   cy.get('#mat-tab-label-0-3 .mat-ripple-element').should('not.exist')
  //   cy.get('app-license-detail-organization vs-grid .p-selectable-row').should('exist');
  //   //cy.matchImageSnapshot('organization-unit-grid');

  //   // Edit Organization Unit
  //   cy.get('app-license-detail-organization vs-grid vs-table-cell-actions vs-button').eq(1).click();
  //   cy.get('app-add-organization-unit-modal div[footer] vs-button').should('exist');
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('organization-unit-edit');
  //   cy.get('app-add-organization-unit-modal div[footer] vs-button').eq(1).click();
  //   cy.get('app-add-organization-unit-modal').should('not.exist');
  //   // Environment Grid
  //   cy.get('app-license-detail-organization vs-grid .p-selectable-row').eq(0).click().blur({ force: true });
  //   cy.get('app-licensings > vs-tabs-view-template > div[container]').scrollTo('bottom');
  //   cy.get('app-license-detail-organization vs-grid').should('have.length', 2);
  //   cy.get('app-license-detail-organization vs-grid .p-selectable-row').should('have.length', 5);
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('organization-environment-grid');
  //   // Edit Environment
  //   cy.get('app-license-detail-organization vs-grid .p-selectable-row').eq(3).click();
  //   cy.get('app-add-organization-environment-modal form vs-input').should('exist');
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('organization-environment-edit');
  //   // Go To file bundle tab
  //   cy.get('app-add-organization-environment-modal div[footer] vs-button').eq(1).click();
  //   cy.get('mat-dialog-container').should('not.exist');
  // })

  // it('Back to home page with new itens inserted', () => {
  //   cy.batchRequestStub([getAllRequestAfterInsert, getDomains, getAllProductsEmpty, getInfrastructureConfigurations]);
  //   cy.visit('/license-management/licensings');
  //   cy.get('vs-grid tbody tr').should('exist');
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('license-grid-after-insert');
  // });

  // it('Edit a existing license, check errors when clear inputs and button states', () => {
  //   cy.batchRequestStub([getLicenseById, getDomains, getAllProducts, getAllProductsEmpty, getOnlyActiveAccounts, getInfrastructureConfigurations]);
  //   cy.get('tr:nth-child(3) [ng-reflect-tooltip="Editar"] > button').click(); // select existis license
  //   cy.getVsInput('licensedCnpjs').type('1').clear().blur({ force: true });
  //   cy.get('#mat-select-0').click({ force: true }); // status select input
  //   cy.get('#mat-option-1 > .mat-option-text').click(); // status blocked option
  //   //cy.get('vs-select vs-icon').click();  // click on X icon to clear status
  //   cy.get('vs-search-input [icon="xmark"]').click(); // click on X icon to clear account
  //   cy.getVsInput('administratorEmail').type('a').clear().blur({ force: true });
  //   cy.get('vs-datepicker [icon="xmark"]').click();
  //   cy.get('[formControlName="notes"] textarea').click().clear().blur({ force: true });
  //   cy.get('[icon="plus"] > .nav-mode').scrollIntoView();
  //   cy.checkIfButtonIsEnabled(getPlusButtonProductTab()); // new bundle button
  //   cy.checkIfButtonIsDisabled('vs-header > header > div[actions] > vs-button[icon="floppy-disk"] > button'); // save license button
  //   cy.checkIfButtonIsEnabled('vs-header > header > div[actions] > vs-button[icon="plus"] > button'); // plus license button
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('edit-license-inputs-clear');
  // });

  // it('Edit a license and save', () => {
  //   cy.batchRequestStub([getOnlyActiveAccounts, getSearchedAccount, updateLicense,
  //     getDomains, getAllProductsEmpty, getInfrastructureConfigurations]);
  //   cy.get('vs-button[icon=magnifying-glass]').click();
  //   cy.get('vs-select-modal > vs-layout vs-search > form > vs-input input').type('Mari'); // search account input
  //   cy.get('#mat-dialog-0 tr.p-selectable-row:nth-child(2) > :nth-child(2)').click(); // second row
  //   cy.get('#mat-select-0').click({ force: true }); // status select input
  //   cy.get('#mat-option-1 > .mat-option-text').click(); // status trial option
  //   cy.getVsInput('administratorEmail').clear().type('admin@maribebidas.com.br')
  //   cy.get('#mat-input-0').type('01/12/2050');
  //   cy.get('[formControlName="notes"] textarea').type('Observações Editadas').blur({ force: true });
  //   cy.get('[icon="plus"] > .nav-mode').scrollIntoView();
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('edit-license-inputs-filled');
  //   cy.get('[icon="floppy-disk"] > .nav-mode').click();
  //   cy.checkIfButtonIsEnabled(getPlusButtonProductTab()); // new bundle button
  // });

  // it('Validate invalid administration email notification', () => {
  //   cy.batchRequestStub([getOnlyActiveAccounts, getAllProducts, getSearchedAccount, createLicenseWithInvalidAdministratorEmail, getLicenseById,
  //     getDomains, getAllProductsEmpty, getAllLooseLicensedApps, getDomains, getAllRequestAfterInsert, getInfrastructureConfigurations]);
  //   cy.get('[icon="plus"] > .nav-mode').click({ force : true });
  //   cy.get('app-license-detail > vs-form vs-search-input[controlname="accountName"] vs-icon').click({ force : true }); // open account input
  //   cy.get('vs-select-modal > vs-layout vs-search > form > vs-input input').invoke('val', 'Farmac') // search account input
  //   cy.get('[ng-reflect-p-selectable-row-disabled="false"][ng-reflect-index="0"] > :nth-child(2)').click({ force : true }); // select a account
  //   //cy.get('.mat-select-arrow-wrapper').click({ force: true }); // status select input
  //   //cy.get('#mat-option-6 > .mat-option-text').click(); // status active option
  //   cy.get('[icon="plus"] > .nav-mode').scrollIntoView();  // scroll to top for snap
  //   cy.get('[icon="floppy-disk"] > .nav-mode').click();
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('license-invalid-administration-email');
  // });

  // it('Validate invalid account notification', () => {
  //   cy.batchRequestStub([getOnlyActiveAccounts, getAllRequest, getAllProducts, getSearchedAccount, createLicenseWithInvalidAccount, getLicenseById,
  //     getDomains, getAllProductsEmpty, getAllLooseLicensedApps, getDomains, getAllRequestAfterInsert, getInfrastructureConfigurations]);
  //   cy.visit('/license-management/licensings');
  //   cy.get('[icon="plus"] > .nav-mode').click({ force: true });
  //   cy.get('app-license-detail > vs-form vs-search-input[controlname="accountName"] vs-icon').click({ force: true, multiple: true }); // open account input
  //   cy.get('vs-select-modal > vs-layout vs-search > form > vs-input input').type('Farmac').blur({ force: true }); // search account input
  //   cy.get('#mat-dialog-0 .p-selectable-row').eq(0).click({ force: true }); // select a account
  //   cy.get('[icon="plus"] > .nav-mode').scrollIntoView();  // scroll to top for snap
  //   cy.get('[icon="floppy-disk"] > .nav-mode').click();
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('license-invalid-account');
  // });

  // it('Validate invalid identifier notification', () => {
  //   cy.batchRequestStub([getOnlyActiveAccounts, getAllProducts, getSearchedAccount, createLicenseWithInvalidIdentifier, getLicenseById,
  //     getDomains, getAllProductsEmpty, getAllLooseLicensedApps, getDomains, getAllRequestAfterInsert, getInfrastructureConfigurations]);
  //   cy.visit('/license-management/licensings');
  //   cy.get('[icon="plus"] > .nav-mode').click({ force: true });
  //   cy.get('app-license-detail > vs-form vs-search-input[controlname="accountName"] vs-icon').click({ force: true, multiple: true }); // open account input
  //   cy.get('vs-select-modal > vs-layout vs-search > form > vs-input input').type('Farmac').blur({ force: true }); // search account input
  //   cy.get('#mat-dialog-0 .p-selectable-row').eq(0).click({ force: true }); // select a account
  //   cy.get('[icon="plus"] > .nav-mode').scrollIntoView();  // scroll to top for snap
  //   cy.get('vs-button[icon=floppy-disk"] button').should('be.enabled');
  //   cy.get('vs-button[icon="floppy-disk"] > .nav-mode').click({ force: true });
  //   cy.wait(500);
  //   //cy.matchImageSnapshot('license-invalid-identifier');
  // });
});
