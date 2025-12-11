import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LicensingEnvironmentOrganizationalComponent } from './licensing-environment-organizational.component';

describe('LicensingEnvironmentOrganizationalComponent', () => {
  let component: LicensingEnvironmentOrganizationalComponent;
  let fixture: ComponentFixture<LicensingEnvironmentOrganizationalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LicensingEnvironmentOrganizationalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingEnvironmentOrganizationalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
