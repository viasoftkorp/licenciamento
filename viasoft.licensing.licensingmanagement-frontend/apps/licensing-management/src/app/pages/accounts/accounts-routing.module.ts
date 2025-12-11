import { Routes, RouterModule } from '@angular/router';
import { AccountsComponent } from './accounts.component';
import { NgModule } from '@angular/core';

const routes: Routes = [
    {
        path: '', component: AccountsComponent
    },
    { path: '**', redirectTo: '' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AccountsRoutingModule { }
