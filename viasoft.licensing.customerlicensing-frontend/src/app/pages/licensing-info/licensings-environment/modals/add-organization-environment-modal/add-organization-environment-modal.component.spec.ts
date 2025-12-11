import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AddOrganizationEnvironmentModalComponent } from './add-organization-environment-modal.component';

describe('AddOrganizationEnvironmentModalComponent', () => {
  let component: AddOrganizationEnvironmentModalComponent;
  let fixture: ComponentFixture<AddOrganizationEnvironmentModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AddOrganizationEnvironmentModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddOrganizationEnvironmentModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
