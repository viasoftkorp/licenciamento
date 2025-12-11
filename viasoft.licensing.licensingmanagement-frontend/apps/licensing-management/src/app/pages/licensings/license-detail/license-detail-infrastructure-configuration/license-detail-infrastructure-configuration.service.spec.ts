import { TestBed } from '@angular/core/testing';

import { LicenseDetailInfrastructureConfigurationService } from './license-detail-infrastructure-configuration.service';

describe('LicenseDetailInfrastructureConfigurationService', () => {
  let service: LicenseDetailInfrastructureConfigurationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LicenseDetailInfrastructureConfigurationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
