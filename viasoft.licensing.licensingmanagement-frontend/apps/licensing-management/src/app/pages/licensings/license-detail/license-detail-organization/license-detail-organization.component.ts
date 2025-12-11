import { Component, Inject, OnDestroy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MessageService, VsSubscriptionManager } from '@viasoft/common';
import { VsDialog } from '@viasoft/components/dialog';
import {
  VsGridCheckboxColumn,
  VsGridGetInput,
  VsGridGetResult,
  VsGridOptions,
  VsGridSimpleColumn,
} from '@viasoft/components/grid';
import { LicensingFormCurrentTab } from '@viasoft/licensing-management/app/tokens/enum/licensing-form-current-tab.enum';
import {
  AddOrganizationEnvironmentModalComponent,
} from '@viasoft/licensing-management/app/tokens/modals/add-organization-environment-modal/add-organization-environment-modal.component';
import {
  AddOrganizationUnitModalComponent,
} from '@viasoft/licensing-management/app/tokens/modals/add-organization-unit-modal/add-organization-unit-modal.component';
import { LicenseOrganizationService } from '@viasoft/licensing-management/app/tokens/services/license-organization.service';
import { OrganizationUnit, OrganizationUnitEnvironment } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable, of } from 'rxjs';
import { first, map, tap } from 'rxjs/operators';

import { LicensingsFormControlServices } from '../../licensings-form-control.service';
import { LicensingsService } from '../../licensings.service';
import { LicenseDetailComponent } from '../license-detail.component';

@Component({
  selector: 'app-license-detail-organization',
  templateUrl: './license-detail-organization.component.html',
  styleUrls: ['./license-detail-organization.component.scss']
})
export class LicenseDetailOrganizationComponent implements OnDestroy {
  private subs = new VsSubscriptionManager();
  private units: OrganizationUnit[];
  public currentUnit: OrganizationUnit;
  public unitGridOptions: VsGridOptions;
  public environmentGridOptions: VsGridOptions;

  public get hasUnitSelected(): boolean {
    return Boolean(this.currentUnit?.id);
  }

  constructor(
    private readonly licensingsService: LicensingsService,
    private readonly licensingsFormControl: LicensingsFormControlServices,
    private readonly licenseOrganizationService: LicenseOrganizationService,
    private vsDialog: VsDialog,
    private dialog: MatDialog,
    private messageService: MessageService,
    @Inject(LicenseDetailComponent) private licenseDetail: LicenseDetailComponent
  ) {
    this.licensingsService.setCurrentTab(LicensingFormCurrentTab.Organization);
    this.subs.add('grid-refresher', this.licensingsFormControl.gridRefresherSubject.subscribe(() => {
      this.unitGridOptions.refresh();
    }));
    this.setupUnitGridOptions();
    this.setupEnvironmentGridOptions();
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  private setupUnitGridOptions(): void {
    this.unitGridOptions = new VsGridOptions();
    this.unitGridOptions.enableQuickFilter = false;
    this.unitGridOptions.id = 'ed4cd6af-aff8-418f-97c9-d687548124b9';
    this.unitGridOptions.selectionMode = 'single';
    this.unitGridOptions.actions = [
      {
        icon: 'plus',
        tooltip: 'Organization.Unit.AddEnvironment',
        callback: (_, unit) => this.addEnvironment(unit),
        condition: () => this.licenseDetail.hasPoliciesForUpdateDetails
      },
      {
        icon: 'edit',
        tooltip: 'Organization.Unit.EditUnit',
        callback: (_, unit) => this.editUnit(unit),
        condition: () => this.licenseDetail.hasPoliciesForUpdateDetails
      },
      {
        icon: 'play',
        tooltip: 'Organization.Unit.Activate',
        callback: (_, unit) => this.activateUnit(unit),
        condition: (_, unit: OrganizationUnit) => !unit.isActive && this.licenseDetail.hasPoliciesForUpdateDetails
      },
      {
        icon: 'ban',
        tooltip: 'Organization.Unit.Deactivate',
        callback: (_, unit) => this.deactivateUnit(unit),
        condition: (_, unit: OrganizationUnit) => unit.isActive && this.licenseDetail.hasPoliciesForUpdateDetails
      }
    ];
    this.unitGridOptions.columns = [
      new VsGridSimpleColumn({
        field: 'name',
        headerName: 'Organization.Unit.Fields.Name',
        width: 100
      }),
      new VsGridSimpleColumn({
        field: 'description',
        headerName: 'Organization.Unit.Fields.Description',
        width: 200
      }),
    ];
    this.unitGridOptions.get = (input) => this.getUnit(input);
    this.unitGridOptions.select = (_, unit) => this.selectUnit(unit);
  }

  private setupEnvironmentGridOptions(): void {
    this.environmentGridOptions = new VsGridOptions();
    this.environmentGridOptions.id = 'ad4cd6af-aff9-412f-12a7-d447548124b6';
    this.environmentGridOptions.enableQuickFilter = false;
    this.environmentGridOptions.actions = [
      {
        icon: 'play',
        tooltip: 'Organization.UnitEnvironment.Activate',
        callback: (_, environment) => this.activateEnvironment(environment),
        condition: (_, environment: OrganizationUnitEnvironment) => Boolean(this.currentUnit?.isActive) && !environment.isActive && this.licenseDetail.hasPoliciesForUpdateDetails
      },
      {
        icon: 'ban',
        tooltip: 'Organization.UnitEnvironment.Deactivate',
        callback: (_, environment) => this.deactivateEnvironment(environment),
        condition: (_, environment: OrganizationUnitEnvironment) => Boolean(this.currentUnit?.isActive) && environment.isActive && this.licenseDetail.hasPoliciesForUpdateDetails
      },
      {
        icon: 'copy',
        tooltip: 'Organization.UnitEnvironment.CopyEnvironmentIdTooltip',
        callback: (_, environment: OrganizationUnitEnvironment) => this.copyToTheClipboard(environment.id),
        condition: () => Boolean(this.currentUnit?.isActive)
      }
    ];
    this.environmentGridOptions.columns = [
      new VsGridSimpleColumn({
        field: 'name',
        headerName: 'Organization.UnitEnvironment.Fields.Name'
      }),
      new VsGridSimpleColumn({
        field: 'description',
        headerName: 'Organization.UnitEnvironment.Fields.Description'
      }),
      new VsGridCheckboxColumn({
        field: 'isWeb',
        headerName: 'Organization.UnitEnvironment.Fields.IsWeb',
        disabled: true,
        sorting: {
          disable: true
        }
      }),
      new VsGridCheckboxColumn({
        field: 'isDesktop',
        headerName: 'Organization.UnitEnvironment.Fields.IsDesktop',
        disabled: true,
        sorting: {
          disable: true
        }
      }),
      new VsGridCheckboxColumn({
        field: 'isProduction',
        headerName: 'Organization.UnitEnvironment.Fields.IsProduction',
        disabled: true,
        sorting: {
          disable: true
        }
      }),
      new VsGridCheckboxColumn({
        field: 'isMobile',
        headerName: 'Organization.UnitEnvironment.Fields.IsMobile',
        disabled: true,
        sorting: {
          disable: true
        }
      }),
      new VsGridSimpleColumn({
        field: 'databaseName',
        headerName: 'Organization.UnitEnvironment.Fields.DatabaseName'
      }),
      new VsGridSimpleColumn({
        field: 'desktopDatabaseVersion',
        headerName: 'Organization.UnitEnvironment.Fields.Version'
      })
    ];
    this.environmentGridOptions.get = (input) => this.getEnvironment(input);
    this.environmentGridOptions.select = (_, environment) => this.editEnvironment(environment);
  }

  private getUnit(input: VsGridGetInput): Observable<VsGridGetResult> {
    const licensedTenantId = this.licensingsService.getLicensedTenantId();
    return this.licenseOrganizationService.getUnitsByOrganization(this.licenseDetail.licenseIdentifier, licensedTenantId, input.filter, input.advancedFilter, input.sorting, input.skipCount, input.maxResultCount)
      .pipe(
        tap((result) => {
          this.units = result ? result.items : [];
          if (this.currentUnit) {
            // Update environments if selected unit was deleted
            const selectedUnitInResult = this.units.find(u => u.id == this.currentUnit.id);
            if (!selectedUnitInResult) {
              this.currentUnit = undefined;
            } else {
              this.selectUnit(selectedUnitInResult);
            }
          }
          if (this.units.length === 1) {
            this.selectUnit(this.units[0]);
          }
        }),
        map((res) => (new VsGridGetResult(res.items, res.totalCount)))
      );
  }

  private editUnit(unit: OrganizationUnit): void {
    if (!this.licenseDetail.hasPoliciesForUpdateDetails) {
      return;
    }
    this.dialog.open(AddOrganizationUnitModalComponent, {
      data: {
        unitId: unit.id,
        licenseIdentifier: this.licenseDetail.licenseIdentifier
      }
    })
      .afterClosed()
      .pipe(first())
      .subscribe((result) => {
        if (result) {
          this.unitGridOptions.refresh();
          this.selectUnit(unit);
        }
      });
  }

  private selectUnit(unit: OrganizationUnit = this.currentUnit, shouldUpdate = true): void {
    if (this.currentUnit !== unit) {
      this.currentUnit = unit;
      if (shouldUpdate) {
        this.environmentGridOptions.refresh();
      }
    }
  }

  private activateUnit(unit: OrganizationUnit): void {
    if (!this.licenseDetail.hasPoliciesForUpdateDetails) {
      return;
    }
    this.licenseOrganizationService.activateUnit(this.licenseDetail.licenseIdentifier, unit.id)
      .pipe(first())
      .subscribe(() => {
        this.selectUnit(unit, false);
        this.environmentGridOptions.refresh();
        this.unitGridOptions.refresh();
        this.messageService.info('Organization.Unit.Warnings.EnvironmentsWillNotBeActivatedAutomatically')
          .pipe(first())
          .subscribe();
      });
  }

  private deactivateUnit(unit: OrganizationUnit): void {
    if (!this.licenseDetail.hasPoliciesForUpdateDetails) {
      return;
    }
    this.licenseOrganizationService.deactivateUnit(this.licenseDetail.licenseIdentifier, unit.id)
      .pipe(first())
      .subscribe(() => {
        this.unitGridOptions.refresh();
        this.selectUnit(unit);
      });
  }

  private addEnvironment(unit: OrganizationUnit): void {
    if (!this.licenseDetail.hasPoliciesForUpdateDetails) {
      return;
    }
    this.vsDialog.open(AddOrganizationEnvironmentModalComponent, {
      licenseIdentifier: this.licenseDetail.licenseIdentifier,
      unitId: unit.id
    })
      .afterClosed()
      .pipe(first())
      .subscribe((shouldUpdate: boolean) => {
        if (shouldUpdate) {
          this.selectUnit(unit, false);
          this.environmentGridOptions.refresh();
        }
      });
  }

  private editEnvironment(environment: OrganizationUnitEnvironment): void {
    if (!this.licenseDetail.hasPoliciesForUpdateDetails) {
      return;
    }
    this.vsDialog.open(AddOrganizationEnvironmentModalComponent, {
      licenseIdentifier: this.licenseDetail.licenseIdentifier,
      environment: environment,
      unitId: this.currentUnit.id
    })
      .afterClosed()
      .pipe(first())
      .subscribe((shouldUpdate: boolean) => {
        if (shouldUpdate) {
          this.environmentGridOptions.refresh();
        }
      });
  }

  private getEnvironment(input: VsGridGetInput): Observable<any> {
    const unitId = this.currentUnit?.id;
    if (!unitId) {
      return of(new VsGridGetResult([], 0));
    }
    return this.licenseOrganizationService.getEnvironmentByUnit(this.licenseDetail.licenseIdentifier, unitId, false, null, null, null, input.filter, input.advancedFilter, input.sorting, input.skipCount, input.maxResultCount)
      .pipe(map((res) => (new VsGridGetResult(res.items, res.totalCount))));
  }

  private activateEnvironment(data: OrganizationUnitEnvironment): void {
    if (!this.licenseDetail.hasPoliciesForUpdateDetails) {
      return;
    }
    this.licenseOrganizationService.activateEnvironment(this.licenseDetail.licenseIdentifier, data.id)
      .pipe(first())
      .subscribe(() => {
        this.environmentGridOptions.refresh();
      });
  }

  private deactivateEnvironment(data: OrganizationUnitEnvironment): void {
    if (!this.licenseDetail.hasPoliciesForUpdateDetails) {
      return;
    }
    this.licenseOrganizationService.deactivateEnvironment(this.licenseDetail.licenseIdentifier, data.id)
      .pipe(first())
      .subscribe(() => {
        this.environmentGridOptions.refresh();
      });
  }

  private copyToTheClipboard(text: string) : void{
    navigator.clipboard.writeText(text);
  }
}
