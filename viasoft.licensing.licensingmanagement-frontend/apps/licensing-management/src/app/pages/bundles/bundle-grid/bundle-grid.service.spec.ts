import { TestBed } from '@angular/core/testing';

import { BundlesService } from '../bundles.service';

describe('BundlesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: BundlesService = TestBed.get(BundlesService);
    expect(service).toBeTruthy();
  });
});
