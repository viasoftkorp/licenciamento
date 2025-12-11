import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SoftwaresComponent } from './softwares.component';

describe('SoftwaresComponent', () => {
  let component: SoftwaresComponent;
  let fixture: ComponentFixture<SoftwaresComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SoftwaresComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SoftwaresComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
