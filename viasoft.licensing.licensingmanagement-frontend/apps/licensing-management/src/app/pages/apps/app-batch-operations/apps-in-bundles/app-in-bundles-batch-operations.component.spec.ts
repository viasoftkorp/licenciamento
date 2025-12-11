import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AppInBundlesBatchOperationsComponent } from './app-in-bundles-batch-operations.component';

describe('AppInBundlesBatchOperationsComponent', () => {
  let component: AppInBundlesBatchOperationsComponent;
  let fixture: ComponentFixture<AppInBundlesBatchOperationsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AppInBundlesBatchOperationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppInBundlesBatchOperationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
