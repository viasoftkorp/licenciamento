import {Component, OnInit} from '@angular/core';
import {MatDialog} from "@angular/material/dialog";
import {Observable, of} from "rxjs";
import {map, tap} from "rxjs/operators";

import {
  VsDialog,
  VsGridCheckboxColumn,
  VsGridGetInput,
  VsGridGetResult,
  VsGridOptions,
  VsGridSimpleColumn
} from "@viasoft/components";
import {VsAuthorizationService, VsSubscriptionManager} from "@viasoft/common";
import {LicensingsEnvironmentService} from "../licensings-environment.service";
import {OrganizationUnit} from "../../../../../client/customer-licensing/model/organizationUnit";
import {OrganizationUnitEnvironment} from "../../../../../client/customer-licensing/model/organizationUnitEnvironment";
import {GetUnitForGridInput} from "../../../../../client/customer-licensing/model/getUunitForGridInput";
import {getEnvironmentForGridInput} from "../../../../../client/customer-licensing/model/getEnvironmentForGridInput";
import {AddOrganizationUnitModalComponent} from "../modals/add-organization-unit-modal/add-organization-unit-modal.component";
import {AddOrganizationEnvironmentModalComponent} from "../modals/add-organization-environment-modal/add-organization-environment-modal.component";
import {Policies} from "../../../../authorizations/policies.class";
import {TranslateService} from "@ngx-translate/core";


@Component({
  selector: 'app-licensing-environment-organizational',
  templateUrl: './licensing-environment-organizational.component.html',
  styleUrls: ['./licensing-environment-organizational.component.scss']
})

export class LicensingEnvironmentOrganizationalComponent implements OnInit {

  public currentUnitName: string = "Environment.Environment.EnvironmentUnits";
  public loaded:boolean = false;
  public canUpdate: boolean = false;
  public unitGridOptions: VsGridOptions = new VsGridOptions();
  public environmentGridOptions: VsGridOptions = new VsGridOptions();
  public currentUnit: OrganizationUnit;
  private units: OrganizationUnit[];
  private licensedTenantId: string;
  private tenantId: string;
  private subs: VsSubscriptionManager = new VsSubscriptionManager();
  private policies = Policies;

  public get hasUnitSelected(): boolean {
    return Boolean(this.currentUnit?.id);
  }

  public get canShowUnitEnvironment() {
   return Boolean (this.hasUnitSelected && this.loaded)
  }

  constructor(
    private licensingsEnvironmentService: LicensingsEnvironmentService,
    private authorizationService: VsAuthorizationService,
    private dialog: MatDialog,
    private vsDialog: VsDialog,
   private translateService: TranslateService
  ) {}

  ngOnInit(): void {
    this.userAuthorizations();
    this.tenantId = this.licensingsEnvironmentService.getTenantId();
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  private userAuthorizations(): void {
    this.authorizationService.isGranted([this.policies.UpdateEnvironments])
      .then(val => {
        this.canUpdate = val;
        this.init();
      });
  }

  private init(): void {
    this.setupUnitGridOptions();
    this.setupEnvironmentGridOptions();
  }

  private setupUnitGridOptions(): void {
    this.unitGridOptions.id = 'ecd041f4-33c7-49c5-9ae2-381aba4d1f7f';
    this.unitGridOptions.selectionMode = 'single';
    this.unitGridOptions.enableQuickFilter = false;
    this.unitGridOptions.actions = [
      {
        icon: 'plus',
        callback: (_, unit) => this.addEnvironment(unit),
        tooltip:'Environment.Organization.Tooltips.Add',
        condition: () => this.canUpdate
      },
      {
        icon: 'edit',
        callback: (_, unit) => this.editUnit(unit),
        tooltip: 'Environment.Organization.Tooltips.Edit',
        condition: () => this.canUpdate
      },
      {
        icon: 'play',
        callback: (_, unit) => this.activateUnit(unit),
        tooltip: 'Environment.Organization.Tooltips.Activate',
        condition: (_, unit: OrganizationUnit) => !unit.isActive && this.canUpdate
      },
      {
        icon: 'ban',
        callback: (_, unit) => this.deactivateUnit(unit),
        tooltip: 'Environment.Organization.Tooltips.Deactivate',
        condition: (_, unit: OrganizationUnit) => unit.isActive && this.canUpdate
      }
    ];

    this.unitGridOptions.columns = [
      new VsGridSimpleColumn({
        field: 'name',
        headerName: 'Environment.Organization.UnitsFieldName.Name',
        width: 100
      }),
      new VsGridSimpleColumn({
        field: 'description',
        headerName: 'Environment.Organization.UnitsFieldName.Description'
      })
    ];

    this.subs.add('grid-options',
      this.licensingsEnvironmentService.getLicensedTenantId().subscribe(res =>
        {
          this.licensedTenantId = res.licensedTenantId;

          this.unitGridOptions.get = (input) => this.getUnit(input);
          this.unitGridOptions.select = (_, unit) => this.selectUnit(unit);
          this.loaded = true;
        }
      )
    )
    this.subs.add('update-unit-grid-after-adding',
      this.licensingsEnvironmentService.updatingUnitGrid.subscribe(() => this.unitGridOptions.refresh())
    )
  }

  private setupEnvironmentGridOptions(): void {
    this.environmentGridOptions = new VsGridOptions();
    this.environmentGridOptions.id = 'ad4cd6af-aff9-412f-12a7-d447548124b6';
    this.environmentGridOptions.enableQuickFilter = false;
    this.environmentGridOptions.actions = [
      {
        icon: 'play',
        tooltip: 'Environment.Environment.UnitEnvironment.Tooltips.Activate',
        callback: (_, environment) => this.activateEnvironment(environment),
        condition: (_, environment: OrganizationUnitEnvironment) => Boolean(this.currentUnit?.isActive) && !environment.isActive && this.canUpdate
      },
      {
        icon: 'ban',
        tooltip: 'Environment.Environment.UnitEnvironment.Tooltips.Deactivate',
        callback: (_, environment) => this.deactivateEnvironment(environment),
        condition: (_, environment: OrganizationUnitEnvironment) => Boolean(this.currentUnit?.isActive) && environment.isActive && this.canUpdate
      }
    ];
    this.environmentGridOptions.columns = [
      new VsGridSimpleColumn({
        field: 'name',
        headerName: 'Environment.Environment.UnitEnvironment.FieldName.Name',
        width: 100
      }),
      new VsGridSimpleColumn({
        field: 'description',
        headerName: 'Environment.Environment.UnitEnvironment.FieldName.Description',
      }),
      new VsGridCheckboxColumn({
        field: 'isWeb',
        headerName: 'Environment.Environment.UnitEnvironment.FieldName.IsWeb',
        disabled: true,
        sorting: {
          disable: true
        },
        width: 50
      }),
      new VsGridCheckboxColumn({
        field: 'isDesktop',
        headerName: 'Environment.Environment.UnitEnvironment.FieldName.IsDesktop',
        disabled: true,
        sorting: {
          disable: true
        },
        width: 50
      }),
      new VsGridCheckboxColumn({
        field: 'isProduction',
        headerName: 'Environment.Environment.UnitEnvironment.FieldName.IsProduction',
        disabled: true,
        sorting: {
          disable: true
        },
        width: 50
      }),
      new VsGridSimpleColumn({
        field: 'databaseName',
        headerName: 'Environment.Environment.UnitEnvironment.FieldName.DatabaseName',
        width: 100
      }),
      new VsGridSimpleColumn({
        field: 'desktopDatabaseVersion',
        headerName: 'Environment.Environment.UnitEnvironment.FieldName.Version',
        width: 50
      })
    ];

    this.environmentGridOptions.get = (input) => this.getEnvironment(input);
    this.environmentGridOptions.select = (_, environment) => this.editEnvironment(environment);
  }

  private getUnit(input: VsGridGetInput): Observable<VsGridGetResult> {
   return this.licensingsEnvironmentService.getUnitForGrid<GetUnitForGridInput>(this.licensedTenantId, input.sorting, input.filter, input.advancedFilter, input.skipCount, input.maxResultCount)
     .pipe(
       tap((result) => {
         this.units = result ? result.items : [];
         if (this.currentUnit) {
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
        map((result) => (new VsGridGetResult(result.items, result.totalCount)))
     )
  }

  private editUnit(unit: OrganizationUnit): void {
    this.subs.add('editUnit-modal',
      this.dialog.open(AddOrganizationUnitModalComponent, {
        data: {
          unitId: unit.id,
          licenseIdentifier: this.licensedTenantId
        }
      })
        .afterClosed()
        .subscribe((result) => {
          if (result) {
            this.unitGridOptions.refresh();
          }
        }))
  }

  private selectUnit(unit: OrganizationUnit = this.currentUnit, shouldUpdate = true): void {
    if (this.currentUnit !== unit) {
      this.currentUnit = unit;
      this.currentUnitName = this.translateService.instant("Environment.Environment.EnvironmentUnitsOf") + this.currentUnit.name;
      if (shouldUpdate) {
        this.environmentGridOptions.refresh();
      }
    }
  }

  private activateUnit(unit: OrganizationUnit): void {
    this.subs.add('activateUnit',
      this.licensingsEnvironmentService.activateUnit(unit.id).subscribe(() => {
        this.unitGridOptions.refresh();
      })
    )
  }

 private deactivateUnit(unit: OrganizationUnit): void {
    this.subs.add('deactivate-unit',
      this.licensingsEnvironmentService.deactivateUnit(unit.id).subscribe(() =>
        this.unitGridOptions.refresh()
      ))
  }

 private getEnvironment(input: VsGridGetInput): Observable<any> {
    const unitId = this.currentUnit?.id;
    if (!unitId) {
      return of(new VsGridGetResult([], 0));
    }
    return this.licensingsEnvironmentService.getEnvironmentForGrid<getEnvironmentForGridInput>(
      this.licensedTenantId, unitId, false, null, null, null, input.filter, input.advancedFilter, input.sorting, input.skipCount, input.maxResultCount )
      .pipe(map((result) => (new VsGridGetResult(result.items, result.totalCount))))
  }

  private addEnvironment(unit: OrganizationUnit): void {

    this.subs.add('add-environment',
      this.vsDialog.open(AddOrganizationEnvironmentModalComponent, {
        licenseIdentifier: this.tenantId,
        unitId: unit.id
      })
        .afterClosed()
        .subscribe((shouldUpdate: boolean) => {
          if (shouldUpdate) {
            this.selectUnit(unit, false);
            this.environmentGridOptions.refresh();
          }
        })
    )
  }

  private editEnvironment(environment: OrganizationUnitEnvironment): void {

    if(this.canUpdate) {
      this.subs.add('edit-environment',
      this.vsDialog.open(AddOrganizationEnvironmentModalComponent, {
        licenseIdentifier: this.licensedTenantId,
        environment: environment,
        unitId: this.currentUnit.id
      })
        .afterClosed()
        .subscribe((result: boolean) => {
          if (result) {
            this.environmentGridOptions.refresh();
          }
        }))
    }
  }

  private activateEnvironment(data: OrganizationUnitEnvironment): void {
    this.subs.add('activate-environment',
      this.licensingsEnvironmentService.activateEnvironment(this.licensedTenantId, data.id).subscribe(() => {
        this.environmentGridOptions.refresh();
      }))
  }

  private deactivateEnvironment(data: OrganizationUnitEnvironment): void {
    this.subs.add('deactivate-environment',
      this.licensingsEnvironmentService.deactivateEnvironment(this.licensedTenantId, data.id).subscribe(() =>
        this.environmentGridOptions.refresh()
      ))
  }

}

