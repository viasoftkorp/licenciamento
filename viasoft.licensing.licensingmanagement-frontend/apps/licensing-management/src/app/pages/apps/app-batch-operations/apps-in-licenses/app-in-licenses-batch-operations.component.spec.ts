import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AppInLicensesBatchOperationsComponent } from './app-in-licenses-batch-operations.component';

describe('AppInLicensesBatchOperationsComponent', () => {
  let component: AppInLicensesBatchOperationsComponent;
  let fixture: ComponentFixture<AppInLicensesBatchOperationsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AppInLicensesBatchOperationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppInLicensesBatchOperationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
