import { Component, OnInit } from '@angular/core';
import {
  VsGridCheckboxColumn,
  VsGridGetInput,
  VsGridGetResult,
  VsGridOptions,
  VsGridSimpleColumn,
} from '@viasoft/components';
import { BundleViewService } from '@viasoft/licensing-management/app/tokens/services/bundle-view.service';
import { BundleCreateOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-bundle-grid-batch-operation',
  templateUrl: './bundle-grid.component.html',
  styleUrls: ['./bundle-grid.component.scss']
})
export class BundleGridBatchOperationsComponent implements OnInit {

  bundleGrid: VsGridOptions;

  constructor(private bundleViewService: BundleViewService) { }

  ngOnInit() {
    this.configureGrid();
  }

  configureGrid() {
    this.bundleGrid = new VsGridOptions();
    this.bundleGrid.enableQuickFilter = false;

    this.bundleGrid.columns = [
      new VsGridSimpleColumn({
        headerName: 'products.name',
        field: 'name',
        width: 100,
      }),
      new VsGridSimpleColumn({
        headerName: 'products.identifier',
        field: 'identifier',
        width: 100,
      }),
      new VsGridSimpleColumn({
        headerName: 'products.software',
        field: 'softwareName',
        width: 100,
        sorting: {
          disable: true
        },
        disableFilter: true
      }),
      new VsGridCheckboxColumn({
        headerName: 'products.custom',
        field: 'isCustom',
        disabled: true,
        disableFilter: true,
        width: 100,
      }),
      new VsGridCheckboxColumn({
        headerName: 'products.active',
        field: 'isActive',
        disabled: true,
        disableFilter: true,
        width: 50,
      })
    ];

    this.bundleGrid.selectionMode = 'multiple';
    this.bundleGrid.get = (input: VsGridGetInput) => this.get(input);
    this.bundleGrid.select = (i: number, data: BundleCreateOutput) => this.bundleSelected(data);
    this.bundleGrid.unselect = (i, data: BundleCreateOutput) => this.bundleUnSelected(data);
    this.bundleGrid.selectAll = (state: boolean, currentGet: VsGridGetInput) => this.bundleSelectedAll(state, currentGet);
    this.bundleGrid.selectionCleared = () => {
      this.bundleViewService.clearStates();
      this.bundleViewService.emitCurrentState();
    };
  }

  get(input: VsGridGetInput): Observable<any> {
    return this.bundleViewService.getAll(
      input
    )
      .pipe(
        map((list: any) => {
          return new VsGridGetResult(list.items, list.totalCount);
        }
        ),
        tap((list) => {
          this.bundleViewService.totalCount = list.totalCount;
        })
      );
  }

  bundleSelected(data: BundleCreateOutput) {
    this.bundleViewService.selected(data.id);
  }

  bundleUnSelected(data: BundleCreateOutput) {
    this.bundleViewService.unSelected(data.id);
  }

  bundleSelectedAll(state: boolean, currentGet: VsGridGetInput) {
    this.bundleViewService.allSelected(state, currentGet);
  }

}
