import { TestBed } from '@angular/core/testing';

import { LicensingsOnlineUsersGadgetService } from './licensings-online-users-gadget.service';

describe('LicensingsOnlineUsersGadgetService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LicensingsOnlineUsersGadgetService = TestBed.get(LicensingsOnlineUsersGadgetService);
    expect(service).toBeTruthy();
  });
});
