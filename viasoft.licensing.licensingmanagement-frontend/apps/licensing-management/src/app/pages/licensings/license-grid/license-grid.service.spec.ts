import { TestBed } from '@angular/core/testing';

import { LicensingsService } from '../licensings.service';

describe('LicenseGridService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LicensingsService = TestBed.get(LicensingsService);
    expect(service).toBeTruthy();
  });
});
