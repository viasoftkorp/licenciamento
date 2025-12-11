import { TestBed } from '@angular/core/testing';

import { LicensingsService } from './licensings.service';

describe('LicensingsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LicensingsService = TestBed.get(LicensingsService);
    expect(service).toBeTruthy();
  });
});
