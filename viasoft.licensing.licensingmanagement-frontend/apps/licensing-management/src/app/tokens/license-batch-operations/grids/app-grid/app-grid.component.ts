import { Component, OnInit } from '@angular/core';
import {
  VsGridCheckboxColumn,
  VsGridGetInput,
  VsGridGetResult,
  VsGridOptions,
  VsGridSimpleColumn,
} from '@viasoft/components';
import { AppViewService } from '@viasoft/licensing-management/app/tokens/services/app-view.service';
import { DomainsService } from '@viasoft/licensing-management/app/tokens/services/domains.service';
import { AppCreateOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';
import { first, map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-app-grid-batch-operation',
  templateUrl: './app-grid.component.html',
  styleUrls: ['./app-grid.component.scss']
})
export class AppGridBatchOperationsComponent implements OnInit {
  appGrid: VsGridOptions;
  listOfDomains: Array<string> = [];

  constructor(private appViewService: AppViewService, private domainsService: DomainsService) { }

  ngOnInit() {
    this.getDomains().pipe(first()).subscribe(() => { })
    this.configureGrid();
  }

  configureGrid() {
    this.appGrid = new VsGridOptions();
    this.appGrid.enableQuickFilter = false;

    this.appGrid.columns = [
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
        disableFilter: true,
        translate: true,
        format: (value) => {
          if (this.listOfDomains[value] !== undefined) {
            return 'apps.domains.' + this.listOfDomains[value];
          }
        }
      }),
      new VsGridCheckboxColumn({
        headerName: 'apps.active',
        field: 'isActive',
        disabled: true,
        width: 50,
        disableFilter: true
      }),
      new VsGridCheckboxColumn({
        headerName: 'apps.default',
        field: 'isDefault',
        disabled: true,
        width: 50,
        sorting: {
          disable: true
        },
        disableFilter: true
      }),
      new VsGridSimpleColumn({
        headerName: 'apps.software',
        field: 'softwareName',
        width: 100,
        sorting: {
          disable: true
        },
        disableFilter: true
      })
    ];

    this.appGrid.selectionMode = 'multiple';
    this.appGrid.get = (input: VsGridGetInput) => this.get(input);
    this.appGrid.select = (i: number, data: AppCreateOutput) => this.appSelected(data);
    this.appGrid.unselect = (i, data: AppCreateOutput) => this.appUnSelected(data);
    this.appGrid.selectAll = (state: boolean, currentGet: VsGridGetInput) => this.appSelectedAll(state, currentGet);
    this.appGrid.selectionCleared = () => {
      this.appViewService.clearStates();
      this.appViewService.emitCurrentState();
    };
  }

  get(input: VsGridGetInput): Observable<any> {
    return this.appViewService.getAll(
      input
    )
      .pipe(
        map((list) => {
          return new VsGridGetResult(list.items, list.totalCount);
        }
        ),
        tap((list) => {
          this.appViewService.totalCount = list.totalCount;
        })
      );
  }

  appSelected(data: AppCreateOutput) {
    this.appViewService.selected(data.id);
  }

  appUnSelected(data: AppCreateOutput) {
    this.appViewService.unSelected(data.id);
  }

  appSelectedAll(state: boolean, currentGet: VsGridGetInput) {
    this.appViewService.allSelected(state, currentGet);
  }

  getDomains() {
    return this.domainsService.getDomains().pipe(
      tap((value) => {
        for (const key in value) {
          if (key) {
            this.listOfDomains.push(key);
          }
        }
      }));
  }
}
