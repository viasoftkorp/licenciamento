import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService, VsSubscriptionManager } from '@viasoft/common';
import { interval } from 'rxjs';
import { takeWhile } from 'rxjs/operators';
import { AddNamedUserToProductValidationCode } from 'src/app/common/enums/AddNamedUserToProductValidationCode';
import { ProductType } from 'src/app/common/enums/ProductType';
import { AddNamedUserDialogDataOutput } from 'src/client/customer-licensing/model/AddNamedUserDialogDataOutput';
import { LicensedBundleStatus } from 'src/client/customer-licensing/model/LicensedBundleStatus';
import { LicensingModels } from 'src/client/customer-licensing/model/LicensingModels';
import { LicensingsService } from '../../licensings/licensings.service';
import { AddNamedUserModalComponent } from '../../tokens/modals/named-user/add-named-user-modal.component';
import { ProductService } from '../product-grid/product.service';
import { NamedUserService } from './named-user.service';
import { UsersGridService } from './users-grid/users-grid.service';
import { VsJwtProviderService } from '@viasoft/http';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss']
})
export class ProductComponent implements OnInit, OnDestroy {

  private subscriptions: VsSubscriptionManager = new VsSubscriptionManager();
  private id: string;
  private productType: ProductType;
  private licensingIdentifier: string;
  public licensesUsageRate: string = '';
  private licensedTenantId: string;
  public product = {
    id: '',
    identifier: '',
    status: {
      icon: '',
      iconColor: '',
      label: ''
    },
    name: '',
    numberOfLicenses: 0,
    numberOfUsedLicenses: 0,
    licensingModel: 0,
    licensingMode: null,
    usedLicensesLabel: ''
  };
  public isLicensingNamed: boolean;
  public isLoad:boolean = false;

  public isLoading = true;
  private timer = interval(60000);
  private hasFinishedTimer = false;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private jwt: VsJwtProviderService,
    private productService: ProductService,
    private licensingsService: LicensingsService,
    private readonly dialog: MatDialog,
    private readonly namedUserService: NamedUserService,
    private readonly messageService: MessageService,
    private usersGridService: UsersGridService
  ) {
    this.licensingIdentifier = this.jwt.getTenantIdFromJwt(this.jwt.getStoredJwt());
  }

  ngOnInit(): void {
    this.id = this.activatedRoute.snapshot.paramMap.get('id');
    let productType = this.activatedRoute.snapshot.paramMap.get('productType');
    this.productType = ProductType[productType];

    if (this.id) {
      this.loadTenantInfo();
      this.subscribeToRefreshProductInfo();
    } else {
      this.router.navigate(['']);
    }
    this.refreshInfo();
  }

  private loadTenantInfo(): void {
    this.subscriptions.add('getTenantInfo', this.licensingsService.getTenantInfoFromId(this.licensingIdentifier).subscribe(
      (result) => {
        if(!result) return
        this.licensedTenantId = result.licensedTenantId;
        this.loadComponent();
      }
    ))
  }

  private loadComponent(): void {
    const licensingIdentifier = this.jwt.getTenantIdFromJwt(this.jwt.getStoredJwt());
    this.subscriptions.add('getLicensedBundle', this.productService.getById(this.licensedTenantId, this.id, this.productType, licensingIdentifier).subscribe(
      (result: any) => {
        this.product = {
          id: result.id,
          identifier: result.identifier,
          status: {
            label: 'LicensingInfo.Products.Status.' + LicensedBundleStatus[result.status],
            icon: result.status === LicensedBundleStatus.BundleActive ? 'check-circle' : 'times-circle',
            iconColor: result.status === LicensedBundleStatus.BundleActive ? '#2D9D78' : '',
          },
          name: result.name,
          numberOfLicenses: result.numberOfLicenses,
          numberOfUsedLicenses: result.numberOfUsedLicenses,
          licensingModel: result.licensingModel,
          licensingMode: result.licensingMode,
          usedLicensesLabel: result.licensingModel === LicensingModels.Named ? 'LicensingInfo.UsedLicenses' : 'LicensingInfo.LicensesInUse'
        }
        this.licensesUsageRate = (this.product.numberOfUsedLicenses / this.product.numberOfLicenses)*100 + '%';
        this.isLicensingNamed = result.licensingModel === LicensingModels.Named;
        this.isLoad = true;
      }
    ))
  }

  private refreshInfo(): void {
    const refreshInfoEveryMinute = this.timer.pipe(
      takeWhile(() => !this.hasFinishedTimer)
    );

    this.subscriptions.add('timer', refreshInfoEveryMinute.subscribe(
      () => this.loadComponent()
    ));
  }

  public assignLicense(): void {
    this.subscriptions.add(
      'open-create',
      this.dialog.open(AddNamedUserModalComponent, {data: {
        licensedBundleId: this.product.id,
        licensingMode: this.product.licensingMode,
        licensedTenantIdentifier: this.licensingIdentifier,
        isCreate: true
      }})
      .afterClosed()
      .subscribe(
        (result: AddNamedUserDialogDataOutput) => {
          if (!result) {
            return;
          }
         this.subscriptions.add(
            'add-named-user',
            this.namedUserService.create(this.licensedTenantId, this.product.id, {
                namedUserEmail: result.key,
                namedUserId: result.value,
                deviceId: result.deviceId,
                productType: this.productType
              }).subscribe(
                () => {
                  this.usersGridService.refreshProductInfo.next(true);
                },
                (output: any) => {
                 this.messageService.error(this.createOperationValidationErrorToString(output?.error?.validationCode));
                 this.usersGridService.refreshProductInfo.next(true);
                }
              )
            );
        }
    ));
  }

  private subscribeToRefreshProductInfo(): void {
    this.usersGridService.refreshProductInfo.subscribe(() => this.loadComponent());
  }

  private createOperationValidationErrorToString(validationCode: AddNamedUserToProductValidationCode): string {
    switch (validationCode) {
      case AddNamedUserToProductValidationCode.NoProduct:
        return 'LicensingInfo.Errors.NoProduct';
      case AddNamedUserToProductValidationCode.ProductIsNotNamed:
        return 'LicensingInfo.Errors.ProductIsNotNamed';
      case AddNamedUserToProductValidationCode.NoLicensedTenant:
        return 'LicensingInfo.Errors.NoTenantWithSuchId';
      case AddNamedUserToProductValidationCode.TooManyNamedUsers:
        return 'LicensingInfo.Errors.TooManyNamedUsers';
      case AddNamedUserToProductValidationCode.NamedUserEmailAlreadyInUse:
        return 'LicensingInfo.Errors.NamedUserEmailAlreadyInUse';
      default:
        return 'LicensingInfo.Errors.UnknownError';
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.clear();
  }

}
