import { TestBed } from '@angular/core/testing';

import { SoftwareSelectService } from './software-select.service';

describe('SoftwareSelectService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SoftwareSelectService = TestBed.get(SoftwareSelectService);
    expect(service).toBeTruthy();
  });
});
