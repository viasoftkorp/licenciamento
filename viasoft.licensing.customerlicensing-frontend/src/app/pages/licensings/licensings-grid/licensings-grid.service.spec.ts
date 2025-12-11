import { TestBed } from '@angular/core/testing';

import { LicensingsGridService } from './licensings-grid.service';

describe('LicensingsGridService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LicensingsGridService = TestBed.get(LicensingsGridService);
    expect(service).toBeTruthy();
  });
});
