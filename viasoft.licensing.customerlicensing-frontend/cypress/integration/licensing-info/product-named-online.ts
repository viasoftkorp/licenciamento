import { GetTenantInfoFromId } from "cypress/support/requests/license";
import { GetProductNamedOnlineById } from "cypress/support/requests/product";
import { AddNamedUserToProductSuccessfully, GetAllUsers, RemoveNamedUserFromProductSuccessfully, UpdateNamedUserFromProductSuccessfully } from "cypress/support/requests/named-user";
import { GetUserBehaviourFromProductNamedOnline } from "cypress/support/requests/user-behaviour";

describe('product-named-online', () => {
    visitLicensedBundleScreen();
    canAddNamedUserToLicensedBundleSuccessfully();
    canRemoveNamedUserFromBundleSuccessfylly();
    canUpdateNamedUserFromBundleSuccessfully();
})

function visitLicensedBundleScreen() {
    it('Visit licensed bundle named online screen', () => {
        cy.intercept(
            GetTenantInfoFromId.method,
            GetTenantInfoFromId.url,
            GetTenantInfoFromId.response
        ).as('GetTenantInfoFromId');
        cy.intercept(
            GetProductNamedOnlineById.method,
            GetProductNamedOnlineById.url,
            GetProductNamedOnlineById.response
        ).as('GetProductNamedOnlineById');
        cy.intercept(
            GetUserBehaviourFromProductNamedOnline.method,
            GetUserBehaviourFromProductNamedOnline.url,
            GetUserBehaviourFromProductNamedOnline.response
        ).as('GetUserBehaviourFromProductNamedOnline');
        cy.visit('customer-licensing/licensing-info/product/0/f05b29a2-4dd1-4466-fa6c-08d97866fc96');
        cy.wait('@GetTenantInfoFromId');
        cy.wait('@GetProductNamedOnlineById');
        cy.wait('@GetUserBehaviourFromProductNamedOnline');
        cy.get('vs-grid').should('exist');
    })
}

function canAddNamedUserToLicensedBundleSuccessfully() {
    it('Can add named user to licensed bundle successfully?', () => {
        cy.get('.title-container > vs-button').should('exist').click();
        cy.get('vs-dialog').should('exist');
        cy.get('vs-dialog vs-label').should('contain', 'Atribuir licença');

        selectUserToAutocomplete();
        
        cy.intercept(
            AddNamedUserToProductSuccessfully.method,
            AddNamedUserToProductSuccessfully.url,
            AddNamedUserToProductSuccessfully.response
        ).as('AddNamedUserToProductSuccessfully');
        cy.intercept(
            GetUserBehaviourFromProductNamedOnline.method,
            GetUserBehaviourFromProductNamedOnline.url,
            GetUserBehaviourFromProductNamedOnline.response
        ).as('GetUserBehaviourFromProductNamedOnline');
        cy.get('vs-button[type="save"] > button').should('be.enabled').click();
        cy.wait('@AddNamedUserToProductSuccessfully');
        cy.wait('@GetUserBehaviourFromProductNamedOnline');
    })
}

function canRemoveNamedUserFromBundleSuccessfylly() {
    it('Can remove named user successfully?', () => {
        cy.get('vs-grid tbody > :nth-child(1) vs-button vs-icon[ng-reflect-icon="trash-alt"]').click();
        cy.get('#mat-dialog-1 .title').should('contain', 'Você tem certeza de que deseja remover "admin@korp.com.br"?');
        cy.get('#mat-dialog-1 vs-button[type="cancel"] button').should('be.enabled');
        cy.intercept(
            RemoveNamedUserFromProductSuccessfully.method,
            RemoveNamedUserFromProductSuccessfully.url,
            RemoveNamedUserFromProductSuccessfully.response
        ).as('RemoveNamedUserFromProductSuccessfully');
        cy.intercept(
            GetUserBehaviourFromProductNamedOnline.method,
            GetUserBehaviourFromProductNamedOnline.url,
            GetUserBehaviourFromProductNamedOnline.response
        ).as('GetUserBehaviourFromProductNamedOnline');
        cy.get('#mat-dialog-1 vs-button[type="save"] button').should('be.enabled').click();
        cy.wait('@RemoveNamedUserFromProductSuccessfully');
        cy.wait('@GetUserBehaviourFromProductNamedOnline');
    })
}

function canUpdateNamedUserFromBundleSuccessfully() {
    it('Can update named user from bundle successfully?', () => {
        cy.get('vs-grid tbody > :nth-child(1) vs-button vs-icon[ng-reflect-icon="arrow-alt-right"]').click();
        cy.get('vs-layout > vs-label > span').should('contain', 'Transferir licença');
        cy.get('vs-autocomplete-select vs-icon').click();
        selectUserToAutocomplete();
        cy.intercept(
            UpdateNamedUserFromProductSuccessfully.method,
            UpdateNamedUserFromProductSuccessfully.url,
            UpdateNamedUserFromProductSuccessfully.response
        ).as('UpdateNamedUserFromProductSuccessfully');
        cy.intercept(
            GetUserBehaviourFromProductNamedOnline.method,
            GetUserBehaviourFromProductNamedOnline.url,
            GetUserBehaviourFromProductNamedOnline.response
        ).as('GetUserBehaviourFromProductNamedOnline');
        cy.get('vs-button[type="save"] > button').should('be.enabled').click();
        cy.wait('@UpdateNamedUserFromProductSuccessfully');
        cy.wait('@GetUserBehaviourFromProductNamedOnline');
    })
}

function selectUserToAutocomplete() {
    cy.intercept(
        GetAllUsers.method,
        GetAllUsers.url,
        GetAllUsers.response
    ).as('GetAllUsers');
    cy.wait(300);
    cy.get('vs-autocomplete-select input').click();
    cy.wait('@GetAllUsers');
    cy.get('vs-button.option').eq(0).click();
}