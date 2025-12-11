import { TestBed } from '@angular/core/testing';

import { AuditingService } from './auditing.service';

describe('AuditingService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AuditingService = TestBed.get(AuditingService);
    expect(service).toBeTruthy();
  });
});
