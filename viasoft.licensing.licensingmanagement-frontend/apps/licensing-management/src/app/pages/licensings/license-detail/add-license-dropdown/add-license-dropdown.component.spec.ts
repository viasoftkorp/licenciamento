import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddLicenseDropdownComponent } from './add-license-dropdown.component';

describe('AddLicenseDropdownComponent', () => {
  let component: AddLicenseDropdownComponent;
  let fixture: ComponentFixture<AddLicenseDropdownComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddLicenseDropdownComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddLicenseDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
