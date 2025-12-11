import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FileQuotaLicensedAppSelectComponent } from './file-quota-licensed-app-select.component';

describe('FileQuotaLicensedAppSelectComponent', () => {
  let component: FileQuotaLicensedAppSelectComponent;
  let fixture: ComponentFixture<FileQuotaLicensedAppSelectComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FileQuotaLicensedAppSelectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FileQuotaLicensedAppSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
