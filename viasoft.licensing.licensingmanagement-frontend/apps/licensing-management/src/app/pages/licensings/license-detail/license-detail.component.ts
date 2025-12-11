import { Component, Inject, OnDestroy, OnInit, Optional } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DataLivelyService, ENVIRONMENT, IVsEnvironment, MessageService, VsAuthorizationService, VsSubscriptionManager } from '@viasoft/common';
import { VsSelectOption } from '@viasoft/components/select';
import { LicenseType } from '@viasoft/licensing-management/app/tokens/classes/licenseType.class';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';
import { LicensingFormCurrentTab } from '@viasoft/licensing-management/app/tokens/enum/licensing-form-current-tab.enum';
import {
  FileProviderConfigurationModalComponent,
} from '@viasoft/licensing-management/app/tokens/file-provider-configuration/file-provider-configuration-modal/file-provider-configuration-modal.component';
import {
  AddOrganizationUnitModalComponent,
} from '@viasoft/licensing-management/app/tokens/modals/add-organization-unit-modal/add-organization-unit-modal.component';
import {
  LicenseConsumeInfoComponent,
} from '@viasoft/licensing-management/app/tokens/modals/license-consume-info/license-consume-info.component';
import {
  LicensesNumberSelectComponent,
} from '@viasoft/licensing-management/app/tokens/modals/licenses-number-select/licenses-number-select.component';
import {
  LooseAppsNumberSelectComponent,
} from '@viasoft/licensing-management/app/tokens/modals/loose-apps-number-select/loose-apps-number-select.component';
import {
  AccountSelectService,
} from '@viasoft/licensing-management/app/tokens/services/modals-service/account-select.service';
import { cpfOrCnpjValidator } from '@viasoft/licensing-management/app/tokens/validators/cnpjOrCpf.validator';
import {
  InfrastructureConfigurationUpdateInput,
  LicensedAppCreateOutput,
  OperationValidation,
  OperationValidationBaseCrudResponseError,
} from '@viasoft/licensing-management/clients/licensing-management';
import { UUID } from 'angular2-uuid';
import { BehaviorSubject, Subscription } from 'rxjs';
import { first, tap } from 'rxjs/operators';

import { DomainsService } from '../../../tokens/services/domains.service';
import { LicenseTenantCreateInput } from '../../../tokens/interfaces/license-tenant-create-input.interface';
import { LicenseConsumeType } from '../../../tokens/enum/license-consume-type.enum';
import { LicensingsFormControlServices } from '../licensings-form-control.service';
import { LicensingsService } from '../licensings.service';
import {
  LicenseDetailInfrastructureConfigurationService,
} from './license-detail-infrastructure-configuration/license-detail-infrastructure-configuration.service';
import { ProductType } from '@viasoft/licensing-management/app/tokens/enum/ProductType';
import { LicensingStatus } from '@viasoft/licensing-management/app/tokens/enum/licensing-status.enum';
import { validateMinDate } from '@viasoft/licensing-management/app/tokens/utils/validate-min-date.function';
import { LicenseDetailLicensesServerService } from './license-detail-licenses-server/license-detail-licenses-server.service';
import {
  LicensedTenantSettingsOutput
} from "@viasoft/licensing-management/clients/licensing-management/model/licensedTenantSettingsOutput";
import { VsRadioGroupOption } from "@viasoft/components";
import { ControlLicenseDetailsFieldsRelatedSagaInfo } from '@viasoft/licensing-management/app/tokens/classes/controlLicenseDetailsFieldsRelatedSagaInfo.class';
import { LicensedTenantSagaStatusUpdateNotification } from './notifications/licensed-tenant-saga-status-update-notification';

@Component({
  selector: 'app-license-detail',
  templateUrl: './license-detail.component.html',
  styleUrls: ['./license-detail.component.scss']
})
export class LicenseDetailComponent implements OnInit, OnDestroy {
  private LICENSED_TENANT_SAGA_STATUS_UPDATE_NOTIFICATION_NAME = 'LicensedTenantSagaStatusUpdateNotification';
  private subs = new VsSubscriptionManager();
  private license: LicenseTenantCreateInput;
  private listOfDomains: string[] = [];
  private cnpjToRemove: string;
  private emailToRemove: string;
  private isValidSubject = new BehaviorSubject(this.isValid);
  public controlFieldsRelatedSagaInfo = new ControlLicenseDetailsFieldsRelatedSagaInfo;
  public form: FormGroup;
  public id: string;
  public optionsSelect: VsSelectOption[] = [
    { name: 'licensings.blocked', value: 1 },
    { name: 'licensings.trial', value: 2 },
    { name: 'licensings.active', value: 3 },
    { name: 'licensings.readOnly', value: 4 }
  ];
  hasPoliciesForUpdateDetails = false;
  isEditing = false;
  accessConst = LicenseType.Access;
  connectionConst = LicenseType.Connection;
  isCurrentTabInfrastructureConfiguration = false;
  infrastructureInitialValues = {
    gatewayAddress: null,
    desktopDatabaseName: null
  };
  licensedTenantInitialValues = {};

  public licensedTenantSettingsInitialValues: LicensedTenantSettingsOutput = {
    id: null,
    key: null,
    licensingIdentifier: null,
    value: null
  };

  radios: VsRadioGroupOption[] = [
    {
      value: LicenseConsumeType.Connection,
      title: 'licensings.connectionLicenseConsume',
      type: 'user'
    },
    {
      value: LicenseConsumeType.Access,
      title: 'licensings.acessLicenseConsume',
      type: 'access'
    }
  ];

  public fileProviderForm: FormGroup = new FormGroup({});
  public hasFinishedFileConfigurationRequest: boolean;
  public identifierReady = false;
  public initialized = false;

  public get licenseIdentifier(): string {
    return this.license?.identifier;
  }

  private get isValid() {
    return this.form && this.form.valid &&
    ((this.form.get('status').value == LicensingStatus.Trial && this.form.get('expirationDateTime').value)
    || this.form.get('status').value != LicensingStatus.Trial)
    && (
        (!this.fileProviderForm && this.form.dirty) ||
        (this.fileProviderForm && this.fileProviderForm.valid && (this.fileProviderForm.dirty || this.form.dirty))
      );
  }

  constructor(
    public licenseGridService: LicensingsService,
    private licenses: LicensingsFormControlServices,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private notification: MessageService,
    private dialog: MatDialog,
    private domainsService: DomainsService,
    private accountSelectService: AccountSelectService,
    private dataLivelyService : DataLivelyService,
    private readonly authorizationService: VsAuthorizationService,
    @Optional() @Inject(ENVIRONMENT) private environment: IVsEnvironment,
    private readonly licenseDetailInfrastructureConfigurationService: LicenseDetailInfrastructureConfigurationService,
    private readonly licenseDetailLicensesServerService: LicenseDetailLicensesServerService
  ) {
    this.createForm();
  }

  ngOnInit() {
    this.getAuthorizations().then(result => {
      this.hasPoliciesForUpdateDetails = result;
      this.initComponentAfterGetAuthorizations();
    });
    this.licenseGridService.currentTabSubject.subscribe(v => {
      setTimeout(() => {
        this.isCurrentTabInfrastructureConfiguration = (v === LicensingFormCurrentTab.InfrastructureConfiguration);
      });
    });
    this.subscribeToLicensedTenantStatusNotification();
  }

  private initComponentAfterGetAuthorizations() {
    this.id = this.activatedRoute.snapshot.paramMap.get('id');

    if (this.id) {
      this.updateForm();
    } else {
      this.controlFieldsRelatedSagaInfo.setInitialStatusTooltip();
      this.form.get('status').setValue(LicensingStatus.Blocked);
    }

    this.initialized = true;

    this.subs.add('licesinsingsSubject', this.licenses.licensingsSubject.subscribe(
      () => this.router.navigate(['/licensings/new'])
    ));

    this.subs.add('saveLicensingsSubject', this.licenses.saveLicensingsSubject.subscribe(
      () => this.saveOnClick()
    ));

    this.subs.add('isValidSubject', this.isValidSubject.subscribe(valid => {
      if (valid) {
        this.licenses.licensingsFormIsValid();
      } else {
        this.licenses.licensingsFormIsInvalid();
      }
    }));

    this.subs.add('licensedTenantSettingsValueChanged', this.licenseDetailLicensesServerService.licensedTenantSettingsValueChanged.subscribe(
      (licensedTenantSettingsFormValue: any) => {
        this.form.get('useSimpleHardwareId').setValue(licensedTenantSettingsFormValue.useSimpleHardwareId);

        let hasUseSimpleHardwareIdChangedToANewValue = licensedTenantSettingsFormValue.useSimpleHardwareId
          !== (this.licensedTenantSettingsInitialValues.value.toLowerCase() === 'true');

        if (hasUseSimpleHardwareIdChangedToANewValue) {
          this.form.get('useSimpleHardwareId').markAsDirty();
        } else {
          this.form.get('useSimpleHardwareId').markAsPristine();
        }
      }
    ));
  }

  ngOnDestroy(): void {
    this.subs.clear();
    this.licenses.licensingsFormIsInvalid();
    this.id = null;
    this.licenseGridService.clearTenantId();
    this.licenseGridService.clearLicensedTenantId();
    this.licenseGridService.clearTenantStatus();
  }

  createForm() {
    if (this.form) {
      this.form.reset({
        identifier: this.license.identifier,
        accountId: this.license.accountId,
        accountName: this.license.accountName,
        status: this.license.status,
        licensedCnpjs: this.license.licensedCnpjs,
        administratorEmail: this.license.administratorEmail,
        notes: this.license.notes,
        expirationDateTime: this.license.expirationDateTime,
        licenseConsume: this.license.licenseConsumeType
      });
    } else {
      this.license = {
        id: UUID.UUID(),
        accountId: '',
        accountName: '',
        status: null,
        identifier: this.environment && this.environment.mock ? '131223122311223' : UUID.UUID(),
        expirationDateTime: null,
        numberOfDaysToExpiration: 1,
        licensedCnpjs: '',
        administratorEmail: '',
        notes: '',
        licenseConsumeType: LicenseConsumeType.Connection,
        desktopDatabaseName: null,
        gatewayAddress: null,
        hardwareId: '',
        bundleIds: null,
        numberOfLicenses: 0
      };

      const gatewayPattern = /((([0-9]{1,3}\.){3}[0-9])|([-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,12}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?))(\:\d{2,4}|)/;
      this.form = new FormGroup({
        identifier: new FormControl({ value: this.license.identifier, disabled: true }, [Validators.required]),
        accountId: new FormControl(this.license.accountId, [Validators.required]),
        accountName: new FormControl(this.license.accountName, [Validators.required]),
        status: new FormControl({value: this.license.status, disabled: true }, [Validators.required]),
        licensedCnpjs: new FormControl(this.license.licensedCnpjs, [cpfOrCnpjValidator.cnpjOrCpfWithCommas(), Validators.required]),
        administratorEmail: new FormControl(this.license.administratorEmail, [Validators.email, Validators.required]),
        notes: new FormControl(this.license.notes),
        expirationDateTime: new FormControl(this.license.expirationDateTime, [validateMinDate]),
        licenseConsume: new FormControl(this.license.licenseConsumeType, [Validators.required]),
        gateway: new FormControl(this.license.gatewayAddress, [Validators.pattern(gatewayPattern)]),
        desktopDatabaseName: new FormControl(this.license.desktopDatabaseName),
        hardwareId: new FormControl(this.license.hardwareId),
        useSimpleHardwareId: new FormControl()
      });
    }

    this.subs.add('formValueChanges', this.form.valueChanges.subscribe(() =>
      setTimeout(() => this.isValidSubject.next(this.isValid))
    ));

    this.subs.add('disableStatusField', this.controlFieldsRelatedSagaInfo.disableStatusSubject.subscribe(v =>
      v ? this.form.get('status').disable() : this.form.get('status').enable()
    ));

    this.licensedTenantInitialValues = {
      accountId: this.license.accountId,
      accountName: this.license.accountName,
      status: this.license.status,
      identifier: this.license.identifier,
      expirationDateTime: this.license.expirationDateTime,
      licensedCnpjs: this.license.licensedCnpjs,
      administratorEmail: this.license.administratorEmail,
      notes: this.license.notes,
      licenseConsumeType: LicenseConsumeType.Connection,
      hardwareId: this.license.hardwareId
    };

    this.subs.add('infraestructureConfiguration', this.licenseDetailInfrastructureConfigurationService.infrastructureConfigurationsSubject.subscribe((v) => {
      if (v !== null) {
        const desktopDatabaseNameForm = this.form.get('desktopDatabaseName');
        const gatewayForm = this.form.get('gateway');
        if (gatewayForm.value !== v.gatewayAddress || desktopDatabaseNameForm.value !== v.desktopDatabaseName) {
          gatewayForm.setValue(v.gatewayAddress);
          desktopDatabaseNameForm.setValue(v.desktopDatabaseName);
          if (gatewayForm.value !== this.infrastructureInitialValues.gatewayAddress) {
            gatewayForm.markAsDirty();
          } else {
            gatewayForm.markAsPristine();
          }
          if (desktopDatabaseNameForm.value !== this.infrastructureInitialValues.desktopDatabaseName) {
            desktopDatabaseNameForm.markAsDirty();
          } else {
            desktopDatabaseNameForm.markAsPristine();
          }
        }
      }
    }));

    this.subs.add('gatewayValueChanges', this.form.get('gateway').valueChanges.subscribe(v =>
      this.licenseDetailInfrastructureConfigurationService.updateGatewayInitialValue(v)
    ));

    this.subs.add('desktopDatabaseNameValueChanges', this.form.get('desktopDatabaseName').valueChanges.subscribe(v =>
      this.licenseDetailInfrastructureConfigurationService.updateDesktopDatabaseNameValue(v)
    ));

    if (!this.hasPoliciesForUpdateDetails && this.isEditing) {
      this.setFormDisabled();
    }
  }

  updateForm() {
    this.isEditing = true;
    this.licenseGridService.setLicensedTenantId(this.id);

    this.subs.add('getLicensingById', this.licenseGridService.getById(this.id).subscribe((data) => {
      if (data) {
        this.controlFieldsRelatedSagaInfo.setValuesAccordingLicensedTenantSagaInfo(data.sagaInfo);
        this.license.hardwareId = data.hardwareId;
        this.license.accountId = data.accountId;
        this.license.accountName = data.accountName;
        this.license.administratorEmail = data.administratorEmail;
        this.license.expirationDateTime = data.expirationDateTime;
        this.license.id = data.id;
        this.license.identifier = data.identifier;
        this.license.licenseConsumeType = data.licenseConsumeType;
        this.license.licensedCnpjs = data.licensedCnpjs;
        this.license.notes = data.notes;
        this.license.status = data.status;

        this.licenseGridService.setTenantId(this.license.identifier);
        this.licenseGridService.setLicensedTenantIdentifier(this.license.identifier);
        this.license.licenseConsumeType = data.licenseConsumeDescription === LicenseConsumeType.Connection.toString()
          ? LicenseConsumeType.Connection : LicenseConsumeType.Access;
        if (data.expirationDateTime !== null) {
          this.license.expirationDateTime = new Date(data.expirationDateTime.toString().slice(0, 10));
        }
        this.licenseGridService.setTenantStatus(this.form.get('status').value);

        this.form.get('identifier').setValue(data.identifier);
        this.form.get('accountId').setValue(data.accountId);
        this.form.get('accountName').setValue(data.accountName);
        this.form.get('status').setValue(data.status);
        this.form.get('licensedCnpjs').setValue(data.licensedCnpjs);
        this.form.get('administratorEmail').setValue(data.administratorEmail);
        this.form.get('notes').setValue(data.notes);
        if (data.expirationDateTime) {
          this.form.get('expirationDateTime').setValue(data.expirationDateTime.toString().slice(0, 10));
        }
        this.form.get('licenseConsume').setValue(data.licenseConsumeType);
        this.form.get('hardwareId').setValue(data.hardwareId);
        this.licenseGridService.setDeviceId(data.hardwareId);

        this.identifierReady = true;
        this.licenseDetailInfrastructureConfigurationService.licensingIdentifierReady();
        this.licenseDetailLicensesServerService.licensingIdentifierReady();

        //checks if there is any invalid CPF/CNPJ registered, so the error message is shown
        this.form.updateValueAndValidity();
        if (this.form.get('licensedCnpjs').errors?.cpfAndCnpjWithCommasInvalid) {
          this.form.get('licensedCnpjs').markAsDirty();
          this.form.get('licensedCnpjs').markAsTouched();
        }

      } else {
        this.router.navigateByUrl('/licensings');
      }
    }, () => this.router.navigateByUrl('/licensings')));
  }

  setFormDisabled() {
    this.form.disable();
  }

  subscribeToLicensedTenantStatusNotification() {
    this.subs.add('licensedTenantSagaStatusUpdateNotification', this.dataLivelyService.get<LicensedTenantSagaStatusUpdateNotification>(this.LICENSED_TENANT_SAGA_STATUS_UPDATE_NOTIFICATION_NAME)
    .subscribe((data: LicensedTenantSagaStatusUpdateNotification) => {
      if(this.controlFieldsRelatedSagaInfo.shouldUpdateValues(data)){
        this.controlFieldsRelatedSagaInfo.setValuesAccordingLicensedTenantSagaInfo(data);
        this.form.get('status').setValue(data.currentLicensedTenantStatus);
        this.form.get('administratorEmail').setValue(data.newEmail);
      }
    }));
  }

  private saveOnClick(): void {
    if (!this.id) {
      this.createLicense();
    } else {
      this.updateLicense();
    }
  }

  private createLicense(): void {
    this.license.identifier = this.form.get('identifier').value;
    this.license.accountId = this.form.get('accountId').value;
    this.license.status = this.form.get('status').value;
    this.license.licensedCnpjs = this.form.get('licensedCnpjs').value;
    this.license.administratorEmail = this.form.get('administratorEmail').value;
    this.license.notes = this.form.get('notes').value;
    this.license.licenseConsumeType = this.form.get('licenseConsume').value;
    this.license.hardwareId = this.form.get('hardwareId').value;

    if (this.form.get('expirationDateTime').value !== null) {
      this.license.expirationDateTime = new Date(this.form.get('expirationDateTime').value);
    }

    this.subs.add('licenseGridCreate', this.licenseGridService.create(this.license)
    .subscribe(result => {
      if (result.operationValidationDescription === OperationValidation.DuplicatedIdentifier) {
        this.notification.error(
          'common.error.identifier_already_exists',
          'common.error.could_not_create|name:' + this.license.identifier
        );
      }
      if (result.operationValidationDescription === OperationValidation.AccountIdAlreadyInUse) {
        this.correctFormsAfterAccountIdError();
        this.notification.error(
          'common.error.account_is_being_used|name:' + result.identifier,
          'common.error.invalid_accountId'
        );
      }
      if (result.operationValidationDescription === OperationValidation.AdministrationEmailAlreadyInUse) {
        this.notification.error(
          'common.error.administration_email_is_being_used|name:' + result.identifier,
          'common.error.invalid_administration_email'
        );
      }
      if (result.operationValidationDescription === OperationValidation.InvalidDate) {
        this.notification.error(
          'common.error.expiration_date_should_be_valid|name:' + result.identifier,
          'common.error.invalidDate'
        );
      } else if (result.operationValidationDescription === OperationValidation.NoError) {
        this.router.navigate(['licensings/', result.id]);
      }
    }));
  }

  private updateLicense(): void {
    this.license.identifier = this.form.get('identifier').value;
    this.license.accountId = this.form.get('accountId').value;
    this.license.status = this.form.get('status').value;
    this.license.licensedCnpjs = this.form.get('licensedCnpjs').value;
    this.license.administratorEmail = this.form.get('administratorEmail').value;
    this.license.notes = this.form.get('notes').value;
    this.license.licenseConsumeType = this.form.get('licenseConsume').value;
    this.license.gatewayAddress = this.form.get('gateway').value;
    this.license.desktopDatabaseName = this.form.get('desktopDatabaseName').value;
    this.license.hardwareId = this.form.get('hardwareId').value;

    if (this.form.get('expirationDateTime').value !== null) {
      this.license.expirationDateTime = new Date(this.form.get('expirationDateTime').value);
    } else {
      this.license.expirationDateTime = null;
    }

    let error = false;

    if (this.isDifferentFromInitialLicensedTenantValue()) {
      this.subs.add('licenseGridUpdate', this.licenseGridService.update(this.license)
      .subscribe(result => {
        if (result.operationValidationDescription === OperationValidation.DuplicatedIdentifier) {
          error = true;
          this.notification.error(
            'common.error.identifier_already_exists',
            'common.error.could_not_edit|name:' + this.license.identifier
          );
        }
        if (result.operationValidationDescription === OperationValidation.AccountIdAlreadyInUse) {
          error = true;
          this.correctFormsAfterAccountIdError();
          this.notification.error(
            'common.error.account_is_being_used|name:' + result.identifier,
            'common.error.invalid_accountId'
          );
        }
        if (result.operationValidationDescription === OperationValidation.AdministrationEmailAlreadyInUse) {
          error = true;
          this.notification.error(
            'common.error.administration_email_is_being_used|name:' + result.identifier,
            'common.error.invalid_administration_email'
          );
        }
        if (result.operationValidationDescription === OperationValidation.InvalidDate) {
          this.notification.error(
            'common.error.expiration_date_should_be_valid|name:' + result.identifier,
            'common.error.invalidDate'
          );
        }
        if (result.operationValidationDescription === OperationValidation.NoError) {
          this.licensedTenantInitialValues = {
            accountId: this.form.get('accountId').value,
            accountName: this.form.get('accountName').value,
            status: this.form.get('status').value,
            identifier: this.form.get('identifier').value,
            expirationDateTime: this.form.get('expirationDateTime').value,
            licensedCnpjs: this.form.get('licensedCnpjs').value,
            administratorEmail: this.form.get('administratorEmail').value,
            notes: this.form.get('notes').value,
            licenseConsumeType: this.form.get('licenseConsume').value,
            hardwareId: this.form.get('hardwareId').value
          };
        }
      }));
    }

    if (this.isDifferentFromInitialInfrastructureValues()) {

      this.subs.add('licenseDetailInfrastructureConfigurationUpdate', this.licenseDetailInfrastructureConfigurationService.update({
        licensedTenantId: this.license.id,
        desktopDatabaseName: this.form.get('desktopDatabaseName').value,
        gatewayAddress: this.form.get('gateway').value
      } as InfrastructureConfigurationUpdateInput).subscribe((v) => {
        if (v.success === false) {
          error = true;
          if (v.errors.includes({
            errorCode: OperationValidation.InvalidGateway,
            message: 'InvalidGateway'
          } as OperationValidationBaseCrudResponseError)) {
            this.notification.error(
              'common.error.invalid_gateway|name:' + v.gatewayAddress,
              'common.error.invalid_gateway'
            );
          }
        } else {
          //This is for that the field visible to the user GatewayAddress is updated after the ensureNotTrailingSlash
          this.licenseDetailInfrastructureConfigurationService.updateGatewayAdressValue(v.gatewayAddress);
          this.infrastructureInitialValues.desktopDatabaseName = v.desktopDatabaseName;
          this.infrastructureInitialValues.gatewayAddress = v.gatewayAddress;
        }
      }));
    }

    if (this.isDifferentFromInitialLicensedTenantSettingsValues()) {
      this.subs.add('licenseDetailLicensesServerUpdateLicensingSettings', this.licenseDetailLicensesServerService.updateLicensedTenantSettings(this.licenseIdentifier, this.form.get('useSimpleHardwareId').value)
      .subscribe((licensedTenantSettings: LicensedTenantSettingsOutput) => {
        this.licensedTenantSettingsInitialValues = licensedTenantSettings;
      }));
    }

    if (!error) {
      this.form.markAsPristine();
    }
  }

  openModalProduct(productType: ProductType) {
    if(productType == ProductType.LicensedBundle) {
      const bundleModal = this.dialog.open(LicensesNumberSelectComponent);
      this.subs.add('afterClosedBundleModal', bundleModal.afterClosed().subscribe(bundleModalResult => {
        if (bundleModalResult) {
          this.subs.add('addBundleToLicenseGrid', this.licenseGridService.addBundleToLicense(bundleModalResult).subscribe((value) => {
            if (value.operationValidationDescription === 'AppAlreadyLicensed') {
              this.notification.info(
                'common.error.app_already_licensed'
              );
            }
            this.licenses.refreshGrid();
            this.licenses.nextFilterAppDetail(bundleModalResult.bundleId);
          }));
        }
      }));
    } else {


      this.subs.add('getDomains', this.getDomains().subscribe(() => { }));
      const appsModal = this.dialog.open(LooseAppsNumberSelectComponent, {
        data: {
          listOfDomains: this.listOfDomains,
          hasUpdatePermission: this.hasPoliciesForUpdateDetails,
          isCreate: true
        }
      });

      this.subs.add('afterClosedAppsModal', appsModal.afterClosed().subscribe(appsModalResult => {
        if (appsModalResult) {
          this.subs.add('addAppToLicense', this.licenseGridService.addAppToLicense(appsModalResult).subscribe((addAppToLicenseResult: LicensedAppCreateOutput) => {
            if (addAppToLicenseResult.operationValidationDescription !== String(OperationValidation.NoError)) {
              let errorMessage = `common.error.AddLooseAppToLicenseErrors.${addAppToLicenseResult.operationValidationDescription}`;
              const doesCurrentErrorHasTranslation = this.translateService.instant(errorMessage) !== errorMessage;
              if (!doesCurrentErrorHasTranslation) {
                errorMessage = 'common.error.AddLooseAppToLicenseErrors.Unknown';
              }
              this.notification.info(errorMessage);
            }
            this.licenses.refreshGrid();
          }));
        }
      }));
    }

  }

  public get isCurrentTabProduct(){
    return this.licenseGridService.getCurrentTab() == LicensingFormCurrentTab.Product;
  }

  openModal() {
    if (!this.hasPoliciesForUpdateDetails) {
      return;
    }
    switch (this.licenseGridService.getCurrentTab()) {
      case LicensingFormCurrentTab.FileQuota:
        const fileQuotaModal = this.dialog.open(FileProviderConfigurationModalComponent, {
          data: { licensedTenantId: this.licenseGridService.getLicensedTenantId() }
        });
        this.subs.add('afterClosedFileQuotaModal', fileQuotaModal.afterClosed().subscribe(() =>
          this.licenses.refreshGrid()
        ));
        break;
      case LicensingFormCurrentTab.Organization:
        this.dialog.open(AddOrganizationUnitModalComponent, {
          data: {
            organizationId: this.id,
            licenseIdentifier: this.licenseIdentifier
          }
        })
          .afterClosed()
          .pipe(first())
          .subscribe((result) => {
            if (result) {
              this.licenses.refreshGrid();
            }
          });
        break;
    }
  }

  getDomains() {
    return this.domainsService.getDomains().pipe(tap((value) => {
      for (const key in value) {
        if (key) {
          this.listOfDomains.push(key);
        }
      }
    }));
  }

  onAccountSearch() {
    this.subs.add('accountOpenDialog', this.accountSelectService.openDialog({ onlyActiveAccounts: true }).subscribe(
      (result) => {
        if (result) {
          this.form.get('accountName').setValue(result.companyName);
          this.form.get('accountId').setValue(result.id);
          const cnpjForm = this.form.get('licensedCnpjs');
          const emailForm = this.form.get('administratorEmail');
          this.cnpjToRemove = result.cnpjCpf;
          if (result.email !== emailForm.value) {
            this.emailToRemove = result.email;
          }
          if (!String(cnpjForm.value).includes(result.cnpjCpf) && String(cnpjForm.value).length > 0) {
            cnpjForm.setValue(cnpjForm.value + ',' + result.cnpjCpf);
          } else if (String(cnpjForm.value).length === 0) {
            cnpjForm.setValue(result.cnpjCpf);
          }
          if (emailForm.value === '') {
            emailForm.setValue(result.email);
          }
        }
      }
    ));
  }

  correctFormsAfterAccountIdError() {
    const cnpjForm = this.form.get('licensedCnpjs');
    const emailForm = this.form.get('administratorEmail');
    this.form.get('accountId').reset();
    this.form.get('accountName').reset();
    if (String(cnpjForm.value).includes(',' + this.cnpjToRemove)) {
      const valueToReplace = String(cnpjForm.value).replace(',' + this.cnpjToRemove, '');
      cnpjForm.setValue(valueToReplace);
    } else if (String(cnpjForm.value).includes(this.cnpjToRemove)) {
      const valueToReplace = String(cnpjForm.value).replace(this.cnpjToRemove, '');
      cnpjForm.setValue(valueToReplace);
    }
    if (String(emailForm.value).includes(this.emailToRemove) && this.emailToRemove !== '') {
      emailForm.setValue('');
    }
    if (this.form.valid) {
      this.licenses.licensingsFormIsValid();
    }
  }

  getAuthorizations() {
    return this.authorizationService.isGranted(Policies.UpdateLicense);
  }

  showInfoConsume(type: string) {
    this.dialog.open(LicenseConsumeInfoComponent, {
      data: {
        infoType: type
      },
      maxWidth: '30vw'
    });
  }

  preventDefault(event: MouseEvent) {
    event.preventDefault();
    event.stopImmediatePropagation();
  }

  private isDifferentFromInitialLicensedTenantValue(): boolean {
    const keys = Object.keys(this.licensedTenantInitialValues);
    let output = false;

    keys.forEach(element => {
      if (this.licensedTenantInitialValues[element] !== this.license[element]) {
        output = true;
      }
    });
    return output;
  }

  private isDifferentFromInitialInfrastructureValues(): boolean {
    const keys = Object.keys(this.infrastructureInitialValues);
    let output = false;
    keys.forEach(element => {
      if (this.infrastructureInitialValues[element] !== this.license[element]) {
        output = true;
      }
    });
    return output;
  }

  private isDifferentFromInitialLicensedTenantSettingsValues(): boolean {
    let initialUseSimpleHardwareIdValue;
    if (this.licensedTenantSettingsInitialValues.value === undefined || this.licensedTenantSettingsInitialValues.value === null) {
      return false
    }
    initialUseSimpleHardwareIdValue =  this.licensedTenantSettingsInitialValues.value.toLowerCase() === 'true';
    let useSimpleHardwareIdFormValue = this.form.get('useSimpleHardwareId').value;
    return useSimpleHardwareIdFormValue != undefined && initialUseSimpleHardwareIdValue !== useSimpleHardwareIdFormValue;
  }

  public copyToTheClipboard(text: string): void{
    navigator.clipboard.writeText(text)
  }
}
