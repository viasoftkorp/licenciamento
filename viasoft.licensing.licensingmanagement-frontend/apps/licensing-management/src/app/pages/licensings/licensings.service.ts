import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Optional } from '@angular/core';
import { Injectable } from '@angular/core';
import { VsGridGetInput } from '@viasoft/components';
import { API_GATEWAY } from '@viasoft/http';
import { LicensingFormCurrentTab } from '@viasoft/licensing-management/app/tokens/enum/licensing-form-current-tab.enum';
import {
  BundleServiceProxy,
  LicensedAppCreateInput,
  LicensedAppCreateOutput,
  LicensedAppDeleteInput,
  LicensedAppUpdateInput,
  LicensedBundleCreateInput,
  LicensedBundleDeleteInput,
  LicensedBundleUpdateInput,
  LicensingServiceProxy,
} from '@viasoft/licensing-management/clients/licensing-management';
import { ProductServiceProxy } from '@viasoft/licensing-management/clients/licensing-management/api/productServiceProxy';
import { LicensedProductOutputPagedResult } from '@viasoft/licensing-management/clients/licensing-management/model/LicensedProductOutputPagedResult';
import { Observable, Subject } from 'rxjs';
import { LicenseGetAllInput } from '../../tokens/inputs/license-get-all.input';
import { LicenseTenantCreateInput } from '../../tokens/interfaces/license-tenant-create-input.interface';
import { LicenseTenantCreateOutput } from '../../tokens/interfaces/license-tenant-create-output.interface';
import { LicenseTenantUpdateInput } from '../../tokens/interfaces/license-tenant-update-input.interface';
import { LicenseTenantUpdateOutput } from '../../tokens/interfaces/license-tenant-update-output.interface';
import { PagedResultLicenseTenantCreateOutput } from '../../tokens/interfaces/paged-result-license-tenant-create-output.interface';
@Injectable({
  providedIn: 'root'
})
export class LicensingsService {
  private tenantId: string;
  currentTab: LicensingFormCurrentTab;
  licensedTenant = {
    LicensedTenantId: null,
    LicensendTenantStatus: null,
    deviceId: null,
    licensedTenantIdentifier: null
  };
  currentTabSubject: Subject<LicensingFormCurrentTab> = new Subject<LicensingFormCurrentTab>();

  constructor(
    private licenses: LicensingServiceProxy,
    private bundles: BundleServiceProxy,
    private readonly httpClient: HttpClient,
    private products: ProductServiceProxy,
    @Optional() @Inject(API_GATEWAY) protected basePath: string
  ) { }

  getAll(input: LicenseGetAllInput) {
    const route = `${this.basePath}Licensing/LicensingManagement/Licensing/GetAll`;

    let params = new HttpParams();
    if (input.filter !== undefined && input.filter !== null && input.filter !== '') {
      params = params.set('Filter', input.filter);
    }
    if (input.advancedFilter !== undefined && input.advancedFilter !== null && input.advancedFilter !== '') {
      params = params.set('AdvancedFilter', input.advancedFilter);
    }
    if (input.sorting !== undefined && input.sorting !== null && input.sorting !== '') {
      params = params.set('Sorting', input.sorting);
    }
    if (input.skipCount !== undefined && input.skipCount !== null) {
      params = params.set('SkipCount', input.skipCount.toString());
    }
    if (input.maxResultCount !== undefined && input.maxResultCount !== null) {
      params = params.set('MaxResultCount', input.maxResultCount.toString());
    }

    return this.httpClient.get<PagedResultLicenseTenantCreateOutput>(route, { params });
  }

  public getById(id: string): Observable<LicenseTenantCreateOutput> {
    const route = `${this.basePath}Licensing/LicensingManagement/Licensing/GetById?id=${id}`;
    return this.httpClient.get<LicenseTenantCreateOutput>(route);
  }

  public create(license: LicenseTenantCreateInput): Observable<LicenseTenantCreateOutput> {
    const route = `${this.basePath}Licensing/LicensingManagement/Licensing/Create`;
    return this.httpClient.post<LicenseTenantCreateOutput>(route, license);
  }

  public update(license: LicenseTenantUpdateInput): Observable<LicenseTenantUpdateOutput> {
    const route = `${this.basePath}Licensing/LicensingManagement/Licensing/Update`;
    return this.httpClient.post<LicenseTenantUpdateOutput>(route, license);
  }

  delete(id: string) {
    return this.licenses._delete(id);
  }

  getAllLicensedProduct(input: VsGridGetInput, licensedTenantId: string): Observable<LicensedProductOutputPagedResult> {
    return this.products.getAll(input, licensedTenantId);
  }

  getAllLicensedBundle(
    licensedTenantId: string,
    filter: string,
    advancedFilter: string,
    sorting: string,
    skipCount: number,
    maxResultCount: number
  ) {
    return this.bundles.getAllLicensedBundle(
      licensedTenantId,
      filter,
      advancedFilter,
      sorting,
      skipCount,
      maxResultCount);
  }

  getAllLicensedAppsInBundle(
    licensedTenantId: string,
    licensedBundleId: string,
    filter: string,
    advancedFilter: string,
    sorting: string,
    skipCount: number,
    maxResultCount: number
  ) {
    return this.licenses.getAllLicensedAppsInBundle(
      licensedTenantId,
      licensedBundleId,
      filter,
      advancedFilter,
      sorting,
      skipCount,
      maxResultCount
    );
  }

  getAllLooseLicensedApps(
    licensedTenantId: string,
    filter: string,
    advancedFilter: string,
    sorting: string,
    skipCount: number,
    maxResultCount: number
  ) {
    return this.licenses.getAllLooseLicensedApps(
      licensedTenantId,
      filter,
      advancedFilter,
      sorting,
      skipCount,
      maxResultCount
    );
  }

  public getAllLicensedApps(
    licensedTenantId: string,
    filter?: string,
    advancedFilter?: string,
    sorting?: string,
    skipCount?: number,
    maxResultCount?: number
  ) {
    return this.licenses.getAllLicensedApps(
      licensedTenantId,
      filter,
      advancedFilter,
      sorting,
      skipCount,
      maxResultCount
    );
  }

  addBundleToLicense(license: LicensedBundleCreateInput) {
    return this.licenses.addBundleToLicense(license);
  }

  removeBundleFromLicense(license: LicensedBundleDeleteInput) {
    return this.licenses.removeBundleFromLicense(license);
  }

  updateBundleFromLicense(license: LicensedBundleUpdateInput) {
    return this.licenses.updateBundleFromLicense(license);
  }

  addAppToLicense(license: LicensedAppCreateInput): Observable<LicensedAppCreateOutput> {
    return this.licenses.addAppToLicense(license);
  }

  removeAppFromLicense(license: LicensedAppDeleteInput) {
    return this.licenses.removeAppFromLicense(license);
  }

  updateBundledAppFromLicense(license: LicensedAppUpdateInput) {
    return this.licenses.updateBundledAppFromLicense(license);
  }

  updateLooseAppFromLicense(license: LicensedAppUpdateInput) {
    return this.licenses.updateLooseAppFromLicense(license);
  }

  public getCurrentTab() {
    return this.currentTab;
  }

  public setCurrentTab(tab: LicensingFormCurrentTab): void {
    this.currentTab = tab;
    this.setCurrentTabSubject(tab);
  }

  noCurrentTab() {
    this.setCurrentTabSubject(undefined);
    return this.currentTab = undefined;
  }

  public getTenantId(): string {
    return this.tenantId;
  }

  public setTenantId(id: string): void {
    this.tenantId = id;
  }

  public clearTenantId(): void {
    this.tenantId = undefined;
  }

  public setLicensedTenantId(id: string) {
    return this.licensedTenant.LicensedTenantId = id;
  }

  public getLicensedTenantId() {
    return this.licensedTenant.LicensedTenantId;
  }

  public clearLicensedTenantId() {
    return this.licensedTenant.LicensedTenantId = null;
  }

  public setTenantStatus(status: any) {
    return this.licensedTenant.LicensendTenantStatus = status;
  }

  public clearTenantStatus() {
    return this.licensedTenant.LicensendTenantStatus = null;
  }

  public setDeviceId(deviceId: string): void {
    this.licensedTenant.deviceId = deviceId;
  }

  public getDeviceId(): string {
    return this.licensedTenant.deviceId;
  }

  public clearDeviceId(): void {
    this.licensedTenant.deviceId = null;
  }

  public setLicensedTenantIdentifier(licensedTenantIdentifier: string): void {
    this.licensedTenant.licensedTenantIdentifier = licensedTenantIdentifier;
  }

  public getLicensedTenantIdentifier(): string {
    return this.licensedTenant.licensedTenantIdentifier;
  }

  public clearLicensedTenantIdentifier(): void {
    this.licensedTenant.licensedTenantIdentifier = null;
  }

  public setCurrentTabSubject(input: LicensingFormCurrentTab) {
    this.currentTabSubject.next(input);
  }
}
