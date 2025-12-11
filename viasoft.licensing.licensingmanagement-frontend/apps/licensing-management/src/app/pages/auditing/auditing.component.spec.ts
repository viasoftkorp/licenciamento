import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AuditingComponent } from './auditing.component';

describe('AuditingComponent', () => {
  let component: AuditingComponent;
  let fixture: ComponentFixture<AuditingComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AuditingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AuditingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
