import { Injectable } from '@angular/core';
import { Rules } from 'src/app/common/interfaces/Rules.interface';
import { AdvancedFilter } from 'src/app/common/interfaces/advancedFilter.interface';

@Injectable()
export class CustomFilterService {

    constructor() {}

    getAdvancedFilter(advancedFilter: AdvancedFilter) {
        const mainFilter = {
            condition: 'and',
            rules: []
        } as AdvancedFilter;
        let ruleToDelete = this.getRuleWithFieldName(advancedFilter);
        if (ruleToDelete !== undefined) {
            mainFilter.rules.push(
                this.createCustomRulesForAppNameBundleNameAndUserName(advancedFilter, ruleToDelete)
            );
        }
        ruleToDelete = this.getRuleWithFieldDescription(advancedFilter);
        if (ruleToDelete !== undefined) {
            mainFilter.rules.push(
                this.createCustomRulesForAppIdentifierAndBundleIdentifier(advancedFilter, ruleToDelete)
            );
        }
        ruleToDelete = this.getRuleWithFieldAdditional(advancedFilter);
        if (ruleToDelete !== undefined) {
            mainFilter.rules.push(
                this.createCustomRulesForAdditionalLicenses(advancedFilter, ruleToDelete)
            );
        }
        ruleToDelete = this.getRuleWithFieldTotal(advancedFilter);
        if (ruleToDelete !== undefined) {
            mainFilter.rules.push(
                this.createCustomRulesForTotal(advancedFilter, ruleToDelete)
            );
        }
        return JSON.stringify(mainFilter);
    }

    getRuleWithFieldName(advancedFilter: AdvancedFilter) {
    let ruleToReturn;
    advancedFilter.rules.forEach(rule => {
        if (rule.field === 'name') {
        ruleToReturn = rule;
        }
    });
    return ruleToReturn;
    }

    getRuleWithFieldDescription(advancedFilter: AdvancedFilter) {
    let ruleToReturn;
    advancedFilter.rules.forEach(rule => {
        if (rule.field === 'description') {
        ruleToReturn = rule;
        }
    });
    return ruleToReturn;
    }

    getRuleWithFieldAdditional(advancedFilter: AdvancedFilter) {
        let ruleToReturn;
        advancedFilter.rules.forEach(rule => {
            if (rule.field === 'additional') {
                ruleToReturn = rule;
            }
        });
        return ruleToReturn;
    }

    getRuleWithFieldStartTime(advancedFilter: AdvancedFilter) {
        let ruleToReturn;
        advancedFilter.rules.forEach(rule => {
            if (rule.field === 'startTime') {
                ruleToReturn = rule;
            }
        });
        return ruleToReturn;
    }

    getRuleWithFieldTotal(advancedFilter: AdvancedFilter) {
        let ruleToReturn;
        advancedFilter.rules.forEach(rule => {
            if (rule.field === 'total') {
                ruleToReturn = rule;
                if (isNaN(Number(ruleToReturn.value))) {
                    ruleToReturn = undefined;
                }
            }
        });
        return ruleToReturn;
    }

    createCustomRulesForAppNameBundleNameAndUserName(advancedFilter: AdvancedFilter, ruleToDelete: any) {
        const index = advancedFilter.rules.indexOf(ruleToDelete);
        advancedFilter.rules.splice(index, 1);
        advancedFilter.rules.push(
        {
            field: 'appName',
            operator: ruleToDelete.operator,
            type: ruleToDelete.type,
            value: ruleToDelete.value
        } as Rules
        );
        advancedFilter.rules.push(
        {
            field: 'bundleName',
            operator: ruleToDelete.operator,
            type: ruleToDelete.type,
            value: ruleToDelete.value
        } as Rules
        );
        advancedFilter.rules.push(
        {
            field: 'user',
            operator: ruleToDelete.operator,
            type: ruleToDelete.type,
            value: ruleToDelete.value
        } as Rules
        );
        advancedFilter.condition = 'or';
        return advancedFilter;
    }

    createCustomRulesForAppIdentifierAndBundleIdentifier(advancedFilter: AdvancedFilter, ruleToDelete: any) {
        const index = advancedFilter.rules.indexOf(ruleToDelete);
        advancedFilter.rules.splice(index, 1);
        advancedFilter.rules.push(
            {
            field: 'appIdentifier',
            operator: ruleToDelete.operator,
            type: ruleToDelete.type,
            value: ruleToDelete.value
            } as Rules
        );
        advancedFilter.rules.push(
            {
            field: 'bundleIdentifier',
            operator: ruleToDelete.operator,
            type: ruleToDelete.type,
            value: ruleToDelete.value
            } as Rules
        );
        advancedFilter.condition = 'or';
        return advancedFilter;
    }

    createCustomRulesForAdditionalLicenses(advancedFilter: AdvancedFilter, ruleToDelete: any) {
        const index = advancedFilter.rules.indexOf(ruleToDelete);
        advancedFilter.rules.splice(index, 1);
        advancedFilter.rules.push(
            {
                field: 'additionalLicenses',
                operator: 'equal',
                type: 'integer',
                value: ruleToDelete.value
            } as Rules
        );
        advancedFilter.condition = 'or';
        return advancedFilter;
    }

    createCustomRulesForTotal(advancedFilter: AdvancedFilter, ruleToDelete: any) {
        const index = advancedFilter.rules.indexOf(ruleToDelete);
        advancedFilter.rules.splice(index, 1);
        advancedFilter.rules.push(
            {
                field: 'appLicenses',
                operator: 'equal',
                type: 'integer',
                value: ruleToDelete.value
            } as Rules
        );
        advancedFilter.condition = 'or';
        return advancedFilter;
    }
}
