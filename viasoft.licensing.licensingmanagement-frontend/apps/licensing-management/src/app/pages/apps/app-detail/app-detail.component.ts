import { Component, OnDestroy, OnInit, Inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UUID } from 'angular2-uuid';
import { Subscription, of, Observable } from 'rxjs';
import { SoftwareSelectService } from '@viasoft/licensing-management/app/tokens/services/modals-service/software-select.service';
import { AppCreateInput, AppUpdateInput, SoftwareCreateOutput } from '@viasoft/licensing-management/clients/licensing-management';

import { SoftwaresService } from '../../softwares/softwares.service';
import { AppsFormControlService } from '../apps-form-control.service';
import { AppsService } from '../apps.service';
import { MessageService } from '@viasoft/common';
import { VsAutocompleteGetInput, VsAutocompleteGetNameFn, VsAutocompleteOption, VsAutocompleteOutput } from '@viasoft/components';
import { DomainsService } from '../../../tokens/services/domains.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { map, tap } from 'rxjs/operators';
import { pt } from '@viasoft/licensing-management/app/i18n/pt';
import { en } from '@viasoft/licensing-management/app/i18n/en';

@Component({
  selector: 'app-app-detail',
  templateUrl: './app-detail.component.html',
  styleUrls: ['./app-detail.component.scss']
})
export class AppDetailComponent implements OnInit, OnDestroy {
  id: string;
  form: FormGroup;
  app: any;
  activeSoftwaresOnly: true;
  subs: Array<Subscription> = [];
  domains: VsAutocompleteOutput<string> = {
    items: [],
    totalCount: 0
  };
  notShowSaveButton = false;

  constructor(
    private appsService: AppsFormControlService,
    private appGridService: AppsService,
    private notification: MessageService,
    private softwares: SoftwaresService,
    private softwareSelectService: SoftwareSelectService,
    private domainsService: DomainsService,
    @Inject(MAT_DIALOG_DATA)
    private data: any
  ) { }

  ngOnInit() {
    this.app = {
      id: UUID.UUID(),
      name: '',
      identifier: '',
      isActive: true,
      softwareId: '',
      isDefault: false,
      domain: null
    } as AppCreateInput;

    if (this.data) {
      this.id = this.data.id;
    }

    this.subs.push(this.prepareDomainsData().subscribe(() => {
      if (this.id) {
        this.subs.push(this.appGridService.getById(this.id).subscribe(data => {
          if (data) {
            this.app = data as AppUpdateInput;
            this.createForm();
            this.subs.push(this.softwares.getById(data.softwareId)
              .subscribe(software => {
                this.form.get('software').setValue(software.name);
              }));
          }
        }));
      } else {
        this.createForm();
      }
    }));
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  createForm() {
    if (this.form) {
      this.form.reset({
        name: this.app.name,
        identifier: this.app.identifier,
        isActive: this.app.isActive,
        isDefault: this.app.isDefault,
        softwareId: this.app.softwareId,
        software: '',
        domain: this.domains.items[this.app.domain]?.value ?? ''
      });
    } else {
      this.form = new FormGroup({
        name: new FormControl(this.app.name, [Validators.required]),
        identifier: new FormControl(this.app.identifier, [Validators.required]),
        isActive: new FormControl(this.app.isActive),
        isDefault: new FormControl(this.app.isDefault),
        softwareId: new FormControl(this.app.softwareId, [Validators.required]),
        software: new FormControl('', [Validators.required]),
        domain: new FormControl(this.domains.items[this.app.domain]?.value ?? '', [Validators.required])
      });
    }
    this.subs.push(this.form.valueChanges.subscribe(() => {
      const domainForm = this.form.get('domain');
      const domainValue = this.domains.items[this.app.domain]?.value;
      if (domainValue && domainForm.value === domainValue) {
        domainForm.markAsPristine();
      }
    }));
    if (this.data && !this.data.hasUpdatePermission) {
      this.setFormDisabled();
    }
  }

  setFormDisabled() {
    this.form.disable();
    this.notShowSaveButton = true;
  }

  get saveIsEnable() {
    return !!(this.form && this.form.valid && this.form.dirty);
  }

  save() {
    this.saveOnClick();
  }

  saveOnClick() {
    if (!this.id) {
      this.app.name = this.form.get('name').value;
      this.app.identifier = this.form.get('identifier').value;
      this.app.isActive = this.form.get('isActive').value;
      this.app.softwareId = this.form.get('softwareId').value;
      this.app.isDefault = this.form.get('isDefault').value;
      this.app.domain = this.form.get('domain').value;

      this.subs.push(this.appGridService.create(this.app)
        .subscribe(result => {
          if (result.operationValidationDescription === 'DuplicatedIdentifier') {
            this.notification.error(
              'common.error.identifier_already_exists',
              'common.error.could_not_create|name:' + this.app.name
            );
          }
          this.appsService.refreshGrid();
          this.id = this.app.id;
          this.form.markAsPristine();
        }));
    } else {
      this.app.name = this.form.get('name').value;
      this.app.identifier = this.form.get('identifier').value;
      this.app.isActive = this.form.get('isActive').value;
      this.app.softwareId = this.form.get('softwareId').value;
      this.app.isDefault = this.form.get('isDefault').value;
      this.app.domain = this.form.get('domain').value;

      this.subs.push(this.appGridService.update(this.app).subscribe(result => {
        if (result.operationValidationDescription === 'DuplicatedIdentifier') {
          this.notification.error(
            'common.error.identifier_already_exists',
            'common.error.could_not_edit|name:' + this.app.name
          );
        } else {
          this.form.markAsPristine();
          this.appsService.refreshGrid();
        }
      }));
    }
  }

  onSoftwareSearch() {
    this.subs.push(this.softwareSelectService.openDialog({ onlyActiveSoftwares: true }).subscribe((software: SoftwareCreateOutput) => {
      if (!software) {
        return;
      }
      this.form.get('software').setValue(software.name);
      this.form.get('softwareId').setValue(software.id);
    }
    ));
  }

  public autocompleteGet = (input: VsAutocompleteGetInput): Observable<VsAutocompleteOutput<string>> => {
    return this.prepareDomainsData().pipe(
      map(() => {
        let filteredDomains = [...this.domains.items];
        if (input?.valueToFilter) {
          filteredDomains = filteredDomains.filter(d => d.name.toLowerCase().includes(input.valueToFilter.toLowerCase()));
        }
        return {
          items: filteredDomains,
          totalCount: filteredDomains.length
        };
      })
    );
  }

  public getName: VsAutocompleteGetNameFn<string> = (value, options, controlName) => {
    const currentOption = this.domains?.items.find((o) => o.value === value);
    return of(currentOption?.name);
  };

  prepareDomainsData(): Observable<any> {
    return this.domainsService.getDomains().pipe(
      tap((result) => {
        const items = Object.keys(result).map((key) => ({
          name: navigator.language === 'pt-BR' ? pt.apps.domains[key] : en.apps.domains[key],
          value: result[key]
        })) as VsAutocompleteOption<string>[];

        this.domains = {
          totalCount: items.length,
          items
        };
      })
    );
  }
}
