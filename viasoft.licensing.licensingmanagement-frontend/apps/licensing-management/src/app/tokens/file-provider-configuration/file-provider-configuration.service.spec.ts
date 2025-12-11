import { TestBed } from '@angular/core/testing';

import { FileProviderConfigurationService } from './file-provider-configuration.service';

describe('FileProviderConfigurationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: FileProviderConfigurationService = TestBed.get(FileProviderConfigurationService);
    expect(service).toBeTruthy();
  });
});
