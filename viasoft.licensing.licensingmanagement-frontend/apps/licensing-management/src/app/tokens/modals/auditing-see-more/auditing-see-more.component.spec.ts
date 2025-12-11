import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuditingSeeMoreComponent } from './auditing-see-more.component';

describe('AuditingSeeMoreComponent', () => {
  let component: AuditingSeeMoreComponent;
  let fixture: ComponentFixture<AuditingSeeMoreComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuditingSeeMoreComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuditingSeeMoreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
