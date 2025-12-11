import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LooseAppsNumberSelectComponent } from './loose-apps-number-select.component';

describe('LooseAppsNumberSelectComponent', () => {
  let component: LooseAppsNumberSelectComponent;
  let fixture: ComponentFixture<LooseAppsNumberSelectComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LooseAppsNumberSelectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LooseAppsNumberSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
