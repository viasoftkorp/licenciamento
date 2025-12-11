import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import {SettingsComponent} from "./settings.component";
import {InfrastructureSettingsComponent} from "./infrastructure-settings/infrastructure-settings.component";

const routes: Routes  = [
  {
    path: '',
    component: SettingsComponent,
    children: [
      {path:'', component: InfrastructureSettingsComponent},
      {path: '**' , redirectTo: ''}
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
]
@NgModule({
  declarations: [],
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SettingsRoutingModule { }
