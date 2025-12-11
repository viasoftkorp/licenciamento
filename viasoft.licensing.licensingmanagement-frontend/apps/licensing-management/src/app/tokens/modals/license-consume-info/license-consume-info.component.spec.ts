import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicenseConsumeInfoComponent } from './license-consume-info.component';

describe('LicenseConsumeInfoComponent', () => {
  let component: LicenseConsumeInfoComponent;
  let fixture: ComponentFixture<LicenseConsumeInfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseConsumeInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseConsumeInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
