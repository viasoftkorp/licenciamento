import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicenseDetailFileQuotaComponent } from './license-detail-file-quota.component';

describe('LicenseDetailFileQuotaComponent', () => {
  let component: LicenseDetailFileQuotaComponent;
  let fixture: ComponentFixture<LicenseDetailFileQuotaComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseDetailFileQuotaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseDetailFileQuotaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
