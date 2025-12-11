import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SoftwareGridComponent } from './software-grid.component';

describe('SoftwareGridComponent', () => {
  let component: SoftwareGridComponent;
  let fixture: ComponentFixture<SoftwareGridComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SoftwareGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SoftwareGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
