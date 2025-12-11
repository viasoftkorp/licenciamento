import { Component, OnDestroy, OnInit } from '@angular/core';

import { VsSubscriptionManager } from '@viasoft/common';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';
import { LicensingsFormControlServices } from './licensings-form-control.service';

@Component({
  selector: 'app-licensings',
  templateUrl: './licensings.component.html',
  styleUrls: ['./licensings.component.scss']
})
export class LicensingsComponent implements OnInit, OnDestroy {
  private subs = new VsSubscriptionManager();
  public saveIsEnable: boolean;
  public readonly policies = Policies;

  constructor(private licensingsFormControl: LicensingsFormControlServices) { }

  ngOnInit(): void {
    this.subs.add('form', this.licensingsFormControl.licensingsForm.subscribe((valid) => {
      this.saveIsEnable = valid;
    }));
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  public add(): void {
    this.licensingsFormControl.openSoftwares();
  }

  public save(): void {
    this.licensingsFormControl.saveLicensings();
    this.licensingsFormControl.licensingsFormIsInvalid();
  }
}
