import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { JQQB_OP_EQUAL, MessageService, VsSubscriptionManager } from '@viasoft/common';
import { formatDateTime, VsFilterGetItemsOutput, VsFilterItem, VsGridAction, VsGridGetInput, VsGridGetResult, VsGridOptions, VsGridSimpleColumn } from '@viasoft/components';
import { interval, Observable, of } from 'rxjs';
import { filter, map, switchMap, takeWhile } from 'rxjs/operators';
import { ProductType } from 'src/app/common/enums/ProductType';
import { RemoveNamedUserFromProductValidationCode } from 'src/app/common/enums/RemoveNamedUserFromProductValidationCode';
import { UpdateNamedUsersFromProductValidationCode } from 'src/app/common/enums/UpdateNamedUsersFromProductValidationCode';
import { AddNamedUserModalComponent } from 'src/app/pages/tokens/modals/named-user/add-named-user-modal.component';
import { AddNamedUserDialogDataOutput } from 'src/client/customer-licensing/model/AddNamedUserDialogDataOutput';
import { LicensingModels } from 'src/client/customer-licensing/model/LicensingModels';
import { LicensingModes } from 'src/client/customer-licensing/model/LicensingModes';
import { RemoveNamedUserFromProductOutput } from 'src/client/customer-licensing/model/RemoveNamedUserFromProductOutput';
import { NamedUserService } from '../named-user.service';
import { StatusCellComponent } from './status-cell/status-cell.component';
import { UsersGridService } from './users-grid.service';
import { VsJwtProviderService } from '@viasoft/http';

@Component({
  selector: 'app-users-grid',
  templateUrl: './users-grid.component.html',
  styleUrls: ['./users-grid.component.scss']
})
export class UsersGridComponent implements OnInit, OnDestroy {

  public gridOptions: VsGridOptions;
  private licensingIdentifier: string;

  @Input() private productIdentifier: string;
  @Input() private productId: string;
  @Input() private licensingModel: LicensingModels;
  @Input() private licensingMode: LicensingModes;
  @Input() private licensedTenantId: string;
  @Input() private productType: ProductType;

  public get isNamedOffline(): boolean { return this.licensingModel == LicensingModels.Named && this.licensingMode == LicensingModes.Offline};
  public get isNamedOnline(): boolean { return this.licensingModel == LicensingModels.Named && this.licensingMode == LicensingModes.Online};

  public isLoading = true;
  private timer = interval(60000);
  private hasFinishedTimer = false;

  private subscriptions: VsSubscriptionManager = new VsSubscriptionManager();
  constructor(
    private usersGridService: UsersGridService,
    private jwt: VsJwtProviderService,
    private namedUserService: NamedUserService,
    private notification: MessageService,
    private readonly dialog: MatDialog,
  ) { }


  private statusFilterItems: VsFilterItem[] = [
    {
      key: '0',
      value: 'LicensingInfo.UsersGrid.Status.Online'
    },
    {
      key: '1',
      value: 'LicensingInfo.UsersGrid.Status.Offline'
    }
  ]

  ngOnInit(): void {
    this.configureGrid();
    this.subscribeToRefreshGrid();
    this.licensingIdentifier = this.jwt.getTenantIdFromJwt(this.jwt.getStoredJwt());

    const refreshGridEveryMinute = this.timer.pipe(
      takeWhile(() => !this.hasFinishedTimer)
    );

    this.subscriptions.add('timer', refreshGridEveryMinute.subscribe(
      () => this.gridOptions.refresh()
    ));
  }

  private configureGrid(): void {
    this.gridOptions = new VsGridOptions();
    this.gridOptions.id = "3C6CE79B-0629-428B-849A-7A2BD515DD3B";
    this.gridOptions.sizeColumnsToFit = false;

    this.gridOptions.columns = [
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.User',
        field: 'user',
        width: 200
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.DatabaseName',
        field: 'databaseName',
        width: 260,
        filterOptions: { disable: true }
      }),
      new VsGridSimpleColumn({
        headerName: this.isNamedOffline ? 'LicensingInfo.UsersGrid.LastSeen' : 'LicensingInfo.UsersGrid.StartTime',
        field: 'startTime',
        filterOptions: { disable: true },
        width: 180,
        format: (data: any) => data == '0001-01-01T00:00:00Z' ? "" : formatDateTime(data)
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.AppIdentifier',
        field: 'appIdentifier',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.AppName',
        field: 'appName',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.ProductIdentifier',
        field: 'bundleIdentifier',
        width: 200
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.SoftwareIdentifier',
        field: 'softwareIdentifier',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.SoftwareName',
        field: 'softwareName',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.SoftwareVersion',
        field: 'softwareVersion',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.Language',
        field: 'language',
        width: 180
      }),
      new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.OSInfo',
        field: 'osInfo',
        width: 200
      }),

    ]

    if(!this.isNamedOffline){
      this.gridOptions.columns.splice(2, 0, new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.LastUpdate',
        field: 'lastUpdate',
        filterOptions: { disable: true },
        width: 180,
        format: (data: any) => data == '0001-01-01T00:00:00Z' ? "" : formatDateTime(data)
      }));
      this.gridOptions.columns.splice(5, 0, new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.AcessDuration',
        field: 'accessDurationFormatted',
        filterOptions: { disable: true },
        sorting: {
          disable: true
        },
        width: 180,
        format: (data: any) => data == '00:00:00' ? "" : data
      }));
      this.gridOptions.columns.splice(12, 0, new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.HostName',
        field: 'hostName',
        width: 180
      }));
      this.gridOptions.columns.splice(13, 0, new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.HostUser',
        field: 'hostUser',
        width: 180
      }));
      this.gridOptions.columns.splice(14, 0, new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.LocalIPAddress',
        field: 'localIpAddress',
        width: 180
      }));
      this.gridOptions.columns.splice(17, 0, new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.BrowserInfo',
        field: 'browserInfo',
        width: 180
      }));
    }

    if(this.isNamedOnline) {
      this.gridOptions.columns.splice(1, 0, new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.Status.Status',
        field: 'status',
        type: StatusCellComponent,
        width: 180,
        filterOptions: {
          operators: [JQQB_OP_EQUAL],
          mode: 'selection',
          multiple: true,
          getItems: () => this.getStatusFilterItems(),
          getValidItems: (keys: any[]) => {
            return of(this.statusFilterItems.filter(filterItem =>
              keys.includes(filterItem.key)
            ))
          }
        },
      }))
    } else if(this.isNamedOffline) {
      this.gridOptions.columns.splice(1, 0, new VsGridSimpleColumn({
        headerName: 'LicensingInfo.UsersGrid.DeviceIdentifier',
        field: 'deviceId',
        width: 180
      }))
    }

    if(this.licensingModel == LicensingModels.Named){
      this.gridOptions.actions = [
        <VsGridAction>{
          icon: 'trash-alt',
          tooltip: 'LicensingInfo.RevokeLicense',
          callback: (rowIndex, data) => this.delete(data),
        },
        <VsGridAction>{
          icon: 'arrow-alt-right',
          tooltip: 'LicensingInfo.TransferLicense',
          callback: (rowIndex, data) => this.transferLicense(data),
        }
      ]
    }

    this.gridOptions.get = (input: VsGridGetInput) => this.get(input);
  }

  private delete(data: any): void {
    this.subscriptions.add('confirmDeletion', this.notification.confirm(
      'LicensingInfo.Notification.ActionIrreversible',
      'LicensingInfo.Notification.ConfirmDeletion|name:' + data.user
    ).pipe(
      filter((result) => Boolean(result)),
      switchMap(() => this.namedUserService.delete(this.licensedTenantId, this.productId, data.user, this.productType))
    ).subscribe(() => this.usersGridService.refreshProductInfo.next(true), (result: any) => {
      const output = <RemoveNamedUserFromProductOutput> result.error;
      this.notification.error(this.deleteProductOperationValidationErrorToString(output.validationCode));
    }));
  }

  private deleteProductOperationValidationErrorToString(validationCode: RemoveNamedUserFromProductValidationCode): string {
    switch (validationCode) {
      case RemoveNamedUserFromProductValidationCode.NoProduct:
        return 'LicensingInfo.Errors.NoProduct';
      case RemoveNamedUserFromProductValidationCode.NoLicensedTenant:
        return 'LicensingInfo.Errors.NoLicensedTenant';
      case RemoveNamedUserFromProductValidationCode.NoNamedUser:
        return 'LicensingInfo.Errors.NoNamedUser';
      default:
        return 'LicensingInfo.Errors.UnknownError';
    }
  }

  private getStatusFilterItems(): Observable<VsFilterGetItemsOutput>{
    return of({
      items: this.statusFilterItems,
      totalCount: this.statusFilterItems.length
    })
  }

  private subscribeToRefreshGrid(): void {
    this.subscriptions.add('refreshGrid', this.usersGridService.refreshProductInfo.subscribe(() => this.gridOptions.refresh()));
  }

  private get(input: VsGridGetInput): Observable<any> {
    if(this.licensingModel == LicensingModels.Floating) {
      return this.usersGridService.getUserBehaviourFloating(input, this.licensingIdentifier, this.productIdentifier, this.productType)
      .pipe(
        map((list: any) => new VsGridGetResult(list.items, list.totalCount))
      );
    } else {
      if(this.licensingMode == LicensingModes.Online) {
        return this.usersGridService.getUserBehaviourNamedOnline(input, this.licensedTenantId, this.licensingIdentifier, this.productIdentifier, this.productId, this.productType)
        .pipe(
          map((list: any) => new VsGridGetResult(list.items, list.totalCount))
        )
      } else {
        return this.usersGridService.getUserBehaviourNamedOffline(input, this.licensedTenantId, this.licensingIdentifier, this.productIdentifier, this.productId, this.productType)
      .pipe(
        map((list: any) => new VsGridGetResult(list.items, list.totalCount))
        )
      }
    }
  }

  private transferLicense(input: any): void {
    this.subscriptions.add(
      'open-transfer',
      this.dialog.open(AddNamedUserModalComponent, {data: {
        ProductId: this.productId,
        licensingMode: this.licensingMode,
        licensedTenantIdentifier: this.licensingIdentifier,
        namedUserEmail: input.user,
        deviceId: input.deviceId,
        isCreate: false
      }})
      .afterClosed()
      .subscribe(
        (result: AddNamedUserDialogDataOutput) => {
          if (!result) {
            return;
          }
         this.subscriptions.add(
            'update-named-user',
            this.namedUserService.update(this.licensedTenantId, this.productId, input.user,{
                namedUserEmail: result.key,
                namedUserId: result.value,
                deviceId: result.deviceId,
                productType: this.productType
              }).subscribe(
                () => {
                  this.gridOptions.refresh();
                },
                (output: any) => {
                 this.notification.error(this.updateNamedProductOperationValidationErrorToString(output?.error?.validationCode));
                 this.gridOptions.refresh();
                }
              )
            );
        }
    ));
  }

  private updateNamedProductOperationValidationErrorToString(validationCode: UpdateNamedUsersFromProductValidationCode): string {
    switch (validationCode) {
      case UpdateNamedUsersFromProductValidationCode.NoProduct:
        return 'LicensingInfo.Errors.NoProduct';
      case UpdateNamedUsersFromProductValidationCode.NoLicensedTenant:
        return 'LicensingInfo.Errors.NoLicensedTenant';
      case UpdateNamedUsersFromProductValidationCode.NoNamedUser:
        return 'LicensingInfo.Errors.NoNamedUser';
      case UpdateNamedUsersFromProductValidationCode.NamedUserEmailAlreadyInUse:
        return 'LicensingInfo.Errors.NamedUserEmailAlreadyInUse';
      default:
        return 'LicensingInfo.Errors.UnknownError';
    }
  }

  ngOnDestroy(): void {
    this.hasFinishedTimer = true;
    this.subscriptions.clear();
  }
}
