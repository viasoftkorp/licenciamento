import { TestBed } from '@angular/core/testing';

import { LicensingsTreeTableService } from './licensings-tree-table.service';

describe('LicensingsTreeTableService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LicensingsTreeTableService = TestBed.get(LicensingsTreeTableService);
    expect(service).toBeTruthy();
  });
});
