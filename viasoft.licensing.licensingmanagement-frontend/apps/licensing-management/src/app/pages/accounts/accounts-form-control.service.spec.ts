import { TestBed } from '@angular/core/testing';

import { AccountsFormControlService } from './accounts-form-control.service';

describe('AccountsFormControlService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AccountsFormControlService = TestBed.get(AccountsFormControlService);
    expect(service).toBeTruthy();
  });
});
