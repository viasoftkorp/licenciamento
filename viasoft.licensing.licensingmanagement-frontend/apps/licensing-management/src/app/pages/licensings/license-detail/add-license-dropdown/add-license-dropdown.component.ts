import { Component, EventEmitter, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { ProductType } from '@viasoft/licensing-management/app/tokens/enum/ProductType';

@Component({
  selector: 'app-add-license-dropdown',
  templateUrl: './add-license-dropdown.component.html',
  styleUrls: ['./add-license-dropdown.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddLicenseDropdownComponent implements OnInit {

  @Output() public onClick = new EventEmitter<ProductType>();

  ngOnInit(): void {
  }

  public clickProduct(): void {
    this.onClick.emit(ProductType.LicensedBundle);
  }

  public clickApp(): void {
    this.onClick.emit(ProductType.LicensedApp);
  }

}
