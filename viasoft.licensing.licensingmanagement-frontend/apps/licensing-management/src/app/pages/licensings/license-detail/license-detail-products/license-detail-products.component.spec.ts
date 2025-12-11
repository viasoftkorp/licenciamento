import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LicenseDetailProductsComponent } from './license-detail-products.component';

describe('LicenseDetailProductsComponent', () => {
  let component: LicenseDetailProductsComponent;
  let fixture: ComponentFixture<LicenseDetailProductsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LicenseDetailProductsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseDetailProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
