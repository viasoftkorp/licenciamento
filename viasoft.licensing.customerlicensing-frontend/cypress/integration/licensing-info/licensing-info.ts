import { GetTenantInfoFromId } from "cypress/support/requests/license";
import { GetAllProducts, GetAllLicenseUsage } from "cypress/support/requests/product";

const getGridIconByRowIndex = (index: number, icon: string) => `vs-grid tbody > :nth-child(${index}) vs-icon[ng-reflect-icon="${icon}"`;
const getGridCell = (row: number, column: number) => `vs-grid tbody > :nth-child(${row}) > :nth-child(${column})`;

enum LicensingModels {
    Simultânea,
    Nomeada
}

enum LicensingModes {
    Online,
    Offline
}

enum SubscriptionStatus {
    Bloqueado,
    Ativo
}

describe('licensing-info', () => {
    visitLicensingInfoScreen();
    validateLicensingInformation();
    validateLicensedBundlesGrid();
});

function visitLicensingInfoScreen() {
    it('Visit licensing info screen', () => {
        cy.intercept(
            GetTenantInfoFromId.method,
            GetTenantInfoFromId.url,
            GetTenantInfoFromId.response
        ).as('GetTenantInfoFromId');
        cy.intercept(
            GetAllProducts.method,
            GetAllProducts.url,
            GetAllProducts.response
        ).as('GetAllProducts');
        cy.intercept(
            GetAllLicenseUsage.method,
            GetAllLicenseUsage.url,
            GetAllLicenseUsage.response
        ).as('GetAllLicenseUsage');
        cy.visit('/licensing-info');
        cy.wait('@GetTenantInfoFromId');
        cy.wait('@GetAllProducts');
        cy.wait('@GetAllLicenseUsage');
        cy.get('vs-grid', { timeout: 50000 }).should('exist');
    })
}

function validateLicensingInformation() {
    it('Validate licensing information', () => {
        cy.get('[data-cy="status"] > vs-label').should('contain', 'Ativo');
        cy.get('[data-cy="licensedCnpjs"] > ul > li > vs-label').should('contain', '03.623.045/0001-00');
    })
}

function validateLicensedBundlesGrid() {
    it('Validate licensed bundle grid', () => {
        const items = GetAllProducts.response.items;

        cy.get(getGridIconByRowIndex(2, "check-circle")).should('exist');

        cy.get(getGridIconByRowIndex(3, "check-circle")).should('exist');

        validateLicensingModelColumn(items);
        validateLicensingModeColumn(items);
        validateSubscriptionStatusColumn(items);
    })
}

function validateLicensingModelColumn(items: any[]){
    const indexColumn = 5;
    items.map((item: any, index: number) => {
        cy.get(getGridCell(index + 1, indexColumn)).should('contain', LicensingModels[item.licensingModel]);
    })
}

function validateLicensingModeColumn(items: any[]){
    const indexColumn = 6;
    items.map((item: any, index: number) => {
        if(item.licensingModel == 1){
            cy.get(getGridCell(index + 1, indexColumn)).should('contain', LicensingModes[item.licensingMode]);
        }
    })
}

function validateSubscriptionStatusColumn(items: any[]){
    const indexColumn = 7;
    items.map((item: any, index: number) => {
        cy.get(getGridCell(index + 1, indexColumn)).should('contain', SubscriptionStatus[item.status]);
    })
}