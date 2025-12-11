import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BundleGridComponent } from './product-grid.component';

describe('BundleGridComponent', () => {
  let component: BundleGridComponent;
  let fixture: ComponentFixture<BundleGridComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BundleGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BundleGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
