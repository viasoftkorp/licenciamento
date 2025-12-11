import { GetTenantInfoFromId } from "cypress/support/requests/license";
import { GetProductFloatingById } from "cypress/support/requests/product";
import { GetUserBehaviourFromProductFloating } from "cypress/support/requests/user-behaviour";

describe('product-floating', () => {
    visitLicensedBundleScreen();
    validateLicensedBundleInformations();
})

function visitLicensedBundleScreen() {
    it('Visit licensed bundle floating screen', () => {
        cy.intercept(
            GetTenantInfoFromId.method,
            GetTenantInfoFromId.url,
            GetTenantInfoFromId.response
        ).as('GetTenantInfoFromId');
        cy.intercept(
            GetProductFloatingById.method,
            GetProductFloatingById.url,
            GetProductFloatingById.response
        ).as('GetProductFloatingById');
        cy.intercept(
            GetUserBehaviourFromProductFloating.method,
            GetUserBehaviourFromProductFloating.url,
            GetUserBehaviourFromProductFloating.response
        ).as('GetUserBehaviourFromProductFloating');
        cy.visit('customer-licensing/licensing-info/product/0/f05b29a2-4dd1-4466-fa6c-08d97866fc96');
        cy.wait('@GetTenantInfoFromId');
        cy.wait('@GetProductFloatingById');
        cy.wait('@GetUserBehaviourFromProductFloating');
        cy.get('vs-grid').should('exist');
    })
}

function validateLicensedBundleInformations() {
    it('Validate licensed bundle informations', () => {
        cy.get('[data-cy="name"] > vs-label').should('contain', 'Pacote Default');

        cy.get('[data-cy="status"] > vs-icon[ng-reflect-icon="check-circle"]').should('exist');
        cy.get('[data-cy="status"] > vs-label').should('contain', 'Ativo');

        cy.get('[data-cy="licenses"] > vs-label span').should('contain', '1');
    })
}