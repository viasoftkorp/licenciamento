import { TestBed } from '@angular/core/testing';

import { AccountsZipCodeValidationService } from './accounts-zip-code-validation.service';

describe('AccountsZipCodeValidationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AccountsZipCodeValidationService = TestBed.get(AccountsZipCodeValidationService);
    expect(service).toBeTruthy();
  });
});
