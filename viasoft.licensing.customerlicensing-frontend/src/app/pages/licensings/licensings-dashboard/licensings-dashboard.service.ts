import { VsDashboardService, VsDashboardDataSource, VsBoardDataSource, VsGadgetDataSource, VsDashboardApiService, VsDashboardInput } from '@viasoft/dashboard';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IGadget } from '@viasoft/dashboard/lib/gadgets/common/igadget';
import { GADGETS } from '../../../common/gadgets/gadgets';
import { DashboardInput } from '../../../../client/dashboard';
import { map } from 'rxjs/operators';

@Injectable()
export class LicensingsDashboardService extends VsDashboardService {

    dashboardInput: DashboardInput = {
      consumerId: '0C876E34-CE79-48C3-AB36-740E42D2EC8E'
    };

    // Trocar dashboarServiceProxy -> VsDashboardApiService
    // Provide no vsDashbardApiService
    // provide VsDashboardPrefix
    constructor(private dashBoardServiceProxy: VsDashboardApiService) {
      super(GADGETS);
    }

    getGadgets(): Observable<VsGadgetDataSource[]> {
      return of(GADGETS);
    }

    getBoardLayouts(): Observable<VsBoardDataSource[]> {
      return of([
          <VsBoardDataSource>{
            id: 100,
            title: 'One Column',
            structure: '100',
            rows: [{
              columns: [{
                flex: '100',
              }]
            }]
          },
          <VsBoardDataSource>{
            id: 400,
            title: 'Two Columns',
            structure: '100/50-50',
            rows: [{
              columns: [{
                flex: '100',
                gadgets: []
              }]
            }, {
              columns: [{
                flex: '50',
                gadgets: []
              }, {
                flex: '50',
                gadgets: []
              }]
            }]
          },
          <VsBoardDataSource>{
            id: 450,
            title: 'Two 50-50 Columns',
            structure: '50-50/50-50',
            rows: [{
              columns: [{
                flex: '50',
                gadgets: []
              }, {
                flex: '50',
                gadgets: []
              }]
            }, {
              columns: [{
                flex: '50',
                gadgets: []
              }, {
                flex: '50',
                gadgets: []
              }]
            }]
          },
          <VsBoardDataSource>{
            id: 1188,
            title: 'Three Columns',
            structure: '33-33-33/33-33-33/33-33-33',
            rows: [{
              columns: [{
                flex: '33.333333333333336',
                gadgets: [],
              }, {
                flex: '33.333333333333336',
                gadgets: []
              }, {
                flex: '33.333333333333336',
                gadgets: []
              }]
            }, {
              columns: [{
                flex: '33.333333333333336',
                gadgets: []
              }, {
                flex: '33.333333333333336',
                gadgets: []
              }, {
                flex: '33.333333333333336',
                gadgets: []
              }]
            }, {
              columns: [{
                flex: '33.333333333333336',
                gadgets: []
              }, {
                flex: '33.333333333333336',
                gadgets: []
              }, {
                flex: '33.333333333333336',
                gadgets: []
              }]
            }]
          },
        ]);
    }

    getDashboard(): Observable<VsDashboardDataSource> {
      return this.dashBoardServiceProxy.getDashboard(this.dashboardInput.consumerId)
      .pipe(map(dashboardSource => {
        if (dashboardSource) {
          return VsDashboardDataSource.fromJS(JSON.parse(dashboardSource.dashboardDataSource));
        }
      }));
    }

    public updateDashboard(dataToUpdate: VsDashboardDataSource): Observable<boolean> {
      const input = <VsDashboardInput>{
        consumerId: this.dashboardInput.consumerId,
        dashboardDataSource: JSON.stringify(dataToUpdate)
      };
      return this.dashBoardServiceProxy.updateDashboard(input);
    }

    restoreDashboard(): Observable<void> {
      return this.dashBoardServiceProxy.restoreDashboard('Custom');
    }

    setAsDefaultBoardDashboard(board: VsDashboardDataSource): Observable<void> {
      const input = <VsDashboardInput>{
        consumerId: this.dashboardInput.consumerId,
        dashboardDataSource: JSON.stringify(board)
      };
      return this.dashBoardServiceProxy.setAsDefaultBoardDashboard(input);
    }

    getGadgetComponentByClassName(gadgetName: string): Observable<IGadget> {
      return of();
    }
}
