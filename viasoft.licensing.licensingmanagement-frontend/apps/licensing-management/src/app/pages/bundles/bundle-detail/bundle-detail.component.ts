import { Component, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { JQQB_COND_OR, JQQB_OP_CONTAINS, JQQB_OP_EQUAL, MessageService, VsAuthorizationService, VsSubscriptionManager } from '@viasoft/common';
import { VsFilterGetItemsOutput, VsFilterGetItemsInput, VsFilterItem, VsGridGetInput, VsGridGetResult, VsGridOptions, VsGridSimpleColumn } from '@viasoft/components';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';
import {
  LicenseBatchOperationsService,
} from '@viasoft/licensing-management/app/tokens/license-batch-operations/license-batch-operations.service';
import {
  BatchOperationsLoadingComponent,
} from '@viasoft/licensing-management/app/tokens/license-batch-operations/modals/batch-operations-loading/batch-operations-loading.component';
import { AppSelectService } from '@viasoft/licensing-management/app/tokens/services/modals-service/app-select.service';
import {
  SoftwareSelectService,
} from '@viasoft/licensing-management/app/tokens/services/modals-service/software-select.service';
import {
  BundleCreateInput,
  BundledAppCreateInput,
  BundledAppDeleteInput,
  BundleUpdateInput,
  SoftwareCreateOutput,
} from '@viasoft/licensing-management/clients/licensing-management';
import { UUID } from 'angular2-uuid';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { concatMap, map, mergeMap, tap } from 'rxjs/operators';

import { DomainsService } from '../../../tokens/services/domains.service';
import { SoftwaresService } from '../../softwares/softwares.service';
import { BundlesFormControlService } from '../bundles-form-control.service';
import { BundlesService } from '../bundles.service';

@Component({
  selector: 'app-bundle-detail',
  templateUrl: './bundle-detail.component.html',
  styleUrls: ['./bundle-detail.component.scss']
})
export class BundleDetailComponent implements OnDestroy {

  id: string;
  softwareId: string;
  form: FormGroup;
  bundle: any;
  grid: VsGridOptions;
  bundled: any;
  appsInBundle: any;
  listOfDomains: Array<string> = [];
  isValid$ = new BehaviorSubject(this.isValid);
  finishAuthorizationRequest = false;
  isEditing = false;
  isDomainLoaded = false;
  private subs: VsSubscriptionManager = new VsSubscriptionManager();

  policies = {
    updateBundle: false,
    insertAppInLicenses: false,
    removeAppInLicenses: false
  };

  get isValid() {
    return !!(this.form && this.form.valid && this.form.dirty);
  }

  constructor(
    private bundlesService: BundlesFormControlService,
    private activatedRoute: ActivatedRoute,
    private bundles: BundlesService,
    private softwares: SoftwaresService,
    private router: Router,
    private notification: MessageService,
    private softwareSelectService: SoftwareSelectService,
    private AppsSelectService: AppSelectService,
    private domainsService: DomainsService,
    private licenseBatchOperationsService: LicenseBatchOperationsService,
    private dialog: MatDialog,
    private readonly authorizationService: VsAuthorizationService
  ) {
    this.getDomains();
    this.setAuthorizations();
  }

  initComponentAfterGetAuthorizations() {
    this.subs.add('bundleSubject', this.bundlesService.bundlesSubject.subscribe(
      () => {
        this.router.navigate(['/products/new']);
      }));

    this.isValid$.subscribe(valid => {
      if (valid) {
        this.bundlesService.bundleFormIsValid();
      } else {
        this.bundlesService.bundleFormIsInvalid();
      }
    });

    this.bundle = {
      id: UUID.UUID(),
      name: '',
      identifier: '',
      isActive: true,
      softwareId: '',
      isCustom: false
    } as BundleCreateInput;

    this.bundled = {
      bundleId: '',
      appId: ''
    } as BundledAppCreateInput;

    this.id = this.activatedRoute.snapshot.paramMap.get('id');

    if (this.id) {
      this.isEditing = true;
      this.subs.add('getBundleById', this.bundles.getById(this.id).subscribe(data => {
        if (data) {
          this.bundle = data as BundleUpdateInput;
          this.createForm();
          this.subs.add('getSoftwareById', this.softwares.getById(data.softwareId)
            .subscribe(software => {
              this.form.get('software').setValue(software.name);
            }));
        }
      }));
    } else {
      this.createForm();
    }

    this.subs.add('saveBundle', this.bundlesService.saveBundlesSubject.subscribe(
      () => {
        this.saveOnClick();
      }
    ));
  }

  ngOnDestroy(): void {
    this.subs.clear();
    this.bundlesService.bundleFormIsInvalid();
  }

  createForm() {
    if (this.form) {
      this.form.reset({
        name: this.bundle.name,
        identifier: this.bundle.identifier,
        softwareId: this.bundle.softwareId,
        software: '',
        isActive: this.bundle.isActive,
        isCustom: this.bundle.isCustom
      });
    } else {
      this.form = new FormGroup({
        name: new FormControl(this.bundle.name, [Validators.required]),
        identifier: new FormControl(this.bundle.identifier, [Validators.required]),
        softwareId: new FormControl(this.bundle.softwareId, [Validators.required]),
        software: new FormControl('', [Validators.required]),
        isActive: new FormControl(this.bundle.isActive),
        isCustom: new FormControl(this.bundle.isCustom)
      });
    }

    this.subs.add('formValueChanges', this.form.valueChanges.subscribe(
      () => {
        setTimeout(() => {
          this.isValid$.next(this.isValid);
        }, 0);
      }
    ));
    if (!this.policies.updateBundle && this.isEditing) {
      this.setFormDisabled();
    }
  }

  setFormDisabled() {
    this.form.disable();
  }

  public newAppOnClick(): void {
    let modalLoading: MatDialogRef<BatchOperationsLoadingComponent>;
    let appId: string;
    this.subs.add('newAppDialog', this.AppsSelectService.openDialog({
      listOfDomains: this.listOfDomains
    }).pipe(
      concatMap((res) => {
        if (!res) {
          return of(undefined);
        } else {
          appId = res.id;
          return this.bundles.addAppToBundle({ bundleId: this.id, appId: res.id } as BundledAppCreateInput);
        }
      })
    ).pipe(
      mergeMap((res) => {
        this.grid.refresh();
        if (res && this.policies.insertAppInLicenses) {
          if (res.operationValidationDescription === 'DuplicatedIdentifier') {
            this.notification.info(
              'common.error.identifier_already_exists'
            );
          } else if (res.isBundleLicensedInAnyLicensing) {
            return this.notification.confirm('products.addAppInLincense');
          }
        }
        return of(undefined);
      })
    ).pipe(
      mergeMap(res => {
        if (res) {
          modalLoading = this.dialog.open(BatchOperationsLoadingComponent);
          modalLoading.disableClose = true;
          return this.licenseBatchOperationsService.insertNewAppFromBundleInLicenses({ bundleId: this.id, appId: appId });
        }
        return of(undefined);
      })
    ).subscribe(
      () => { },
      () => {
        this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation(true);
      },
      () => {
        this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation();
      }
    ));
  }

  saveOnClick() {
    if (!this.id) {
      this.createBundle();
    } else {
      this.updateBundle();
    }
  }

  onSoftwareSearch() {
    this.subs.add('getOpenDialog', this.softwareSelectService.openDialog({ onlyActivesoftwares: true }).subscribe((software: SoftwareCreateOutput) => {
      if (!software) {
        return;
      }
      this.form.get('software').setValue(software.name);
      this.form.get('softwareId').setValue(software.id);
    }
    ));
  }

  onSoftwareSearchCleaned() {
    this.form.get('softwareId').setValue('');
    this.form.get('software').setValue('');
  }

  createBundle() {
    this.bundle.name = this.form.get('name').value;
    this.bundle.identifier = this.form.get('identifier').value;
    this.bundle.softwareId = this.form.get('softwareId').value;
    this.bundle.isActive = this.form.get('isActive').value;
    this.bundle.isCustom = this.form.get('isCustom').value;

    this.subs.add('createBundle', this.bundles.create(this.bundle)
      .subscribe(result => {
        if (result.operationValidationDescription === 'DuplicatedIdentifier') {
          this.notification.error(
            'common.error.identifier_already_exists',
            'common.error.could_not_create|name:' + this.bundle.name
          );
        } else {
          this.router.navigate(['products/', result.id]);
        }
      }));
  }

  updateBundle() {
    this.bundle.name = this.form.get('name').value;
    this.bundle.identifier = this.form.get('identifier').value;
    this.bundle.softwareId = this.form.get('softwareId').value;
    this.bundle.isActive = this.form.get('isActive').value;
    this.bundle.isCustom = this.form.get('isCustom').value;

    this.subs.add('updateBundle', this.bundles.update(this.bundle)
      .subscribe(result => {
        if (result.operationValidationDescription === 'DuplicatedIdentifier') {
          this.notification.error(
            'common.error.identifier_already_exists',
            'common.error.could_not_edit|name:' + this.bundle.name
          );
        } else {
          this.form.markAsPristine();
        }
      }));
  }

  configureGrid() {
    this.grid = new VsGridOptions();
    this.grid.columns = [
      new VsGridSimpleColumn({
        headerName: 'apps.name',
        field: 'name',
        width: 100,
      }),
      new VsGridSimpleColumn({
        headerName: 'apps.identifier',
        field: 'identifier',
        width: 100,
      }),
      new VsGridSimpleColumn({
        headerName: 'apps.domains.domain',
        field: 'domain',
        width: 50,
        filterOptions: this.domainsService.getDomainsFilterOptions(this.listOfDomains),
        translate: true,
        format: (value) => {
          if (this.listOfDomains[value] !== undefined) {
            return 'apps.domains.' + this.listOfDomains[value];
          }
        }
      })
    ];
    this.grid.enableSorting = false;
    this.grid.enableFilter = true;
    this.grid.enableQuickFilter = false;
    if (this.policies.updateBundle) {
      this.grid.delete = (i, data) => this.delete(data);
    }
    this.grid.get = (input: VsGridGetInput) => this.get(input);
  }

  get(input: VsGridGetInput): Observable<any> {
    return this.bundles.getAllAppsInBundle(
      this.id,
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount
    )
      .pipe(
        map((list) =>
          new VsGridGetResult(list.items, list.totalCount)
        )
      );
  }

  delete(data: any) {
    if (data.id) {
      let modalLoading: MatDialogRef<BatchOperationsLoadingComponent>;
      this.subs.add('deleteConfirm', this.notification.confirm(
        'common.notification.action_irreversible',
        'common.notification.confirm_deletion|name:' + data.name
      ).pipe(
        concatMap((res) => {
          if (res) {
            return this.bundles.removeAppFromBundle({ bundleId: this.id, appId: data.id } as BundledAppDeleteInput)
          }
          return of(undefined);
        })
      ).pipe(
        mergeMap((res) => {
          this.grid.refresh();
          if (res && this.policies.removeAppInLicenses) {
            if (!res.success && res.errors.length > 0 && res.errors[0].errorCodeReason === 'UsedByOtherRegister') {
              this.notification.error(
                'common.error.product_is_being_used',
                'common.error.could_not_delete|name:' + data.name
              );
            } else if (res.isBundleLicensedInAnyLicensing) {
              return this.notification.confirm('bundles.removeAppInLicense');
            }
          }
          return of(undefined);
        })
      ).pipe(
        mergeMap((res) => {
          if (res) {
            modalLoading = this.dialog.open(BatchOperationsLoadingComponent);
            modalLoading.disableClose = true;
            return this.licenseBatchOperationsService.removeAppFromBundleInLicenses({ bundleId: this.id, appId: data.id });
          }
          return of(undefined);
        })
      ).subscribe(
        () => { },
        () => {
          this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation(true);
        },
        () => {
          this.licenseBatchOperationsService.emitErrorOrFinishInBatchOperation();
        }
      )
      );
    }
  }

  getDomains() {
    this.subs.add('domain', this.domainsService.getDomains().pipe(
      tap((value) => {
        for (const key in value) {
          if (key) {
            this.listOfDomains.push(key);
          }
        }
      }),
      tap(() => {
        this.isDomainLoaded = true;
      })
    ).subscribe());
  }

  getAuthorizations() {
    return this.authorizationService.isGranted(Policies.UpdateBundle);
  }

  setAuthorizations(): void {
    this.authorizationService
      .isGrantedMap([Policies.UpdateBundle, Policies.BatchOperationInsertAppInLicenses])
      .then((result: Map<string, boolean>) => {
        this.policies = {
          updateBundle: result.get(Policies.UpdateBundle),
          insertAppInLicenses: result.get(Policies.BatchOperationInsertAppInLicenses),
          removeAppInLicenses: result.get(Policies.BatchOperationRemoveAppInLicenses),
        };
        this.finishAuthorizationRequest = true;
        this.configureGrid();
        this.initComponentAfterGetAuthorizations();
      });
  }
}
