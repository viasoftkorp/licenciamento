import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {
  IVsSelectModalData,
  VsGridCpfOrCnpjColumn,
  VsGridPhoneColumn,
  VsGridSimpleColumn,
  VsSelectModalComponent,
} from '@viasoft/components';
import { AccountsService } from '@viasoft/licensing-management/app/pages/accounts/accounts.service';
import { AccountCreateOutput } from '@viasoft/licensing-management/clients/licensing-management';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AccountSelectService {

  constructor(
    private dialog: MatDialog,
    private accountsService: AccountsService
  ) { }

    public openDialog(config?: any): Observable<AccountCreateOutput> {

      if (config && config.onlyActiveAccounts) {
        this.accountsService.onlyActiveAccounts = config.onlyActiveAccounts;
      }

      return this.dialog.open(
        VsSelectModalComponent,
        {
          data: {
            icon: 'briefcase',
            title: 'accounts.accounts',

            service: config && config.service
            ? config.service
            : this.accountsService,

            gridOptions: {
              columns: [
                new VsGridSimpleColumn({
                  headerName: 'accounts.details.companyName',
                  field: 'companyName'
                }),
                new VsGridSimpleColumn({
                  headerName: 'accounts.details.tradingName',
                  field: 'tradingName'
                }),
                new VsGridSimpleColumn({
                  headerName: 'accounts.details.email',
                  field: 'email'
                }),
                new VsGridPhoneColumn({
                  headerName: 'accounts.details.phone',
                  field: 'phone'
                }),
                new VsGridCpfOrCnpjColumn({
                  headerName: 'accounts.details.cnpjCpf',
                  field: 'cnpjCpf',
                  width: 150
                })
              ],
            },
            filterOptions: {

              fields: [
                {
                  condition: 'contains',
                  field: 'companyName',
                  type: 'string'
                },
                {
                 condition: 'contains',
                 field: 'tradingName',
                 type: 'string'
                },
                {
                  condition: 'contains',
                  field: 'email',
                  type: 'string'
                },
                {
                  condition: 'contains',
                  field: 'phone',
                  type: 'string'
                },
                {
                  condition: 'contains',
                  field: 'cnpjCpf',
                  type: 'string'
                }
              ]
            },
          } as IVsSelectModalData
        }
      ).afterClosed().pipe(
        tap(
          () => {
            this.accountsService.onlyActiveAccounts = false;
          }
        )
      );
    }
}
