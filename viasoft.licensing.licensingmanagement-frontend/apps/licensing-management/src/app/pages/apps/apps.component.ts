import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { AppsFormControlService } from './apps-form-control.service';
import { VsDialog } from '@viasoft/components';
import { AppDetailComponent } from './app-detail/app-detail.component';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';

@Component({
    selector: 'app-apps',
    templateUrl: './apps.component.html',
    styleUrls: ['./apps.component.scss']
})
export class AppsComponent implements OnInit, OnDestroy {

    public readonly policies = Policies;

    subs: Array<Subscription> = [];
    saveIsEnable = false;

    constructor(private appsService: AppsFormControlService, private vsDialog: VsDialog) {

    }

    ngOnInit(): void { }

    ngOnDestroy(): void {
        this.subs.forEach(s => s.unsubscribe());
    }

    add() {
        this.appsService.openSoftwares();
        this.vsDialog.open(AppDetailComponent, null, { maxWidth: '20vw' });
    }
}
