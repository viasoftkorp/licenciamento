import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MustBeLoggedAuthGuard } from '@viasoft/common';
import { authorizationRoutes } from '@viasoft/authorization-management';

const routes: Routes = [
  ...authorizationRoutes,
  {
    path: '',
    loadChildren: () => import('./pages/pages.module').then(m => m.PagesModule),
    canActivate: [MustBeLoggedAuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {})],
  exports: [RouterModule]
})
export class AppRoutingModule {}
