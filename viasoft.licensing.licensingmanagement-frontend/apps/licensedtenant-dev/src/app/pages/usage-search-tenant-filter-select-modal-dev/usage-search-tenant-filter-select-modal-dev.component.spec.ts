import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { UsageSearchTenantFilterSelectModalDevComponent } from './usage-search-tenant-filter-select-modal-dev.component';

describe('UsageSearchTenantFilterSelectModalDevComponent', () => {
  let component: UsageSearchTenantFilterSelectModalDevComponent;
  let fixture: ComponentFixture<UsageSearchTenantFilterSelectModalDevComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ UsageSearchTenantFilterSelectModalDevComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsageSearchTenantFilterSelectModalDevComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
