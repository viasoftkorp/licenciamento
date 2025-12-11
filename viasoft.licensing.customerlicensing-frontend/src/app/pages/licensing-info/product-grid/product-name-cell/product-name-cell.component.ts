import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { VsTableCell } from '@viasoft/components';
import { ProductOutput } from 'src/client/customer-licensing/model/ProductOutput';

@Component({
  selector: 'app-product-name-cell',
  templateUrl: './product-name-cell.component.html',
  styleUrls: ['./product-name-cell.component.scss']
})
export class ProductNameCellComponent implements VsTableCell {
  
  @Input() public data: ProductOutput;

  constructor(private router: Router) { }

  private navigateToUsers(): void {
    this.router.navigate(['/licensing-info/product', this.data.productType, this.data.id]);
  }

  public onClick(event: MouseEvent): void {
    event.stopPropagation();
    this.navigateToUsers()
  }
}
