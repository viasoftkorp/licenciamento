import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicensingsGridComponent } from './licensings-grid.component';

describe('LicensingsGridComponent', () => {
  let component: LicensingsGridComponent;
  let fixture: ComponentFixture<LicensingsGridComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensingsGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingsGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
