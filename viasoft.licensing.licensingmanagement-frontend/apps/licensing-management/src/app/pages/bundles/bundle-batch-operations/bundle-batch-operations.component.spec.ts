import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BundleBatchOperationsComponent } from './bundle-batch-operations.component';

describe('BundleBatchOperationsComponent', () => {
  let component: BundleBatchOperationsComponent;
  let fixture: ComponentFixture<BundleBatchOperationsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ BundleBatchOperationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BundleBatchOperationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
