import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicensingsOnlineUsersGadgetComponent } from './licensings-online-users-gadget.component';

describe('LicensingsOnlineUsersGadgetComponent', () => {
  let component: LicensingsOnlineUsersGadgetComponent;
  let fixture: ComponentFixture<LicensingsOnlineUsersGadgetComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensingsOnlineUsersGadgetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingsOnlineUsersGadgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
