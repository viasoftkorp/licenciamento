import { Component, Input, OnInit } from '@angular/core';
import { VsTableCell } from '@viasoft/components';
import { ProductStatus } from 'src/app/common/enums/ProductStatus';

@Component({
  selector: 'app-status-cell',
  templateUrl: './status-cell.component.html',
  styleUrls: ['./status-cell.component.scss']
})
export class StatusCellComponent implements VsTableCell, OnInit {

  @Input() data: any;
  status: string = '';
  icon: string = '';
  iconColor: string;

  constructor() { }

  ngOnInit() {
    if(ProductStatus[this.data.status]) {
      this.status = 'LicensingInfo.Products.Status.' + ProductStatus[this.data.status];
      if(this.data.status == ProductStatus.ProductBlocked) {
        this.icon = "times-circle";
      } else {
        this.icon = "check-circle";
        this.iconColor = "#2D9D78";
      }
    }
  }
}
