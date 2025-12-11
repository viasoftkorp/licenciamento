import { Component, OnDestroy, OnInit } from '@angular/core';
import { VsSubscriptionManager } from '@viasoft/common';
import {
  VsGridDateTimeColumn,
  VsGridGetInput,
  VsGridGetResult,
  VsGridOptions,
  VsGridSimpleColumn,
} from '@viasoft/components';
import { interval } from 'rxjs';
import { map, takeWhile } from 'rxjs/operators';

import { LicenseUserBehaviourOutputPagedResultDto } from '../../../../client/customer-licensing';
import { LicensingsGetAll } from '../../../common/inputs/licensings-get-all.input';
import { LicensingsService } from '../licensings.service';
import { LicensingsGridService } from './licensings-grid.service';


@Component({
  selector: 'app-licensings-grid',
  templateUrl: './licensings-grid.component.html',
  styleUrls: ['./licensings-grid.component.scss']
})
export class LicensingsGridComponent implements OnInit, OnDestroy {
  grid: VsGridOptions;
  curentTime: Date;
  gadgetHasOperationControls = false;
  isLoading = true;
  timer = interval(60000);
  hasFinishedTimer = false;
  subs = new VsSubscriptionManager();
  constructor(private readonly userBehaviourGridService: LicensingsGridService,
              readonly licensingsService: LicensingsService) {
  }

  ngOnInit() {
    this.configureGrid();

    const refreshGridEveryMinute = this.timer.pipe(
      takeWhile(() => !this.hasFinishedTimer)
    );

    this.subs.add('timer', refreshGridEveryMinute.subscribe(
      () => this.grid.refresh()
    ));
  }

  ngOnDestroy(): void {
    this.hasFinishedTimer = true;
    this.subs.clear();
  }

  configureGrid() {
    this.grid = new VsGridOptions();

    this.grid.columns = [
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.TenantId',
        field: 'licensingIdentifier',
        disableFilter: true,
        width: 280
      }),
      new VsGridDateTimeColumn({
        headerName: 'Licensings.Grid.LastUpdate',
        field: 'lastUpdate',
        disableFilter: true,
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.DatabaseName',
        field: 'databaseName',
        width: 260
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.User',
        field: 'user',
        width: 150
      }),
      new VsGridDateTimeColumn({
        headerName: 'Licensings.Grid.StartTime',
        field: 'startTime',
        disableFilter: true,
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.AcessDuration',
        field: 'accessDurationFormatted',
        disableFilter: true,
        sorting: {
          disable: true
        },
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.AppIdentifier',
        field: 'appIdentifier',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.AppName',
        field: 'appName',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.BundleIdentifier',
        field: 'bundleIdentifier',
        width: 200
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.BundleName',
        field: 'bundleName',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.SoftwareIdentifier',
        field: 'softwareIdentifier',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.SoftwareName',
        field: 'softwareName',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.SoftwareVersion',
        field: 'softwareVersion',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.HostName',
        field: 'hostName',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.HostUser',
        field: 'hostUser',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.LocalIPAddress',
        field: 'localIpAddress',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.Language',
        field: 'language',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.OSInfo',
        field: 'osInfo',
        width: 200
      }),
      new VsGridSimpleColumn({
        headerName: 'Licensings.Grid.BrowserInfo',
        field: 'browserInfo',
        width: 180
      })
    ];
    this.grid.sizeColumnsToFit = false;
    this.grid.enableFilter = true;
    this.grid.get = (input: VsGridGetInput) => this.get(input);
    this.grid.id = '788BDD2D-AAAA-4BB4-A382-796104690114';
  }

  get(input: LicensingsGetAll) {
    input.licensingIdentifier = this.licensingsService.getTenantId();
    if (input.advancedFilter && input.advancedFilter !== '') {
      this.licensingsService.notifyAddSubject(input);
    } else {
      this.licensingsService.notifyClearSubject();
    }
    return this.userBehaviourGridService.getUsersBehaviour(
      input
    ).pipe(
      map(
        (list: LicenseUserBehaviourOutputPagedResultDto) => new VsGridGetResult(list.items, list.totalCount)
      )
    );
  }
}
