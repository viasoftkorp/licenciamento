import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SoftwareGridComponent } from './software-grid/software-grid.component';
import { SoftwareDetailComponent } from './software-detail/software-detail.component';
import { SoftwaresComponent } from './softwares.component';

const routes: Routes = [
  {
    path: '', component: SoftwaresComponent, children: [
      { path: 'new', component: SoftwareDetailComponent },
      { path: ':id', component: SoftwareDetailComponent },
      { path: '', component: SoftwareGridComponent },
      { path: '**', redirectTo: '' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SoftwaresRoutingModule { }
