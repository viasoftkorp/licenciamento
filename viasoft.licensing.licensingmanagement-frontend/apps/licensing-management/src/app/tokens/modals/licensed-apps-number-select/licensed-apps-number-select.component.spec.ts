import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicensedAppsNumberSelectComponent } from './licensed-apps-number-select.component';

describe('LicensedAppsNumberSelectComponent', () => {
  let component: LicensedAppsNumberSelectComponent;
  let fixture: ComponentFixture<LicensedAppsNumberSelectComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensedAppsNumberSelectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensedAppsNumberSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
