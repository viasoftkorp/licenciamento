import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { SoftwaresFormControlService } from './softwares-form-control.service';
import { VsDialog } from '@viasoft/components';
import { SoftwareDetailComponent } from './software-detail/software-detail.component';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';

@Component({
  selector: 'app-softwares',
  templateUrl: './softwares.component.html',
  styleUrls: ['./softwares.component.scss']
})
export class SoftwaresComponent implements OnInit, OnDestroy {

  softwareForm: Subscription;
  saveIsEnable: boolean;
  subs: Array<Subscription> = [];
  public readonly policies = Policies;

  constructor(private softwaresService: SoftwaresFormControlService,
    private vsDialog: VsDialog) { }

  ngOnInit() {
    this.subs.push(this.softwaresService.softwareFormSubject.subscribe(
      () => {
        this.saveIsEnable = true;
      }
    ));
    this.subs.push(this.softwaresService.softwareInvalidSubject.subscribe(
      () => {
        this.saveIsEnable = false;
      }
    ));
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe);
  }

  add() {
    this.vsDialog.open(SoftwareDetailComponent, null, { maxWidth: '20vw' });
  }

  save() {
    this.softwaresService.saveSoftwares();
    this.softwaresService.softwareFormIsInvalid();
  }

}
