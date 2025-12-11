import {Component, Input, OnDestroy, OnInit, Output} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {forkJoin} from "rxjs";

import {VsAuthorizationService, VsSubscriptionManager} from "@viasoft/common";
import {MessageService} from "@viasoft/common";

import {InfrastructureSettingsService} from "./infrastructure-settings.service";
import {SharedService} from "../shared.service";
import {TenantInfoOutput} from "../../../../client/customer-licensing";
import {InfrastructureSettings} from "./infrastructure-settings.interface";
import {VsSelectOption} from "@viasoft/components";
import {Policies} from "../../../authorizations/policies.class";

@Component({
    selector: 'app-infrastructure-settings',
    templateUrl: './infrastructure-settings.component.html',
    styleUrls: ['./infrastructure-settings.component.scss']
  })

  export class InfrastructureSettingsComponent implements OnInit, OnDestroy {

    @Input() gatewayAddress: string;

    @Output() infrastructureSettingsForm: FormGroup;
    public versionsOfDropdown: VsSelectOption[] =[];
    public selectedVersion: string = null;

    private infrastructureSettings: InfrastructureSettings;
    private subs: VsSubscriptionManager = new VsSubscriptionManager();
    private licensedTenantId: string;
    private canUpdate: boolean;

    constructor(
      private infrastructureSettingsService: InfrastructureSettingsService,
      private authorizationService: VsAuthorizationService,
      private sharedService: SharedService,
      private notification: MessageService

    ) {}

    ngOnInit(): void {
      this.getGatewayAndVersions();
      this.getPermissions();
     this.subs.add('gateway-Update',
       this.sharedService.putGateway.subscribe(() => this.updateGateway())
     );
    }

    public getPermissions(): void {
      this.authorizationService.isGranted([Policies.UpdateSettings]).then(res => this.canUpdate = res)
    }

    ngOnDestroy(): void {
      this.subs.clear();
    }

   private getGatewayAndVersions(): void {
      this.subs.add('get-licensedTenantId-and-initial-values-of-infrastructureSettings',
        forkJoin([this.infrastructureSettingsService.getLicensedTenantId(), this.infrastructureSettingsService.getGatewayAndVersions()])
          .subscribe((result: [TenantInfoOutput, InfrastructureSettings] )=> {

            this.licensedTenantId = result[0].licensedTenantId;
            const gatewayAndVersions = result[1];
            this.infrastructureSettings = {
              gatewayAddress: gatewayAndVersions.gatewayAddress,
              deployVersions: gatewayAndVersions.deployVersions
            }

            this.versionsOfDropdown = this.infrastructureSettingsService.getVersionsOfDropdown(this.infrastructureSettings.deployVersions)
            this.initForm(this.infrastructureSettings);
            this.changingGateway()
          },
            () => {
              this.notification.error('settings.infrastructureSettings.error.CouldntGetInfrastructureConfiguration');
            }
          ))
    }

    private initForm(infrastructureSettings: InfrastructureSettings): void {
      const gatewayPattern = /((([0-9]{1,3}\.){3}[0-9])|([-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,12}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?))(\:\d{2,4}|)/;
      this.infrastructureSettingsForm = new FormGroup({
          gatewayAddress: new FormControl({value: infrastructureSettings?.gatewayAddress, disabled: !this.canUpdate}, [Validators.pattern(gatewayPattern)]),
      })
    }

    private updateGateway(): void {
      const currentGatewayAddress = this.infrastructureSettingsForm.get("gatewayAddress");
      this.subs.add('update-gateway',
        this.infrastructureSettingsService.updateGateway(this.licensedTenantId, currentGatewayAddress.value).subscribe(data => {
          this.licensedTenantId == data?.licensedTenantId;
          currentGatewayAddress.setValue(data.gatewayAddress);
        })
      )
    }

    private changingGateway(): void {
      let authorized: boolean;
      this.authorizationService.isGranted([Policies.UpdateSettings, Policies.ReadSettings], 'AND').then(res => authorized = res);
      this.subs.add('update gatewayAddress',
        this.infrastructureSettingsForm.get("gatewayAddress").valueChanges.subscribe(actualGateway => {
          if ( actualGateway !== undefined && authorized) {
            this.sharedService.hideSaveButton(false)
          }
        })
      )
    }

    public selectedVer(value: string): void {
      this.selectedVersion = value;
    }

    public copyDeployCommand():void {
        this.infrastructureSettingsService.copyToClipboardImplementation(this.selectedVersion);
    }

    public copyUpdateCommand():void {
      this.infrastructureSettingsService.copyToClipboardUpdate(this.selectedVersion);
    }

    public copyUninstallCommand():void {
      this.infrastructureSettingsService.copyToClipboardUninstall(this.selectedVersion);
    }
  }
