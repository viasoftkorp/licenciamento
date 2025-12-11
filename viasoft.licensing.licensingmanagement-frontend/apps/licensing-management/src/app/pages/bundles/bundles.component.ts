import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { BundlesFormControlService } from './bundles-form-control.service';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';

@Component({
  selector: 'app-bundles',
  templateUrl: './bundles.component.html',
  styleUrls: ['./bundles.component.scss']
})
export class BundlesComponent implements OnInit, OnDestroy {

  subs: Array<Subscription> = [];
  saveIsEnable: boolean;
  public readonly policies = Policies;

  constructor(private bundlesService: BundlesFormControlService) { }

  ngOnInit(): void {
    this.subs.push(this.bundlesService.bundleFormSubject.subscribe(
      (valid) => {
        if (valid) {
          this.saveIsEnable = true;
        } else {
          this.saveIsEnable = false;
        }
      }
    ));
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  add() {
    this.bundlesService.openSoftwares();
  }

  save() {
    this.bundlesService.saveBundles();
    this.bundlesService.bundleFormIsInvalid();
  }
}
