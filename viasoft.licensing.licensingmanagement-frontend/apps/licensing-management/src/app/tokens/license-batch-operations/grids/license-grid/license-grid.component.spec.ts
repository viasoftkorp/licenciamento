import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicenseGridBatchOperationsComponent } from './license-grid.component';

describe('LicenseGridComponent', () => {
  let component: LicenseGridBatchOperationsComponent;
  let fixture: ComponentFixture<LicenseGridBatchOperationsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseGridBatchOperationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseGridBatchOperationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
