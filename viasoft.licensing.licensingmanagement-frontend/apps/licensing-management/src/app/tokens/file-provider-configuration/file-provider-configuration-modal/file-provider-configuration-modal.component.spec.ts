import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FileProviderConfigurationModalComponent } from './file-provider-configuration-modal.component';

describe('FileProviderConfigurationModalComponent', () => {
  let component: FileProviderConfigurationModalComponent;
  let fixture: ComponentFixture<FileProviderConfigurationModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FileProviderConfigurationModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FileProviderConfigurationModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
