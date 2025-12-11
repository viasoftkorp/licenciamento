import { TestBed } from '@angular/core/testing';

import { AccountsCnpjValidationService } from './accounts-cnpj-validation.service';

describe('AccountsCnpjValidationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AccountsCnpjValidationService = TestBed.get(AccountsCnpjValidationService);
    expect(service).toBeTruthy();
  });
});
