import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LicensingInfoComponent } from './licensing-info.component';

describe('LicensingInfoComponent', () => {
  let component: LicensingInfoComponent;
  let fixture: ComponentFixture<LicensingInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensingInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
