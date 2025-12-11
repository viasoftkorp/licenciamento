import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicensingsDashboardComponent } from './licensings-dashboard.component';

describe('LicensingsDashboardComponent', () => {
  let component: LicensingsDashboardComponent;
  let fixture: ComponentFixture<LicensingsDashboardComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensingsDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingsDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
