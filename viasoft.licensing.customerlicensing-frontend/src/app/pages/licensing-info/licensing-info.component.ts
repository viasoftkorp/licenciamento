import { Component, OnDestroy, OnInit } from '@angular/core';
import { VsSubscriptionManager } from '@viasoft/common';
import { VsMaskResolverService } from '@viasoft/components';
import { LicensingStatus } from 'src/app/common/enums/licensing-status.enum';
import { OperationValidation } from 'src/client/customer-licensing';
import { LicensingsService } from '../licensings/licensings.service';
import { VsJwtProviderService } from '@viasoft/http';

@Component({
  selector: 'app-licensing-info',
  templateUrl: './licensing-info.component.html',
  styleUrls: ['./licensing-info.component.scss'],
})
export class LicensingInfoComponent implements OnInit, OnDestroy {

  subscriptions: VsSubscriptionManager = new VsSubscriptionManager();
  licensedTenantId: string;
  cnpjMask: any;
  licensing = {
    licensingIdentifier: '',
    cnpjs: [],
    status: {
      icon: '',
      iconColor: '',
      label: ''
    }
  };
  isLoad: boolean;

  constructor(
    private licensingsService: LicensingsService,
    private jwt: VsJwtProviderService,
    ) {
      this.cnpjMask = VsMaskResolverService.getMask('cnpj-cpf');
    }

  ngOnInit(): void {
    const licensingIdentifier = this.jwt.getTenantIdFromJwt(this.jwt.getStoredJwt());
    this.licensingsService.setTenantId(licensingIdentifier);
    this.getLicensingInformation(licensingIdentifier);
  }

  private getLicensingInformation(licensingIdentifier: string) {
    this.subscriptions.add('tenantInfo', this.licensingsService.getTenantInfoFromId(licensingIdentifier).subscribe(
      (value) => {
        if (value.operationValidationDescription === OperationValidation.NoError) {
          this.licensedTenantId = value.licensedTenantId;
          this.licensingsService.setLicensedTenantId(value.licensedTenantId);
          this.setLicensingStatus(value.licensingStatus);
          this.licensing.cnpjs = value.cnpj.split(',');
        }
        this.licensing.licensingIdentifier = licensingIdentifier;
        this.isLoad = true;
      }
    ))
  }

  setLicensingStatus(licensingStatus: any) {
    if(licensingStatus == LicensingStatus.Active || licensingStatus == LicensingStatus.NeedsApproval) {
      this.licensing.status = {
        label: 'LicensingInfo.Status.' + LicensingStatus[licensingStatus],
        icon: 'check-circle',
        iconColor: '#2D9D78'
      }
    } else {
      this.licensing.status = {
        label: 'LicensingInfo.Status.' + LicensingStatus[licensingStatus],
        icon: 'times-circle',
        iconColor: ''
      }
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.clear();
  }
}
