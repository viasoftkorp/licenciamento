import {Component, OnInit} from '@angular/core';
import {MatDialog} from "@angular/material/dialog";

import {VsAuthorizationService, VsSubscriptionManager} from "@viasoft/common";
import {LicensingsEnvironmentService} from "./licensings-environment.service";
import {
  AddOrganizationUnitModalComponent
} from "./modals/add-organization-unit-modal/add-organization-unit-modal.component";
import {Policies} from "../../../authorizations/policies.class";

@Component({
  selector: 'app-licensings-environment',
  templateUrl: './licensings-environment.component.html',
  styleUrls: ['./licensings-environment.component.scss']
})

export class LicensingsEnvironmentComponent implements OnInit {

  public canRead: boolean = false;
  public canUpdate: boolean = false;
  public policies = Policies;
  private tenantId: string;
  private licensedTenantId: string;
  private subs: VsSubscriptionManager = new VsSubscriptionManager();


  constructor(
    private authorizationService: VsAuthorizationService,
    private dialog: MatDialog,
    private licensingEnvironmentService: LicensingsEnvironmentService
  ) {
  }

  public ngOnInit(): void {
    this.environmentAuthorizations();
    this.tenantId = this.licensingEnvironmentService.getTenantId();
    this.subs.add('get-licensed-tenantId',
      this.licensingEnvironmentService.getLicensedTenantId().subscribe(response => {
          this.licensedTenantId = response.licensedTenantId
        }
      )
    )
  }

  public ngOnDestroy(): void {
    this.subs.clear();
  }

  private environmentAuthorizations(): void {
    this.authorizationService.isGranted([this.policies.ReadEnvironments])
      .then(val => {
        this.canRead = val
      });
    this.authorizationService.isGranted([this.policies.UpdateEnvironments])
      .then(val => {
        this.canUpdate = val
      });
  }


  public addOrganizationalUnit(): void {
    if (this.canUpdate && this.licensedTenantId) {
      this.subs.add("add-organizational-unit",
        this.dialog
          .open(AddOrganizationUnitModalComponent, {
            data: {
              organizationId: this.licensedTenantId,
              licenseIdentifier: this.tenantId
            }
          })
          .afterClosed()
          .subscribe((result: boolean) => {
            if (result) {
              this.licensingEnvironmentService.updateUnitGridAfterAdd();
            }
          })
      )

    }
  }

}
