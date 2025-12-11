import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicenseDetailComponent } from './license-detail.component';

describe('LicenseDetailComponent', () => {
  let component: LicenseDetailComponent;
  let fixture: ComponentFixture<LicenseDetailComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
