import { Component, OnInit, OnDestroy, Injector, inject, signal } from '@angular/core';
import { GadgetBase } from '@viasoft/dashboard';
import { switchMap, tap } from 'rxjs/operators';
import { VsSubscriptionManager } from '@viasoft/common';
import { LicenseUsageStatisticsServiceProxy, OnlineAppsCountOutput } from 'src/client/customer-licensing';
import { LicensingsService } from 'src/app/pages/licensings/licensings.service';

@Component({
  selector: 'vs-dynamic-component',
  templateUrl: './licensings-online-apps-gadget.component.html',
  styleUrls: ['./licensings-online-apps-gadget.component.scss']
})
export class LicensingsOnlineAppsGadgetComponent extends GadgetBase implements OnInit, OnDestroy {

  onlineApps = signal<string>('0/0');
  description = 'Licensings.DashBoard.OnlineApps.OnlineAppsTitle';
  advancedFilter = signal<string>(null);

  private subs = new VsSubscriptionManager();

  private readonly serviceProxy = inject(LicenseUsageStatisticsServiceProxy);
  private readonly licensingsService = inject(LicensingsService);

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    this.subs.add('countApps', this.serviceProxy.getOnlineAppsCount(this.licensingsService.getTenantId(), this.advancedFilter())
    .subscribe(
      value => this.setOnlineAppsValue(value)
    ));

    this.subs.add('advancedFilterChanged', (this.licensingsService.advancedFilterSubject.pipe(
      switchMap(
        advancedFilter => {
          this.advancedFilter.set(advancedFilter);
          return this.serviceProxy.getOnlineAppsCount(this.licensingsService.getTenantId(), this.advancedFilter());
        }
      ),
      tap(value => this.setOnlineAppsValue(value))
    ).subscribe()));
  }

  private setOnlineAppsValue(value: OnlineAppsCountOutput) {
    if(value.totalApps === 0) {
      this.onlineApps.set(value.appsInUse.toString());
      return;
    }

    this.onlineApps.set(value.appsInUse.toString() + '/' + value.totalApps.toString());
  }

  ngOnDestroy(): void {
    this.subs.clear();
  }

  run(): void {
    this.initializeRunState(true);
    this.updateData(null);
  }
  stop(): void {
    this.setStopState(false);
  }
  updateProperties(updatedProperties: any): void {  }

  updateData(data: any[]): void {}

  preRun(): void {
    this.run();
  }

}
