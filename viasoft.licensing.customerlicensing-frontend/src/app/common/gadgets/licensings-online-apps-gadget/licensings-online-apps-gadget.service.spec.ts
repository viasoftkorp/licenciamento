import { TestBed } from '@angular/core/testing';

import { LicensingsOnlineAppsGadgetService } from './licensings-online-apps-gadget.service';

describe('LicensingsOnlineAppsGadgetService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LicensingsOnlineAppsGadgetService = TestBed.get(LicensingsOnlineAppsGadgetService);
    expect(service).toBeTruthy();
  });
});
