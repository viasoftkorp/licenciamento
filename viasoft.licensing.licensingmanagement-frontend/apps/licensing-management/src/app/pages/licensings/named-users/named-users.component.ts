import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageService, VsSubscriptionManager } from '@viasoft/common';
import { VsGridGetInput, VsGridGetResult, VsGridOptions, VsGridSimpleColumn, VsTableAction } from '@viasoft/components';
import { LicensingMode } from '@viasoft/licensing-management/app/tokens/enum/licensing-mode.enum';
import { NamedUserTypes } from '@viasoft/licensing-management/app/tokens/enum/named-user-types.enum';
import { OperationValidation } from '@viasoft/licensing-management/app/tokens/enum/operation-validation.enum';
import { RemoveNamedUserBundleValidationCode } from '@viasoft/licensing-management/app/tokens/enum/remove-named-user-bundle-validation-code.enum';
import { AddNamedUserOutput } from '@viasoft/licensing-management/app/tokens/interfaces/add-named-user-output.interface';
import { GetNamedUserAppOutput } from '@viasoft/licensing-management/app/tokens/interfaces/get-named-user-app-output.interface';
import { GetNamedUserBundleOutput } from '@viasoft/licensing-management/app/tokens/interfaces/get-named-user-bundle-output.interface';
import { NamedUserOutput } from '@viasoft/licensing-management/app/tokens/interfaces/named-user-output.interface';
import { NamedUsersDialogDataInput } from '@viasoft/licensing-management/app/tokens/interfaces/named-users-dialog-data-input.interface';
import { RemoveNamedUserbundleOutput } from '@viasoft/licensing-management/app/tokens/interfaces/remove-named-user-bundle-output.interface';
import { AddNamedUserComponent } from '@viasoft/licensing-management/app/tokens/modals/add-named-user/add-named-user.component';
import { map } from 'rxjs/operators';
import { NamedUsersAppService } from './named-users-app.service';
import { NamedUsersBundleService } from './named-users-bundle.service';
import { UpdateNamedUserBundleOutput } from '@viasoft/licensing-management/app/tokens/interfaces/update-named-user-bundle-output.interface';
import { UpdateNamedUserBundleValidationCode } from '@viasoft/licensing-management/app/tokens/enum/update-named-user-bundle-validation-code.enum';
import { UpdateNamedUserAppOutput } from '@viasoft/licensing-management/app/tokens/interfaces/update-named-user-app-output.interface';
import { UpdateNamedUserAppValidationCode } from '@viasoft/licensing-management/app/tokens/enum/update-named-user-app-validation-code.enum';
import { CreateNamedUserAppOutput } from '@viasoft/licensing-management/app/tokens/interfaces/create-named-user-app-output.interface';
import { CreateNamedUserAppValidationCode } from '@viasoft/licensing-management/app/tokens/enum/create-named-user-app-validation-code.enum';
import { RemoveNamedUserAppOutput } from '@viasoft/licensing-management/app/tokens/interfaces/remove-named-user-app-output.interface';
import { RemoveNamedUserAppValidationCode } from '@viasoft/licensing-management/app/tokens/enum/remove-named-user-app-validation-code.enum';


@Component({
  selector: 'app-named-users',
  templateUrl: './named-users.component.html',
  styleUrls: ['./named-users.component.scss']
})
export class NamedUsersComponent implements OnInit, OnDestroy {

  public grid: VsGridOptions;
  public manageNamedUsersSubTitle: string;
  public availableLicenses: number;
  public licensingMode = this.licensingModeToString(this.data.licensingMode);
  private subs = new VsSubscriptionManager();
  private isNamedBundleLicense = this.data.namedUserType === NamedUserTypes.LicensedBundleNamedUser;

  constructor(
    @Inject(MAT_DIALOG_DATA) private readonly data: NamedUsersDialogDataInput,
    private readonly namedUsersAppService: NamedUsersAppService,
    private readonly namedUsersBundleService: NamedUsersBundleService,
    private readonly dialog: MatDialog,
    private readonly messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.configureGrid();
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  public add(): void {
    this.subs.add(
      'open-create',
      this.dialog.open(AddNamedUserComponent, {data: {
        licensedTenantIdentifier: this.data.licensedTenantIdentifier,
        licensingMode: this.data.licensingMode,
        isCreate: true
      }})
      .afterClosed()
      .subscribe(
        (result: AddNamedUserOutput) => {
          if (!result) {
            return;
          }

          if (this.isNamedBundleLicense) {
            this.subs.add(
              'add-named-bundle',
              this.namedUsersBundleService.create(this.data.licensedTenantId, this.data.licensedEntityId, {
                namedUserEmail: result.key,
                namedUserId: result.value,
                deviceId: result.deviceId,
              }).subscribe(
                () => {
                  this.grid.refresh();
                },
                (output: any) => {
                  this.messageService.error(this.createOperationValidationErrorToString(output.error.operationValidation));
                  this.grid.refresh();
                }
              )
            );
          } else {
            this.subs.add(
              'add-named-app',
              this.namedUsersAppService.create(this.data.licensedTenantId, this.data.licensedEntityId, {
                namedUserEmail: result.key,
                namedUserId: result.value,
                deviceId: result.deviceId
              }).subscribe(
                () => {
                  this.grid.refresh();
                },
                (output: any) => {
                  const error = <CreateNamedUserAppOutput> output.error;
                  this.messageService.error(this.createNamedAppOperationValidationErrorToString(error.validationCode));
                  this.grid.refresh();
                }
              )
            );
          }

        }
    ));
  }

  private configureGrid(): void {
    this.grid = new VsGridOptions();

    this.grid.id = '36af3000-f494-11eb-9d3c-fc4596fac591';
    this.grid.enableQuickFilter = false;
    this.grid.deleteBehaviours.enableAutoDeleteConfirm = true;

    this.grid.columns = [
      new VsGridSimpleColumn({
        headerName: 'licensings.user',
        field: 'namedUserEmail',
        width: 290
      })
    ];

    if (this.data.licensingMode === LicensingMode.Offline) {
      this.grid.columns.push(new VsGridSimpleColumn({
        headerName: 'licensings.identifier',
        field: 'deviceId'
      }));
    }

    this.grid.sizeColumnsToFit = false;

    this.grid.get = (input: VsGridGetInput) => this.get(input);
    this.grid.delete = (i: number, data: NamedUserOutput) => this.delete(data);

    this.grid.actions = [
      <VsTableAction> {
        icon: 'user-pen',
        callback: (i: number, data: NamedUserOutput) => {
          this.subs.add(
            'open-update',
            this.dialog
              .open(AddNamedUserComponent, {data: {
               licensingMode: this.data.licensingMode,
               deviceId: data.deviceId,
               id: data.id,
               tenantId: data.tenantId,
               licensedTenantId: data.licensedTenantId,
               namedUserEmail: data.namedUserEmail,
               namedUserId: data.namedUserId,
               operationValidation: data.operationValidation,
               licensedTenantIdentifier: this.data.licensedTenantIdentifier,
               isCreate: false
              }})
              .afterClosed()
              .subscribe(
                (result: AddNamedUserOutput) => {
                  if (!result) {
                    return;
                  }
                  if (this.isNamedBundleLicense) {
                    this.subs.add(
                      'update-named-bundle-license',
                      this.namedUsersBundleService
                        .update(this.data.licensedTenantId, this.data.licensedEntityId, data.id, {
                          deviceId: result.deviceId,
                          namedUserEmail: result.key,
                          namedUserId: result.value
                        })
                        .subscribe(
                          () => {
                            this.grid.refresh();
                          },
                          (output: any) => {
                            const errorOutput = <UpdateNamedUserBundleOutput> output.error;
                            this.messageService.error(this.updateNamedbundleOperationValidationErrorToString(errorOutput.validationCode));
                            this.grid.refresh();
                          }
                        )
                    );
                  } else {
                    this.subs.add(
                      'update-named-app-license',
                      this.namedUsersAppService
                        .update(this.data.licensedTenantId, this.data.licensedEntityId, data.id, {
                          deviceId: result.deviceId,
                          namedUserEmail: result.key,
                          namedUserId: result.value
                        })
                        .subscribe(
                          () => {
                            this.grid.refresh();
                          },
                          (output: any) => {
                            const errorOutput = <UpdateNamedUserAppOutput> output.error;
                            this.messageService.error(this.updateNamedAppOperationValidationErrorToString(errorOutput.validationCode));
                            this.grid.refresh();
                          }
                        )
                    );
                  }

                }
              )
          );
        }
      }
    ];
  }

  private get(input: VsGridGetInput) {
    if (this.isNamedBundleLicense) {
      return this.namedUsersBundleService.get(this.data.licensedTenantId, this.data.licensedEntityId, input).pipe(
        map(
          (result: GetNamedUserBundleOutput) => {
            this.availableLicenses = this.data.numberOfLicenses - result.namedUserBundleLicenseOutputs.totalCount;
            return new VsGridGetResult(result.namedUserBundleLicenseOutputs.items, result.namedUserBundleLicenseOutputs.totalCount);
          }
        )
      );
    }

    return this.namedUsersAppService.get(this.data.licensedTenantId, this.data.licensedEntityId, input).pipe(
      map(
        (result: GetNamedUserAppOutput) => {
          this.availableLicenses = this.data.numberOfLicenses - result.namedUserAppLicenseOutputs.totalCount;
          return new VsGridGetResult(result.namedUserAppLicenseOutputs.items, result.namedUserAppLicenseOutputs.totalCount);
        }
      )
    );
  }

  private delete(data: NamedUserOutput) {
    if (this.isNamedBundleLicense) {
      this.subs.add(
        'delete-bundle-named-license',
        this.namedUsersBundleService
          .delete(this.data.licensedTenantId, this.data.licensedEntityId, data.id)
          .subscribe(
            () => {
              this.grid.refresh();
            },
            (result: any) => {
              const output = <RemoveNamedUserbundleOutput> result.error;
              this.messageService.error(this.deleteNamedBundleOperationValidationErrorToString(output.validationCode));
              this.grid.refresh();
            }
          )
      );
    } else {
      this.subs.add(
        'delete-app-named-license',
        this.namedUsersAppService
          .delete(this.data.licensedTenantId, this.data.licensedEntityId, data.id)
          .subscribe(
            () => {
              this.grid.refresh();
            },
            (result: any) => {
              const output = <RemoveNamedUserAppOutput> result.error;
              this.messageService.error(this.deleteNamedAppOperationValidationErrorToString(output.validationCode));
              this.grid.refresh();
            }
          )
      );
    }
  }

  private licensingModeToString(mode: LicensingMode): string {
    switch (mode) {
      case LicensingMode.Offline:
        return 'offline';
      case LicensingMode.Online:
        return 'online';
      default:
        throw new Error(`Couldn't find a proper translation for the licensingMode: ${mode}`);
    }
  }

  private createOperationValidationErrorToString(validationCode: OperationValidation): string {
    switch (validationCode) {
      case OperationValidation.NoLicensedBundleWithSuchId:
        return 'licensings.errors.noLicensedProductWithSuchId';
      case OperationValidation.NotANamedLicense:
        return 'licensings.errors.notANamedLicense';
      case OperationValidation.NoTenantWithSuchId:
        return 'licensings.errors.noTenantWithSuchId';
      case OperationValidation.TooManyNamedUserBundleLicenses:
        return 'licensings.errors.TooManyNamedUsers';
      case OperationValidation.NamedUserEmailAlreadyInUse:
        return 'licensings.errors.namedUserEmailAlreadyInUse';
      default:
        return 'licensings.errors.unknownError';
    }
  }

  private deleteNamedBundleOperationValidationErrorToString(validationCode: RemoveNamedUserBundleValidationCode): string {
    switch (validationCode) {
      case RemoveNamedUserBundleValidationCode.NoLicensedBundle:
        return 'licensings.errors.noLicensedProductWithSuchId';
      case RemoveNamedUserBundleValidationCode.NoLicensedTenant:
        return 'licensings.errors.noTenantWithSuchId';
      case RemoveNamedUserBundleValidationCode.NoNamedUser:
        return 'licensings.errors.notANamedLicense';
      default:
        break;
    }
  }

  private updateNamedbundleOperationValidationErrorToString(validationCode: UpdateNamedUserBundleValidationCode): string {
    switch (validationCode) {
      case UpdateNamedUserBundleValidationCode.NoLicensedBundle:
        return 'licensings.errors.noLicensedProductWithSuchId';
      case UpdateNamedUserBundleValidationCode.NoLicensedTenant:
        return 'licensings.errors.noTenantWithSuchId';
      case UpdateNamedUserBundleValidationCode.NoNamedUser:
        return 'licensings.errors.notANamedLicense';
      case UpdateNamedUserBundleValidationCode.NamedUserEmailAlreadyInUse:
        return 'licensings.errors.namedUserEmailAlreadyInUse';
      default:
        break;
    }
  }

  private updateNamedAppOperationValidationErrorToString(validationCode: UpdateNamedUserAppValidationCode): string {
    switch (validationCode) {
      case UpdateNamedUserAppValidationCode.NoLicensedApp:
        return 'licensings.errors.noLicensedAppWithSuchId';
      case UpdateNamedUserAppValidationCode.NoLicensedTenant:
        return 'licensing.errors.noTenantWithSuchId';
      case UpdateNamedUserAppValidationCode.NoNamedUser:
        return 'licensings.errors.notANamedLicense';
      case UpdateNamedUserAppValidationCode.NamedUserEmailAlreadyInUse:
        return 'licensings.errors.namedUserEmailAlreadyInUse';
      default:
        break;
    }
  }

  private createNamedAppOperationValidationErrorToString(validationCode: CreateNamedUserAppValidationCode): string {
    switch (validationCode) {
      case CreateNamedUserAppValidationCode.LicensedAppIsNotNamed:
        return 'licensings.errors.notANamedLicense';
      case CreateNamedUserAppValidationCode.NoLicensedApp:
        return 'licensings.errors.noLicensedAppWithSuchId';
      case CreateNamedUserAppValidationCode.NoLicensedTenant:
        return 'licensings.errors.noTenantWithSuchId';
      case CreateNamedUserAppValidationCode.TooManyNamedUsers:
        return 'licensings.errors.TooManyNamedUsers';
      case CreateNamedUserAppValidationCode.NamedUserEmailAlreadyInUse:
        return 'licensings.errors.namedUserEmailAlreadyInUse';
      default:
        break;
    }
  }

  private deleteNamedAppOperationValidationErrorToString(validationCode: RemoveNamedUserAppValidationCode): string {
    switch (validationCode) {
      case RemoveNamedUserAppValidationCode.NoLicensedApp:
        return 'licensings.errors.noLicensedAppWithSuchId';
      case RemoveNamedUserAppValidationCode.NoLicensedTenant:
        return 'licensings.errors.noTenantWithSuchId';
      case RemoveNamedUserAppValidationCode.NoNamedUser:
        return 'licensings.errors.notANamedLicense';
      default:
        break;
    }
  }

}
