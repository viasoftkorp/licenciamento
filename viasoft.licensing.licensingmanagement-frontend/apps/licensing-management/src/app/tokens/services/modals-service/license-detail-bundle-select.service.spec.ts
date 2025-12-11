import { TestBed } from '@angular/core/testing';

import { LicenseDetailBundleSelectService } from './license-detail-bundle-select.service';

describe('LicenseDetailBundleService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LicenseDetailBundleSelectService = TestBed.get(LicenseDetailBundleSelectService);
    expect(service).toBeTruthy();
  });
});
