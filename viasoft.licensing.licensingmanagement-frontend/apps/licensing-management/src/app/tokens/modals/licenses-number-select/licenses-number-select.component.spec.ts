import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicensesNumberSelectComponent } from './licenses-number-select.component';

describe('LicensesNumberSelectComponent', () => {
  let component: LicensesNumberSelectComponent;
  let fixture: ComponentFixture<LicensesNumberSelectComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensesNumberSelectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensesNumberSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
