import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfrastructureSettingsComponent } from './infrastructure-settings.component';

describe('InfrastructureSettingsComponent', () => {
  let component: InfrastructureSettingsComponent;
  let fixture: ComponentFixture<InfrastructureSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InfrastructureSettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InfrastructureSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
