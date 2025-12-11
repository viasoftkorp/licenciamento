import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AddOrganizationUnitModalComponent } from './add-organization-unit-modal.component';

describe('AddOrganizationUnitModalComponent', () => {
  let component: AddOrganizationUnitModalComponent;
  let fixture: ComponentFixture<AddOrganizationUnitModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AddOrganizationUnitModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddOrganizationUnitModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
