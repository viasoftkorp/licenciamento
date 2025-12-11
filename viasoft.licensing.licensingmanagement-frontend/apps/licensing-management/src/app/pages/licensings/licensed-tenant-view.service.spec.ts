import { TestBed } from '@angular/core/testing';

import { LicensedTenantViewService } from './licensed-tenant-view.service';

describe('LicensedTenantViewService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LicensedTenantViewService = TestBed.get(LicensedTenantViewService);
    expect(service).toBeTruthy();
  });
});
