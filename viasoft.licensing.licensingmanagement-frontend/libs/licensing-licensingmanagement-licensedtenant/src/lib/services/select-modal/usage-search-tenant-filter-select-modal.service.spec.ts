import { TestBed } from '@angular/core/testing';

import { UsageSearchTenantFilterSelectModalService } from './usage-search-tenant-filter-select-modal.service';

describe('UsageSearchTenantFilterSelectModalService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UsageSearchTenantFilterSelectModalService = TestBed.get(UsageSearchTenantFilterSelectModalService);
    expect(service).toBeTruthy();
  });
});
