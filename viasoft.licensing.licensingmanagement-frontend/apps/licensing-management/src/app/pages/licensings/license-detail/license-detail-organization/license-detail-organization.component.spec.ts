import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicenseDetailOrganizationComponent } from './license-detail-organization.component';

describe('LicenseDetailOrganizationComponent', () => {
  let component: LicenseDetailOrganizationComponent;
  let fixture: ComponentFixture<LicenseDetailOrganizationComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseDetailOrganizationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseDetailOrganizationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
