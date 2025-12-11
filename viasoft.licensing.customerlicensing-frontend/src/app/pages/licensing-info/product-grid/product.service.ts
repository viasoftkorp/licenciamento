import { Injectable } from "@angular/core";
import { VsGridGetInput } from "@viasoft/components";
import { BehaviorSubject, Observable, of, Subject } from "rxjs";
import { ProductType } from "src/app/common/enums/ProductType";
import { ProductServiceProxy } from "src/client/customer-licensing/api/ProductServiceProxy";
import { ProductOutput } from "src/client/customer-licensing/model/ProductOutput";
import { ProductOutputPagedResultDto } from "src/client/customer-licensing/model/ProductOutputPagedResultDto";
import { LicenseUsageOutput } from "src/client/customer-licensing/model/LicenseUsageOutput";

@Injectable()
export class ProductService {
    public licenseUsage: BehaviorSubject<LicenseUsageOutput[]> = new BehaviorSubject<LicenseUsageOutput[]>([]);

    constructor(private readonly serviceProxy: ProductServiceProxy) {}

    public getAll(input: VsGridGetInput, licensedTenantId: string): Observable<ProductOutputPagedResultDto> {
        return this.serviceProxy.getAll(input, licensedTenantId);
    }

    public getAllLicenseUsage(licensingIdentifier: string, bundleIdentifiers: string[], appIdentifiers: string[]): Observable<LicenseUsageOutput[]> {
        return this.serviceProxy.getAllLicenseUsage(licensingIdentifier, bundleIdentifiers, appIdentifiers);
    }

    public getById(licensedTenantId: string, bundleIdentifier: string, productType: ProductType, licensingIdentifier: string): Observable<ProductOutput>{
        return this.serviceProxy.getById(licensedTenantId, bundleIdentifier, productType, licensingIdentifier);
    }
}