import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LicensedTenantSettingsInfoComponent } from './licensed-tenant-settings-info.component';

describe('LicensedTenantSettingsInfoComponent', () => {
  let component: LicensedTenantSettingsInfoComponent;
  let fixture: ComponentFixture<LicensedTenantSettingsInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LicensedTenantSettingsInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensedTenantSettingsInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
