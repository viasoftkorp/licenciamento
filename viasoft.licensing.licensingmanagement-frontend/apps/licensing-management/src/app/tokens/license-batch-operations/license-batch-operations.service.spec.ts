import { TestBed } from '@angular/core/testing';

import { LicenseBatchOperationsService } from './license-batch-operations.service';

describe('LicenseBatchOperationsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LicenseBatchOperationsService = TestBed.get(LicenseBatchOperationsService);
    expect(service).toBeTruthy();
  });
});
