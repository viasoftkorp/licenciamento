import { TestBed } from '@angular/core/testing';

import { UsersGridService } from './users-grid.service';

describe('UsersGridService', () => {
  let service: UsersGridService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UsersGridService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
