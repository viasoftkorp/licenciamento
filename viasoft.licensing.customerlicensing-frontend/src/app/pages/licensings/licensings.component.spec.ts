import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicensingsComponent } from './licensings.component';

describe('LicensingsComponent', () => {
  let component: LicensingsComponent;
  let fixture: ComponentFixture<LicensingsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
