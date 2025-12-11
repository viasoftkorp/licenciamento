import { TestBed } from '@angular/core/testing';

import { AccountSelectService } from './account-select.service';

describe('AccountSelectService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AccountSelectService = TestBed.get(AccountSelectService);
    expect(service).toBeTruthy();
  });
});
