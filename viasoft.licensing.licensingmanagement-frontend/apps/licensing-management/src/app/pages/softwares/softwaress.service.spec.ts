import { TestBed } from '@angular/core/testing';

import { SoftwaresFormControlService } from './softwares-form-control.service';

describe('ApplicationsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SoftwaresFormControlService = TestBed.get(SoftwaresFormControlService);
    expect(service).toBeTruthy();
  });
});
