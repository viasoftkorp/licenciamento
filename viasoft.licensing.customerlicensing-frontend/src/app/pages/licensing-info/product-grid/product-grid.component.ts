import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { JQQB_OP_EQUAL, VsSubscriptionManager } from '@viasoft/common';
import { VsFilterGetItemsOutput, VsFilterItem, VsGridAction, VsGridGetInput, VsGridGetResult, VsGridNumberColumn, VsGridOptions, VsGridSimpleColumn } from '@viasoft/components';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { ProductType } from 'src/app/common/enums/ProductType';
import { ProductOutput } from 'src/client/customer-licensing/model/ProductOutput';
import { LicensingModels } from 'src/client/customer-licensing/model/LicensingModels';
import { LicensingModes } from 'src/client/customer-licensing/model/LicensingModes';
import { LicensingsService } from '../../licensings/licensings.service';
import { ProductService } from './product.service';
import { NumberOfUsedLicensesCellComponent } from './number-of-licenses-cell/number-of-used-licenses-cell.component';
import { StatusCellComponent } from './status-cell/status-cell.component';
import { ProductNameCellComponent } from './product-name-cell/product-name-cell.component';

@Component({
  selector: 'app-product-grid',
  templateUrl: './product-grid.component.html',
  styleUrls: ['./product-grid.component.scss']
})
export class ProductGridComponent implements OnInit, OnDestroy {

  gridOptions: VsGridOptions;
  @Input() licensedTenantId: string;
  licensingIdentifier: string;
  subscriptions: VsSubscriptionManager = new VsSubscriptionManager();

  private licensingModelFilterItems: VsFilterItem[] =  [
    {
      key: '0',
      value: 'LicensingInfo.Models.Floating'
    },
    {
      key: '1',
      value: 'LicensingInfo.Models.Named'
    }
  ];


  private licensingModeFilterItems: VsFilterItem[] = [
    {
      key: '0',
      value: 'LicensingInfo.Modes.Online'
    },
    {
      key: '1',
      value: 'LicensingInfo.Modes.Offline'
    }
  ];

  private subscriptionStatusFilterItems: VsFilterItem[] = [
    {
      key: '0',
      value: 'LicensingInfo.Products.Status.BundleBlocked'
    },
    {
      key: '1',
      value: 'LicensingInfo.Products.Status.BundleActive'
    }
  ]

  constructor(
    private productService: ProductService,
    private licensingsService: LicensingsService,
  ) { }

  ngOnInit(): void {
    this.configureGrid();
    this.licensingIdentifier = this.licensingsService.getTenantId();
  }

  configureGrid() {
    this.gridOptions = new VsGridOptions();
    this.gridOptions.id = 'a4581bd7-ecf2-47e1-83bc-b480baa27392';

    this.gridOptions.columns = [
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.Products.Name',
        field: 'name',
        width: 200,
        type: ProductNameCellComponent,
      }),
      new VsGridNumberColumn({
        headerName: 'LicensingInfo.Products.NumberOfLicenses',
        field: 'numberOfLicenses',
        width: 100,
        kind: 'number',
        filterOptions: {
          operators: [JQQB_OP_EQUAL],
        },
      }),
      new VsGridNumberColumn({
        headerName: 'LicensingInfo.Products.UsedLicenses',
        field: 'numberOfUsedLicenses',
        width: 100,
        type: NumberOfUsedLicensesCellComponent,
        sorting: {
          disable: true
        },
        filterOptions: { disable: true }
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.Products.LicensingModel',
        field: 'licensingModel',
        disabled: true,
        width: 100,
        format: (data: any) => 'LicensingInfo.Models.' + LicensingModels[data],
        translate: true,
        filterOptions: {
          operators: [JQQB_OP_EQUAL],
          mode: 'selection',
          multiple: true,
          getItems: () => this.getLicensingModelFilterColumn(),
          getValidItems: (keys: any[]) => {
            return of(this.licensingModelFilterItems.filter(filterItem =>
              keys.includes(filterItem.key)
            ))
          }
        },
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.Products.LicensingMode',
        field: 'licensingMode',
        disabled: true,
        width: 100,
        format: (data: any) => data != null ? 'LicensingInfo.Modes.' + LicensingModes[data] : '',
        translate: true,
        filterOptions: {
          operators: [JQQB_OP_EQUAL],
          mode: 'selection',
          multiple: true,
          getItems: () => this.getLicensingModeFilterColumn(),
          getValidItems: (keys: any[]) => {
            return of(this.licensingModeFilterItems.filter(filterItem =>
              keys.includes(filterItem.key)
            ))
          }
        },
        kind: "number"
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.Products.SubscriptionStatus',
        field: 'status',
        disabled: true,
        width: 100,
        type: StatusCellComponent,
        filterOptions: {
          operators: [JQQB_OP_EQUAL],
          mode: 'selection',
          multiple: true,
          getItems: () => this.getSubscriptionStatusFilterColumn(),
          getValidItems: (keys: any[]) => {
            return of(this.subscriptionStatusFilterItems.filter(filterItem =>
              keys.includes(filterItem.key)
            ))
          }
        }
      }),
    ];

    this.gridOptions.get = (input: VsGridGetInput) => this.get(input);
  }

  get(input: VsGridGetInput): Observable<any> {
    return this.productService.getAll(input, this.licensedTenantId)
      .pipe(
        map((list: any) => {
            this.getLicensesConsumed(list.items);
            return new VsGridGetResult(list.items, list.totalCount);
          }
        )
      );
  }

  getSubscriptionStatusFilterColumn(): Observable<VsFilterGetItemsOutput>{
    return of({
      items: this.subscriptionStatusFilterItems,
      totalCount: this.subscriptionStatusFilterItems.length
    })
  }

  getLicensingModeFilterColumn(): Observable<VsFilterGetItemsOutput>{
    return of({
      items: this.licensingModeFilterItems,
      totalCount: this.licensingModeFilterItems.length
    })
  }

  getLicensingModelFilterColumn(): Observable<VsFilterGetItemsOutput>{
    return of({
      items: this.licensingModelFilterItems,
      totalCount: this.licensingModelFilterItems.length
    })
  }

  private getLicensesConsumed(items: ProductOutput[]): void {
    const bundleIdentifiers: string[] = this.resolveBundleIdentifiers(items);
    const appIdentifiers: string[] = this.resolveAppIdentifiers(items);
    this.subscriptions.add('getAllLicenseUsage', this.productService.getAllLicenseUsage(this.licensingIdentifier, bundleIdentifiers, appIdentifiers)
      .subscribe((value) => {
        this.productService.licenseUsage.next(value);
      }))
  }

  private resolveBundleIdentifiers(items: ProductOutput[]): string[] {
    const resolved = items
      .filter(item => item.licensingModel === LicensingModels.Floating && item.productType === ProductType.LicensedBundle)
      .map(item => item.identifier);
        return resolved;
      }
  private resolveAppIdentifiers(items: ProductOutput[]): string[] {
    const resolved = items
      .filter(item => item.licensingModel === LicensingModels.Floating && item.productType === ProductType.LicensedApp)
      .map(item => item.identifier);
    return resolved;
  }
  ngOnDestroy(): void {
    this.subscriptions.clear();
  }
}
