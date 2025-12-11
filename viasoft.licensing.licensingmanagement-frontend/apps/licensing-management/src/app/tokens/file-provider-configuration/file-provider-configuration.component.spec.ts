import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FileProviderConfigurationComponent } from './file-provider-configuration.component';

describe('FileProviderConfigurationComponent', () => {
  let component: FileProviderConfigurationComponent;
  let fixture: ComponentFixture<FileProviderConfigurationComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FileProviderConfigurationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FileProviderConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
