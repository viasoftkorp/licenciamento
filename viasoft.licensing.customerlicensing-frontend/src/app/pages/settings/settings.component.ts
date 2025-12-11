import { Component, OnInit } from '@angular/core';
import {SharedService} from "./shared.service";
import {VsSubscriptionManager} from "@viasoft/common";


@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  public isButtonDisabled: boolean = true;

  private subs: VsSubscriptionManager = new VsSubscriptionManager();


  constructor(
    private sharedService: SharedService
  ) { }

  ngOnInit(): void {
   this.subs.add('see if button is disabled',
     this.sharedService.seeIfDisabled.subscribe(data => {
       this.isButtonDisabled = data;
     }))
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  updateGateway() {
    this.sharedService.updateGateway();
    this.isButtonDisabled = true;
  }

}
