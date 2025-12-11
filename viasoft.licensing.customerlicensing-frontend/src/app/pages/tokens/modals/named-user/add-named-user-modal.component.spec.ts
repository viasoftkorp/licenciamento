import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddNamedUserModalComponent } from './add-named-user-modal.component';

describe('AddNamedUserModalComponent', () => {
  let component: AddNamedUserModalComponent;
  let fixture: ComponentFixture<AddNamedUserModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddNamedUserModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddNamedUserModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
