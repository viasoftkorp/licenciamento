import { Injectable } from '@angular/core';
import { JQQB_COND_OR, JQQB_OP_CONTAINS, JQQB_OP_EQUAL } from '@viasoft/common';
import { VsFilterGetItemsInput, VsFilterGetItemsOutput, VsFilterItem, VsFilterOptions } from '@viasoft/components';
import { DomainServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';
import { domainNames } from '@viasoft/licensing-management/clients/licensing-management/model/domainNames';
import { Observable, of } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class DomainsService {

    listOfDomains: string[];

    constructor(private domainServiceProxy: DomainServiceProxy) { }

    getDomains() {
        return this.domainServiceProxy.getDomains();
    }

    getDomainsFilterOptions(listOfDomains: string[]): VsFilterOptions {
        this.listOfDomains = listOfDomains;
        return ({
            mode: 'selection',
            blockInput: true,
            operators: [JQQB_OP_EQUAL],
            multiple: true,
            getItems: (input: VsFilterGetItemsInput) => this.getDomainNames(input),
            getItemsFilterOperator: JQQB_OP_CONTAINS,
            conditions: [JQQB_COND_OR]
        } as VsFilterOptions);
    }

    private getDomainNames(input: VsFilterGetItemsInput): Observable<VsFilterGetItemsOutput> {
        const items = this.listOfDomains.slice(input.skipCount, input.skipCount + input.maxResultCount);
        return of({
            items: items.map(domain => ({
                key: domainNames[domain],
                value: `apps.domains.${domain}`
            } as VsFilterItem)),
            totalCount: this.listOfDomains.length
        } as VsFilterGetItemsOutput)
    }
}
