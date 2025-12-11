import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NumberOfUsedLicensesCellComponent } from './number-of-used-licenses-cell.component';

describe('NumberOfLicensesCellComponent', () => {
  let component: NumberOfUsedLicensesCellComponent;
  let fixture: ComponentFixture<NumberOfUsedLicensesCellComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NumberOfUsedLicensesCellComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NumberOfUsedLicensesCellComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
