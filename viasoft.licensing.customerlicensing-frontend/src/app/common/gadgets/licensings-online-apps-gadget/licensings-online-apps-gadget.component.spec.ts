import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicensingsOnlineAppsGadgetComponent } from './licensings-online-apps-gadget.component';

describe('LicensingsOnlineAppsGadgetComponent', () => {
  let component: LicensingsOnlineAppsGadgetComponent;
  let fixture: ComponentFixture<LicensingsOnlineAppsGadgetComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensingsOnlineAppsGadgetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingsOnlineAppsGadgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
