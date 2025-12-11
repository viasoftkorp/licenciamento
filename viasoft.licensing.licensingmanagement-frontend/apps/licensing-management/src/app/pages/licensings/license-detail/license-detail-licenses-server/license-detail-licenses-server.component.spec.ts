import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LicenseDetailLicensesServerComponent } from './license-detail-licenses-server.component';

describe('LicenseDetailLicensesServerComponent', () => {
  let component: LicenseDetailLicensesServerComponent;
  let fixture: ComponentFixture<LicenseDetailLicensesServerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LicenseDetailLicensesServerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseDetailLicensesServerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
