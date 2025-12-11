import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AppGridBatchOperationsComponent } from './app-grid.component';

describe('AppGridComponent', () => {
  let component: AppGridBatchOperationsComponent;
  let fixture: ComponentFixture<AppGridBatchOperationsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AppGridBatchOperationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppGridBatchOperationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
