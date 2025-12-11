import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicenseDetailInfrastructureConfigurationComponent } from './license-detail-infrastructure-configuration.component';

describe('LicenseDetailInfrastructureConfigurationComponent', () => {
  let component: LicenseDetailInfrastructureConfigurationComponent;
  let fixture: ComponentFixture<LicenseDetailInfrastructureConfigurationComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseDetailInfrastructureConfigurationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseDetailInfrastructureConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
