import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { VsSubscriptionManager } from '@viasoft/common';
import { UUID } from 'angular2-uuid';
import { FileAppQuotaInput, FileTenantQuotaInput } from '@viasoft/licensing-management/clients/licensing-management';

@Component({
  selector: 'app-file-provider-configuration',
  templateUrl: './file-provider-configuration.component.html',
  styleUrls: ['./file-provider-configuration.component.scss']
})
export class FileProviderConfigurationComponent implements OnInit, OnDestroy {
  private internalConfiguration: FileAppQuotaInput | FileTenantQuotaInput;
  private subscriptions = new VsSubscriptionManager();
  public fileProviderForm: FormGroup;

  @Input() public placeholder: string;
  private _disabled: boolean;
  @Input()
  public get disabled(): boolean {
    return this._disabled;
  }
  public set disabled(value: boolean) {
    this._disabled = value;
    this.disabled ? this.fileProviderForm?.get('quotaLimit').disable() : this.fileProviderForm?.get('quotaLimit').enable();
  }
  @Input() public get configuration(): FileAppQuotaInput | FileTenantQuotaInput { return this.internalConfiguration; }
  public set configuration(value: FileAppQuotaInput | FileTenantQuotaInput) {
    this.internalConfiguration = value;
    if (this.internalConfiguration) {
      this.fileProviderForm.get('id').setValue(value.id || UUID.UUID());
      this.fileProviderForm.get('quotaLimit').setValue(this.mbToBytes(value.quotaLimit));
    }
  }
  @Output() public formChange = new EventEmitter<FormGroup>();

  constructor(
    private fb: FormBuilder
  ) {
    this.fileProviderForm = this.fb.group({
      id: [UUID.UUID()],
      quotaLimit: [{value: undefined, disabled: this.disabled}, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.onFormChange();
  }

  ngOnDestroy(): void {
    this.subscriptions.clear();
  }

  private mbToBytes(mb: number | string) {
    if (typeof mb === 'string') {
      mb.replace(/\,/g, '.');
    } else if (mb < 0) {
      return -1;
    }

    let output = String((Number(mb) / 1024 / 1024).toFixed(4));
    // Remove trailing zeros
    while (output && output.includes('.') && (output.endsWith('0') || output.endsWith('.'))) {
      output = output.substr(0, output.length - 1);
    }
    return output;
  }

  private bytesToMb(bytes: number | string) {
    if (typeof bytes === 'string') {
      bytes.replace(/\,/g, '.');
    }
    if (Number(bytes) < 0) {
      return bytes;
    }
    return (Number(bytes) * 1024 * 1024).toFixed(0);
  }

  private onFormChange(): void {
    this.subscriptions.add('form', this.fileProviderForm.valueChanges.subscribe(() => {
      const formToSend = this.fb.group({ id: [], quotaLimit: ['', Validators.required] });
      const quotaLimitValue = String(this.fileProviderForm.get('quotaLimit').value);
      if (quotaLimitValue) {
        const quotaLimitInMb = this.bytesToMb(quotaLimitValue.replace(/[^-?\,\.\d+]/g, ''));
        formToSend.get('quotaLimit').setValue(String(quotaLimitInMb));
      }
      if (!formToSend.get('id').value) {
        formToSend.get('id').setValue(this.fileProviderForm.get('id').value || UUID.UUID());
      }
      formToSend.markAsDirty();
      this.formChange.emit(formToSend);
    }));
  }
}
