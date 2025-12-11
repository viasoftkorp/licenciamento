import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BundleDetailComponent } from './bundle-detail.component';

describe('BundleDetailComponent', () => {
  let component: BundleDetailComponent;
  let fixture: ComponentFixture<BundleDetailComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ BundleDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BundleDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
