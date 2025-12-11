import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LicensingsTreeTableComponent } from './licensings-tree-table.component';

describe('LicensingsTreeTableComponent', () => {
  let component: LicensingsTreeTableComponent;
  let fixture: ComponentFixture<LicensingsTreeTableComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LicensingsTreeTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicensingsTreeTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
