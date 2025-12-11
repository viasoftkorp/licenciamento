import { Component, OnInit, OnDestroy, Optional, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LicenseDetailInfrastructureConfigurationService } from './license-detail-infrastructure-configuration.service';
import { InfrastructureConfigurationUpdateInput } from '@viasoft/licensing-management/clients/licensing-management/model/infrastructureConfigurationUpdateInput';
import { LicensingsService } from '../../licensings.service';
import { LicensingFormCurrentTab } from '@viasoft/licensing-management/app/tokens/enum/licensing-form-current-tab.enum';
import { Subscription } from 'rxjs';
import { LicenseDetailComponent } from '../license-detail.component';
import { MessageService } from '@viasoft/common';

@Component({
  selector: 'app-license-detail-infrastructure-configuration',
  templateUrl: './license-detail-infrastructure-configuration.component.html',
  styleUrls: ['./license-detail-infrastructure-configuration.component.scss']
})
export class LicenseDetailInfrastructureConfigurationComponent implements OnInit, OnDestroy {

  infrastructureConfigurationForm: FormGroup;
  infrastructureConfiguration: InfrastructureConfigurationUpdateInput;
  private subs: Subscription[] = [];

  constructor(private readonly formBuilder: FormBuilder,
              private readonly licenseDetailInfrastructureConfigurationService: LicenseDetailInfrastructureConfigurationService,
              private readonly licensingsService: LicensingsService,
              @Optional() @Inject(LicenseDetailComponent) private licenseDetail: LicenseDetailComponent,
              private readonly notification: MessageService) { }

  ngOnInit(): void {
    if (this.licenseDetail.identifierReady) {
      this.init();
    } else {
      this.licenseDetailInfrastructureConfigurationService.licensingIdentifierReadySubject.subscribe(
        () => {
          this.init();
        }
      );
    }
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  init() {
    this.subs.push(this.licenseDetailInfrastructureConfigurationService.getByTenantId(this.licenseDetail.licenseIdentifier).subscribe(
      (v) => {
        if (v !== null) {
          this.infrastructureConfiguration = {
            desktopDatabaseName: v.desktopDatabaseName,
            gatewayAddress: v.gatewayAddress
          };
          this.licenseDetail.infrastructureInitialValues.desktopDatabaseName = v.desktopDatabaseName;
          this.licenseDetail.infrastructureInitialValues.gatewayAddress = v.gatewayAddress;
          this.createForm();
          this.licensingsService.setCurrentTab(LicensingFormCurrentTab.InfrastructureConfiguration);
          this.getInfrastructureConfigurationFromLicensedTenant();
        }
      },
      () => {
        this.notification.error('common.error.CouldntGetInfrastructureConfiguration');
      }
    ));
  }

  createForm() {
    const gatewayPattern = /((([0-9]{1,3}\.){3}[0-9])|([-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,12}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?))(\:\d{2,4}|)/;
    this.infrastructureConfigurationForm = this.formBuilder.group({
      gateway: this.formBuilder.control(this.infrastructureConfiguration.gatewayAddress,
        [Validators.pattern(gatewayPattern)]),
      desktopDatabaseName: this.formBuilder.control(this.infrastructureConfiguration.desktopDatabaseName)
    });

    //Emit initial values
    this.licenseDetailInfrastructureConfigurationService.setInfrastructureConfigurationValues(this.infrastructureConfiguration);

    this.subs.push(this.infrastructureConfigurationForm.valueChanges.subscribe((v) => {
      if (v !== null) {
        this.infrastructureConfiguration.gatewayAddress = v.gateway;
        this.infrastructureConfiguration.desktopDatabaseName = v.desktopDatabaseName;
      }
      this.licenseDetailInfrastructureConfigurationService.setInfrastructureConfigurationValues(this.infrastructureConfiguration);
    }));

    this.subs.push(this.licenseDetailInfrastructureConfigurationService.updateGatewayAdressSubject.subscribe((v) => {
      this.infrastructureConfigurationForm.get('gateway').setValue(v);
    }))
  }

  getInfrastructureConfigurationFromLicensedTenant() {
    this.subs.push(this.licenseDetailInfrastructureConfigurationService.infrastructureConfigurationsFromLicensedTenant.subscribe(v => {
      if (v !== null) {
        this.infrastructureConfiguration.desktopDatabaseName = v.desktopDatabaseName;
        this.infrastructureConfiguration.gatewayAddress = v.gatewayAddress;
        this.infrastructureConfigurationForm.get('gateway').setValue(v.gatewayAddress);
        this.infrastructureConfigurationForm.get('desktopDatabaseName').setValue(v.desktopDatabaseName);
      }
    }));
  }
}
