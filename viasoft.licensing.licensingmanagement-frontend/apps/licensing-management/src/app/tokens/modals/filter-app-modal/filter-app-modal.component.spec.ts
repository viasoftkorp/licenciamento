import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterAppModalComponent } from './filter-app-modal.component';

describe('FilterAppModalComponent', () => {
  let component: FilterAppModalComponent;
  let fixture: ComponentFixture<FilterAppModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FilterAppModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FilterAppModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
