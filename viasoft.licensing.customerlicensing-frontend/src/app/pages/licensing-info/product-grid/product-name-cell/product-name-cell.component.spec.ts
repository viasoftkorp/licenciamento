import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductNameCellComponent } from './product-name-cell.component';

describe('ProductNameCellComponent', () => {
  let component: ProductNameCellComponent;
  let fixture: ComponentFixture<ProductNameCellComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProductNameCellComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductNameCellComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
