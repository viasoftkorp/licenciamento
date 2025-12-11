import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicenseGridComponent } from './license-grid.component';

describe('LicenseGridComponent', () => {
  let component: LicenseGridComponent;
  let fixture: ComponentFixture<LicenseGridComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
