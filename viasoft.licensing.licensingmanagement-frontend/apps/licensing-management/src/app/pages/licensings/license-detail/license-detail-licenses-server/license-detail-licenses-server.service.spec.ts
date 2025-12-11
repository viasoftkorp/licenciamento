import { TestBed } from '@angular/core/testing';

import { LicenseDetailLicensesServerService } from './license-detail-licenses-server.service';

describe('LicenseDetailLicensesServerService', () => {
  let service: LicenseDetailLicensesServerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LicenseDetailLicensesServerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
