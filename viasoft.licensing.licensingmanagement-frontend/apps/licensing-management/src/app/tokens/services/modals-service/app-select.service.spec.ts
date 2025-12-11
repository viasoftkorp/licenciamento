import { TestBed } from '@angular/core/testing';

import { AppSelectService } from './app-select.service';

describe('AppSelectService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AppSelectService = TestBed.get(AppSelectService);
    expect(service).toBeTruthy();
  });
});
