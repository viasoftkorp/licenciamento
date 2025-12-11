import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuditingComponent } from './auditing.component';

const routes: Routes = [
  { path: '' , component: AuditingComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuditingRoutingModule { }
