import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LicensingsComponent } from './licensings.component';
import { LicensingsGridComponent } from './licensings-grid/licensings-grid.component';
import { LicensingsDashboardComponent } from './licensings-dashboard/licensings-dashboard.component';

const routes: Routes = [
  { path: '', component: LicensingsComponent, children: [
      {path: '', component: LicensingsGridComponent},
      {path: 'dashboard', component: LicensingsDashboardComponent},
      {path: '**' , redirectTo: ''}
  ]},
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LicensingsRouting { }
