import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LicensingsEnvironmentComponent } from './licensings-environment.component';

describe('LicensingsEnvironmentComponent', () => {
  let component: LicensingsEnvironmentComponent;
  let fixture: ComponentFixture<LicensingsEnvironmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LicensingsEnvironmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingsEnvironmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
