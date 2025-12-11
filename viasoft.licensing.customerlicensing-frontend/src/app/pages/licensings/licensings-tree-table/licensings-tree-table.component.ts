import { Component, OnDestroy, OnInit } from '@angular/core';
import { VsTreeTableColumn, VsTreeTableLoadEvent } from '@viasoft/components';
import { interval, Subscription } from 'rxjs';

import { LicensingsService } from '../licensings.service';
import { LicensingsTreeTableService } from './licensings-tree-table.service';
import { GetAllLicenseUsageInRealTimeInput } from '../../../common/inputs/get-all-licenseUsageInRealTime.input';
import { TreeTableNode } from 'src/app/common/interfaces/tree-table-node.interface';
import { CustomFilterService } from './custom-filters.service';
import { LicenseUsageInRealTimeTreeTableTypes } from 'src/client/customer-licensing';

@Component({
  selector: 'app-licensings-tree-table',
  templateUrl: './licensings-tree-table.component.html',
  styleUrls: ['./licensings-tree-table.component.scss']
})
export class LicensingsTreeTableComponent implements OnInit, OnDestroy {

  data: Array<TreeTableNode>;
  dataToShow: Array<TreeTableNode>;
  skipCount: number;
  maxResultCount: number;
  totalRecords: number;
  columns: Array<VsTreeTableColumn>;
  subs: Array<Subscription> = [];
  tenantId: string;
  advancedFilter: string;
  debounce = true;

  constructor(private licensingsTreeTableService: LicensingsTreeTableService,
              private licensingsService: LicensingsService,
              private customFilterService: CustomFilterService) { }

  ngOnInit() {
    this.tenantId = this.licensingsService.getTenantId();
    // Unused outdated code
    this.columns = [
      // {
      //   headerName: 'Nome',
      //   field: 'name'
      // } as VsTreeTableColumn,
      // {
      //   headerName: 'Descrição',
      //   field: 'description'
      // } as VsTreeTableColumn,
      // {
      //   headerName: 'Número de licenças',
      //   size: 2,
      //   children: [
      //     {
      //       headerName: 'Total',
      //       field: 'total',
      //     } as VsTreeTableColumn,
      //     {
      //       headerName: 'Em uso',
      //       field: 'consumed',
      //       disableFilter: true
      //     } as VsTreeTableColumn,
      //     {
      //       headerName: 'Adicionais',
      //       field: 'additional',
      //       disableFilter: true
      //     } as VsTreeTableColumn
      //   ]
      // } as VsTreeTableColumnGroup,
      // {
      //   headerName: 'Horário de acesso',
      //   field: 'startTime',
      //   disableFilter: true
      // } as VsTreeTableColumn
    ];
    this.subs.push(this.licensingsTreeTableService.arrayOfLicensedApps.subscribe(
      (value) => {
        this.data = this.licensingsTreeTableService.createDataArray(value);
        this.totalRecords = 0;
        this.dataToShow = this.data;
      }));
    this.subs.push(interval(60000).subscribe(
      () => {
        this.subs.push(
          this.licensingsService.getLicenseUsageInRealtime(
            {
              licensingIdentifier: this.tenantId,
              advancedFilter: this.advancedFilter
            } as GetAllLicenseUsageInRealTimeInput
          ).subscribe(
            (value) => {
              this.data = this.expandNewNodes(this.licensingsTreeTableService.createDataArray(value.items),
              this.getExpandedNodes(this.data));
              this.dataToShow = this.data;
              this.subs.pop().unsubscribe();
            }
          )
        );
      }
    ));
    this.subs.push(interval(200).subscribe(
      () => {
        this.debounce = false;
      }
    ));
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  getExpandedNodes(input: Array<TreeTableNode>) {
    const output: Array<string> = [];
    if (input !== undefined) {
      input.forEach(element => {
        if (element.expanded === true) {
          output.push(element.data.description + element.data.type);
        }
        if (element.children.length > 0 && element.data.type == LicenseUsageInRealTimeTreeTableTypes.Bundle) {
          element.children.forEach(children => {
            if (children.expanded === true) {
              output.push(children.data.description + children.data.type);
            }
          });
        }
      });
    }
    return output;
  }

  expandNewNodes(input: Array<TreeTableNode>, arrayOfExpandedNodes: Array<string>) {
    input.forEach(element => {
      if (arrayOfExpandedNodes.includes(element.data.description + element.data.type)) {
        element.expanded = true;
      }
      if (element.children.length > 0 && element.data.type == LicenseUsageInRealTimeTreeTableTypes.Bundle) {
        element.children.forEach(children => {
          if (arrayOfExpandedNodes.includes(children.data.description + children.data.type)) {
            children.expanded = true;
          }
        });
      }
    });
    return input;
  }

  loadTreeData(event: VsTreeTableLoadEvent): void {
    if (event.advancedFilter !== null && event.advancedFilter !== undefined) {
      const advancedFilter = JSON.parse(event.advancedFilter);
      this.advancedFilter = this.customFilterService.getAdvancedFilter(advancedFilter);
      this.subs.push(
        this.licensingsService.getLicenseUsageInRealtime({
          licensingIdentifier: this.tenantId,
          advancedFilter: this.advancedFilter
        } as GetAllLicenseUsageInRealTimeInput).subscribe(
          (value) => {
            this.data = this.licensingsTreeTableService.createDataArray(value.items);
            this.dataToShow = this.data;
          }
        )
      );
    }
    if (event.advancedFilter === undefined && this.debounce === false) {
      this.debounce = true;
      this.advancedFilter = null;
      this.subs.push(
        this.licensingsService.getLicenseUsageInRealtime({
          licensingIdentifier: this.tenantId
        } as GetAllLicenseUsageInRealTimeInput).subscribe(
          (value) => {
            this.data = this.licensingsTreeTableService.createDataArray(value.items);
            this.dataToShow = this.data;
          }
        )
      );
    }
    this.skipCount = event.skipCount;
    this.maxResultCount = event.maxResultCount;
    this.dataToShow = this.data !== undefined ? this.data : [];
  }
}
