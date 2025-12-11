import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BatchOperationsLoadingComponent } from './batch-operations-loading.component';

describe('BatchOperationsLoadingComponent', () => {
  let component: BatchOperationsLoadingComponent;
  let fixture: ComponentFixture<BatchOperationsLoadingComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ BatchOperationsLoadingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BatchOperationsLoadingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
