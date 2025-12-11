import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BundleGridComponent } from './bundle-grid.component';

describe('BundlesComponent', () => {
  let component: BundleGridComponent;
  let fixture: ComponentFixture<BundleGridComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ BundleGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BundleGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
