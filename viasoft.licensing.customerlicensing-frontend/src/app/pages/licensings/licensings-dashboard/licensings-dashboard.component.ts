import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { VsDashboardComponent } from '@viasoft/dashboard';
import { Router } from '@angular/router';

@Component({
  selector: 'app-licensings-dashboard',
  templateUrl: './licensings-dashboard.component.html',
  styleUrls: ['./licensings-dashboard.component.scss'],
})
export class LicensingsDashboardComponent implements AfterViewInit {
  @ViewChild('dashboard') private dashboard: VsDashboardComponent;
  private fragment: string;

  constructor(private router: Router) {}

  ngAfterViewInit(): void {
    if (this.fragment === 'config') {
      this.dashboard.openConfigurations();
    } else if (this.fragment === 'gadget') {
      this.dashboard.openAddGadget();
    } else if (this.fragment === 'layout') {
      this.dashboard.editLayout(null);
    }
  }
}
