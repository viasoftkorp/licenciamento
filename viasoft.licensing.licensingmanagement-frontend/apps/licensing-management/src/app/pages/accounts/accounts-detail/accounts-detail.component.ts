import * as imask from 'imask';
import {
  AccountCreateInput,
  AccountUpdateInput,
  GetCompanyByCnpjOutput,
  OperationValidation,
  ZipCodeAdressDto
  } from '@viasoft/licensing-management/clients/licensing-management';
import { AccountsCnpjValidationService } from '../accounts-cnpj-validation.service';
import { AccountsFormControlService } from '../accounts-form-control.service';
import { AccountsService } from '../accounts.service';
import { AccountsZipCodeValidationService } from '../accounts-zip-code-validation.service';
import {
  Component,
  Inject,
  OnDestroy,
  OnInit
  } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { cpfOrCnpjValidator } from '@viasoft/licensing-management/app/tokens/validators/cnpjOrCpf.validator';
import { interval } from 'rxjs';
import { MaskResolverService, SimplifiedIMask } from '@viasoft/components/shared';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageService, VsSubscriptionManager, VsValidators } from '@viasoft/common';
import { TranslateService } from '@ngx-translate/core';
import { UUID } from 'angular2-uuid';
import { VsSelectOption } from '@viasoft/components/select';

@Component({
  selector: 'app-accounts-detail',
  templateUrl: './accounts-detail.component.html',
  styleUrls: ['./accounts-detail.component.scss']
})
export class AccountsDetailComponent implements OnInit, OnDestroy {
  private subs = new VsSubscriptionManager();
  public generalInfoForm: FormGroup;
  public adressInfoForm: FormGroup;
  public billingInfoForm: FormGroup;
  account: any;
  optionsSelect: VsSelectOption[] = [
    { name: 'accounts.details.accountStatus.blocked', value: 0 },
    { name: 'accounts.details.accountStatus.active', value: 1 }
  ];
  id: string;
  debounce: boolean;
  cnpjCpfErrorMessage = 'common.error.field_is_required';
  lastCnpjSearched: string;
  lastZipCodeSearched: string;
  notShowSaveButton = false;
  public phoneErrorMessage: string;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly maskResolver: MaskResolverService,
    private readonly accountsService: AccountsService,
    private readonly notification: MessageService,
    private readonly accountsFormControlService: AccountsFormControlService,
    private readonly accountsCnpjValidationService: AccountsCnpjValidationService,
    private readonly accountsZipCodeValidationService: AccountsZipCodeValidationService,
    private readonly translateService: TranslateService,
    @Inject(MAT_DIALOG_DATA) private readonly data: any
  ) { }

  ngOnInit(): void {
    this.account = {
      id: UUID.UUID(),
      status: null,
      billingEmail: '',
      city: '',
      cnpjCpf: '',
      companyName: '',
      detail: '',
      email: '',
      neighborhood: '',
      number: '',
      phone: '',
      state: '',
      street: '',
      tradingName: '',
      webSite: '',
      zipCode: ''
    } as AccountCreateInput;

    if (this.data) {
      this.id = this.data.id;
    }

    if (this.id) {
      this.subs.add('getAccountById', this.accountsService.getById(this.id).subscribe(
        (result) => {
          if (result) {
            this.account = result as AccountUpdateInput;
            this.createOrResetForm();
          }
        }
      ));
    }
    this.createOrResetForm();

    this.subs.add('debounceInterval', interval(1000).subscribe(() => {
      this.debounce = false;
    }));
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  createOrResetForm(): void {
    if (this.generalInfoForm) {
      this.generalInfoForm.reset(
        {
          companyName: this.account.companyName,
          tradingName: this.account.tradingName,
          cnpjCpf: this.account.cnpjCpf,
          email: this.account.email,
          phone: this.account.phone,
          status: this.account.status,
          webSite: this.account.webSite,
        }
      );
    } else {
      this.generalInfoForm = this.formBuilder.group({
        companyName: this.formBuilder.control(this.account.companyName, [Validators.required]),
        tradingName: this.formBuilder.control(this.account.tradingName, [Validators.required]),
        cnpjCpf: this.formBuilder.control(this.account.cnpjCpf, [Validators.required, cpfOrCnpjValidator.cnpjOrCpf()]),
        email: this.formBuilder.control(this.account.email, [Validators.email]),
        phone: this.formBuilder.control(this.account.phone, [Validators.pattern(/^\(?([0-9]{2})?\)? ?([0-9]{4,5})-?([0-9]{4})$/)]),
        status: this.formBuilder.control(this.account.status, [Validators.required]),
        webSite: this.formBuilder.control(this.account.webSite)
      });
    }
    if (this.adressInfoForm) {
      this.adressInfoForm.reset({
        zipCode: this.account.zipCode,
        state: this.account.state,
        city: this.account.city,
        neighborhood: this.account.neighborhood,
        street: this.account.street,
        detail: this.account.detail,
        number: this.account.number
      });
    } else {
      this.adressInfoForm = new FormGroup({
        zipCode: this.formBuilder.control(this.account.zipCode, [Validators.pattern('[0-9]*')]),
        state: this.formBuilder.control(this.account.state),
        city: this.formBuilder.control(this.account.city),
        neighborhood: this.formBuilder.control(this.account.neighborhood),
        street: this.formBuilder.control(this.account.street),
        detail: this.formBuilder.control(this.account.detail),
        number: this.formBuilder.control(this.account.number, [Validators.pattern('[0-9]*')])
      });
    }
    if (this.adressInfoForm) {
      this.adressInfoForm.reset({
        zipCode: this.account.zipCode,
        state: this.account.state,
        city: this.account.city,
        neighborhood: this.account.neighborhood,
        street: this.account.street,
        detail: this.account.detail,
        number: this.account.number
      });
    } else {
      this.adressInfoForm = new FormGroup({
        zipCode: this.formBuilder.control(this.account.zipCode, [Validators.pattern('[0-9]*')]),
        state: this.formBuilder.control(this.account.state),
        city: this.formBuilder.control(this.account.city),
        neighborhood: this.formBuilder.control(this.account.neighborhood),
        street: this.formBuilder.control(this.account.street),
        detail: this.formBuilder.control(this.account.detail),
        number: this.formBuilder.control(this.account.number, [Validators.pattern('[0-9]*')])
      });
    }
    if (this.billingInfoForm) {
      this.billingInfoForm.reset({
        billingEmail: this.account.billingEmail
      });
    } else {
      this.billingInfoForm = new FormGroup({
        billingEmail: this.formBuilder.control(this.account.billingEmail, [Validators.email])
      });
    }

    this.subs.add('cnpjCpfValueChanges', this.generalInfoForm.get('cnpjCpf').valueChanges.subscribe(
      (value) => {
        if (this.generalInfoForm.get('cnpjCpf').hasError('pattern')) {
          this.cnpjCpfErrorMessage = 'common.error.only_numbers';
        } else if (value === '') {
          this.cnpjCpfErrorMessage = 'common.error.field_is_required';
        }
      }
    ));
    if (this.data && !this.data.hasUpdatePermission) {
      this.setFormDisabled();
    }
  }

  setFormDisabled() {
    this.generalInfoForm.disable();
    this.adressInfoForm.disable();
    this.billingInfoForm.disable();
    this.notShowSaveButton = true;
  }

  get validToSave() {
    return !!(this.generalInfoForm &&
      this.adressInfoForm &&
      this.billingInfoForm &&
      this.billingInfoForm.valid &&
      this.adressInfoForm.valid &&
      this.generalInfoForm.valid &&
      (this.billingInfoForm.dirty || this.adressInfoForm.dirty || this.generalInfoForm.dirty));
  }

  save() {
    this.setValues();
    if (!this.id) {
      this.subs.add('createAccount', this.accountsService.create(this.account).subscribe(
        (result) => {
          if (result) {
            if (result.operationValidationDescription === OperationValidation.CnpjAlreadyRegistered) {
              this.notification.error(
                'common.error.cnpj_already_registered|name:' + result.companyName,
                'common.error.cnpj_already_in_use'
              );
            }
            this.id = this.account.id;
            this.accountsFormControlService.refreshGrid();
            this.adressInfoForm.markAsPristine();
            this.generalInfoForm.markAsPristine();
            this.billingInfoForm.markAsPristine();
          }
        }
      ));
    } else {
      this.subs.add('updateAccount', this.accountsService.update(this.account).subscribe((result) => {
        if (result.operationValidationDescription === OperationValidation.CnpjAlreadyRegistered) {
          this.notification.error(
            'common.error.cnpj_already_registered|name:' + result.companyName,
            'common.error.cnpj_already_in_use'
          );
        }
        this.accountsFormControlService.refreshGrid();
        this.adressInfoForm.markAsPristine();
        this.generalInfoForm.markAsPristine();
        this.billingInfoForm.markAsPristine();
      }));
    }
  }

  setValues() {
    this.account.billingEmail = this.billingInfoForm.get('billingEmail').value;
    this.account.city = this.adressInfoForm.get('city').value;
    this.account.cnpjCpf = this.generalInfoForm.get('cnpjCpf').value;
    this.account.companyName = this.generalInfoForm.get('companyName').value;
    this.account.detail = this.adressInfoForm.get('detail').value;
    this.account.email = this.generalInfoForm.get('email').value;
    this.account.neighborhood = this.adressInfoForm.get('neighborhood').value;
    this.account.number = this.adressInfoForm.get('number').value;
    if (this.generalInfoForm.get('phone').value != null) {
      this.account.phone = this.generalInfoForm.get('phone').value
        .replace('(', '')
        .replace(')', '')
        .replace('-', '')
        .replace(' ', '');
    }
    this.account.state = this.adressInfoForm.get('state').value;
    this.account.street = this.adressInfoForm.get('street').value;
    this.account.tradingName = this.generalInfoForm.get('tradingName').value;
    this.account.webSite = this.generalInfoForm.get('webSite').value;
    this.account.zipCode = this.adressInfoForm.get('zipCode').value;
    this.account.status = this.generalInfoForm.get('status').value;
  }

  searchCompany(event: string) {
    if (this.debounce === false && event.length > 0
      && !this.generalInfoForm.get('cnpjCpf').hasError('pattern')
      && this.lastCnpjSearched !== event
      && this.generalInfoForm.get('cnpjCpf').value.length !== 11) {
      this.debounce = true;
      this.subs.add('getCompanyByCnpj', this.accountsCnpjValidationService.GetCompanyByCnpj(event, this.account.id).subscribe((value) => {
        this.setFormValuesByCnpj(value, event);
      }));
    }
  }

  searchByFormValueCnpj() {
    if (this.debounce === false) {
      const cnpjCpf = this.generalInfoForm.get('cnpjCpf');
      if (cnpjCpf.value.length > 0
        && !cnpjCpf.hasError('pattern')
        && this.lastCnpjSearched !== cnpjCpf.value) {
        this.debounce = true;
        this.subs.add('getCompanyByCnpjByFormValue', this.accountsCnpjValidationService.GetCompanyByCnpj(cnpjCpf.value, this.account.id)
          .subscribe((value) => {
            this.setFormValuesByCnpj(value, cnpjCpf.value);
          }));
      }
    }
  }

  searchByFormValueZipCode() {
    if (this.debounce === false) {
      const zipCode = this.adressInfoForm.get('zipCode');
      if (zipCode.value.length > 0 && this.lastZipCodeSearched !== zipCode.value && !zipCode.hasError('pattern')) {
        this.debounce = true;
        this.subs.add('getAddressByZipcodeByFormValue', this.accountsZipCodeValidationService.GetAddressByZipcode(zipCode.value)
          .subscribe((value) => {
            this.setFormValuesByZipCode(value, zipCode.value);
          }));
      }
    }
  }

  setFormValuesByCnpj(value: GetCompanyByCnpjOutput, lastValueSearched: string) {
    if (!value) {
      return;
    }
    if (value.operationValidationDescription === OperationValidation.CnpjAlreadyRegistered
      && value.id !== this.account.id) {
      this.cnpjCpfErrorMessage = 'common.error.cnpj_already_in_use';
      this.generalInfoForm.controls.cnpjCpf.setErrors({});
      this.lastCnpjSearched = lastValueSearched;
      this.notification.error(
        'common.error.cnpj_already_registered|name:' + value.name,
        'common.error.cnpj_already_in_use'
      );
    } else if (value.operationValidationDescription === OperationValidation.NoError) {
      const fieldsToUpdate = [
        { form: this.generalInfoForm, fieldName: 'email', value: value.email },
        { form: this.generalInfoForm, fieldName: 'tradingName', value: value.tradingName },
        { form: this.generalInfoForm, fieldName: 'companyName', value: value.name },
        { form: this.adressInfoForm, fieldName: 'state', value: value.uf },
        { form: this.adressInfoForm, fieldName: 'city', value: value.city },
        { form: this.adressInfoForm, fieldName: 'neighborhood', value: value.neighborhood },
        { form: this.adressInfoForm, fieldName: 'street', value: value.street },
        { form: this.adressInfoForm, fieldName: 'detail', value: value.complement },
        { form: this.adressInfoForm, fieldName: 'number', value: value.number },
      ];
      this.adressInfoForm.markAsDirty();
      for (const fieldToUpdate of fieldsToUpdate) {
        fieldToUpdate.form.get(fieldToUpdate.fieldName).setValue(fieldToUpdate.value);
        fieldToUpdate.form.get(fieldToUpdate.fieldName).markAsDirty();
        fieldToUpdate.form.get(fieldToUpdate.fieldName).markAsTouched();
      }
      if (value.phone) {
        // Phone might be invalid, so we need to check it before filling the form
        const phoneMask = imask.createMask(this.maskResolver.getMask('phone'));
        phoneMask.resolve(value.phone);
        const phoneValueToUpdate = Number(String(value.phone).replace(/\D/g, ""));
        const resolvedMaskValue = Number(String(phoneMask.value).replace(/\D/g, ""));
        const isPhoneInvalid = phoneValueToUpdate !== resolvedMaskValue;
        this.phoneErrorMessage = isPhoneInvalid ? this.translateService.instant('common.error.invalid_phone_value', { phoneNumber: value.phone }) : undefined;
        this.generalInfoForm.get('phone').setValue(isPhoneInvalid ? '' : value.phone, { emitEvent: !isPhoneInvalid });
        this.generalInfoForm.get('phone').markAsDirty();
        this.generalInfoForm.get('phone').markAsTouched();
      } else {
        this.generalInfoForm.get('phone').setValue('');
      }

      if (value.status === 'ERROR') {
        this.notification.info('common.error.could_not_find_cnpj');
      }
    }
  }

  searchZipCode(event: string) {
    if (this.debounce === false && event.length > 0 && this.lastZipCodeSearched !== event
      && !this.adressInfoForm.get('zipCode').hasError('pattern')) {
      this.debounce = true;
      this.subs.add('getAddressByZipcode',
        this.accountsZipCodeValidationService.GetAddressByZipcode(event).subscribe(
          (value) => {
            this.setFormValuesByZipCode(value, event);
          }
        )
      );
    }
  }

  setFormValuesByZipCode(value: ZipCodeAdressDto, lastZipCodeSearched: string) {
    if (value) {
      if (value.status === 'ERROR') {
        this.lastZipCodeSearched = lastZipCodeSearched;
        this.notification.info('common.error.could_not_find_adress');
      } else {
        this.adressInfoForm.markAsDirty();
        this.adressInfoForm.get('state').setValue(value.state);
        this.adressInfoForm.get('city').setValue(value.city);
        this.adressInfoForm.get('neighborhood').setValue(value.neighborhood);
        this.adressInfoForm.get('street').setValue(value.street);
      }
    }
  }

}
