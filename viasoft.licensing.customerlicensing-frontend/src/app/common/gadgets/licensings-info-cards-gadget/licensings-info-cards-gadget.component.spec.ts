import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicensingsInfoCardsGadgetComponent } from './licensings-info-cards-gadget.component';

describe('LicensingsInfoCardsGadgetComponent', () => {
  let component: LicensingsInfoCardsGadgetComponent;
  let fixture: ComponentFixture<LicensingsInfoCardsGadgetComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensingsInfoCardsGadgetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingsInfoCardsGadgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
