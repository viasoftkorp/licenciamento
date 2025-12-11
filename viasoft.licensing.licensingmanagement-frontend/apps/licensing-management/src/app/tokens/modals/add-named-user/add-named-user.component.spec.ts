import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AddNamedUserComponent } from './add-named-user.component';

describe('AddNamedUserComponent', () => {
  let component: AddNamedUserComponent;
  let fixture: ComponentFixture<AddNamedUserComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AddNamedUserComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddNamedUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
