import { Component, OnInit, Injector, OnDestroy, inject, signal } from '@angular/core';
import { GadgetBase } from '@viasoft/dashboard';
import { switchMap, map, takeWhile } from 'rxjs/operators';
import { VsSubscriptionManager } from '@viasoft/common';
import { interval, Observable } from 'rxjs';
import { LicenseUsageStatisticsServiceProxy } from 'src/client/customer-licensing';
import { LicensingsService } from 'src/app/pages/licensings/licensings.service';

@Component({
  selector: 'vs-dynamic-component',
  templateUrl: './licensings-online-users-gadget.component.html',
  styleUrls: ['./licensings-online-users-gadget.component.scss'],
})
export class LicensingsOnlineUsersGadgetComponent extends GadgetBase implements OnInit, OnDestroy {
  onlineUsers = signal<number>(0);
  description = 'Licensings.DashBoard.OnlineUsers.OnlineUsersTitle';
  subs = new VsSubscriptionManager();
  timer = interval(60000);
  hasFinishedTimer = false;
  advancedFilter = signal<string>(null);

  private readonly serviceProxy = inject(LicenseUsageStatisticsServiceProxy);
  private readonly licensingsService = inject(LicensingsService);

  constructor(
    injector: Injector
  ) {
    super(injector);
  }

  ngOnInit() {
    this.initUserCount();
    this.listenToAdvancedFilterChanges();
    this.startAutoRefresh();
  }

  private initUserCount(): void {
    this.subs.add(
      'countUsers',
      this.fetchUserCount()
        .subscribe(count => this.onlineUsers.set(count))
    );
  }

  private listenToAdvancedFilterChanges(): void {
    this.subs.add(
      'advancedFilterChanged',
      this.licensingsService.advancedFilterSubject.pipe(
        switchMap(advancedFilter => {
          this.advancedFilter.set(advancedFilter);
          return this.fetchUserCount();
        })
      ).subscribe(count => this.onlineUsers.set(count))
    );
  }

  private startAutoRefresh(): void {
    const updateGadgetEveryMinute = this.timer.pipe(
      takeWhile(() => !this.hasFinishedTimer),
      switchMap(() => this.fetchUserCount())
    );

    this.subs['interval'] = updateGadgetEveryMinute
      .subscribe(count => this.onlineUsers.set(count));
  }

  private fetchUserCount(): Observable<number> {
    return this.serviceProxy
      .getOnlineUserCount(this.licensingsService.getTenantId(), this.advancedFilter())
      .pipe(map(result => result.onlineUserCount));
  }

  ngOnDestroy() {
    this.hasFinishedTimer = true;
    this.subs.clear();
  }

  run(): void {
    this.initializeRunState(true);
    this.updateData(null);
  }
  stop(): void {
    this.setStopState(false);
  }
  updateProperties(updatedProperties: any): void {}

  updateData(data: any[]): void {}

  preRun(): void {
    this.run();
  }
}
