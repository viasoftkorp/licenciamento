import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BundleGridBatchOperationsComponent } from './bundle-grid.component';

describe('BundleGridComponent', () => {
  let component: BundleGridBatchOperationsComponent;
  let fixture: ComponentFixture<BundleGridBatchOperationsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ BundleGridBatchOperationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BundleGridBatchOperationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
