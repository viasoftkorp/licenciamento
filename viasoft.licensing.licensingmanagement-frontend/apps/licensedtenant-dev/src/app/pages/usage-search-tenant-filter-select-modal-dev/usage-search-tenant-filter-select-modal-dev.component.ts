import { Component, OnInit, OnDestroy } from '@angular/core';
import { UsageSearchTenantFilterSelectModalService } from 'libs/licensing-licensingmanagement-licensedtenant/src/public-api';
import { LicensedTenantViewOutput } from 'libs/licensing-licensingmanagement-licensedtenant/src/public-api';
import { VsSubscriptionManager } from '@viasoft/common';

@Component({
  selector: 'app-usage-search-tenant-filter-select-modal-dev',
  templateUrl: './usage-search-tenant-filter-select-modal-dev.component.html',
  styleUrls: ['./usage-search-tenant-filter-select-modal-dev.component.scss']
})
export class UsageSearchTenantFilterSelectModalDevComponent implements OnInit, OnDestroy {

  private subs = new VsSubscriptionManager();

  constructor(private readonly modalService: UsageSearchTenantFilterSelectModalService) { }

  ngOnInit() {
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  openModal() {
    this.subs.add('dialog', this.modalService.openDialog().subscribe((output: LicensedTenantViewOutput) => {
      console.log(output);
    }));
  }
}
