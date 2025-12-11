import { Component, OnDestroy, OnInit } from '@angular/core';
import { VsSubscriptionManager } from '@viasoft/common';
import { VsTableCell } from '@viasoft/components/shared';
import { LicensingModels } from 'src/client/customer-licensing/model/LicensingModels';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-number-of-used-licenses-cell',
  templateUrl: './number-of-used-licenses-cell.component.html',
  styleUrls: ['./number-of-used-licenses-cell.component.scss']
})
export class NumberOfUsedLicensesCellComponent implements VsTableCell, OnInit, OnDestroy {
  data: any;
  numberOfUsedLicenses: number;
  field?: string;
  disabled?: boolean;
  subscriptions = new VsSubscriptionManager();

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    if(this.data.licensingModel === LicensingModels.Named) {
      this.numberOfUsedLicenses = this.data.numberOfUsedLicenses
    }else{
      this.subscriptions.add('licenseUsage', this.productService.licenseUsage.subscribe((value) => 
        this.numberOfUsedLicenses = value?.find(b => b.productIdentifier === this.data.identifier)?.appLicensesConsumed
      ))
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.clear();
  }

}
