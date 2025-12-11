import { Component, OnInit, OnDestroy } from '@angular/core';
import { VsDialog } from '@viasoft/components';
import { AccountsDetailComponent } from './accounts-detail/accounts-detail.component';
import { AccountsService } from './accounts.service';
import { MessageService } from '@viasoft/common';
import { Subscription } from 'rxjs';
import { Policies } from '@viasoft/licensing-management/app/tokens/classes/policies.class';

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styleUrls: ['./accounts.component.scss']
})
export class AccountsComponent implements OnInit, OnDestroy {
  
  public readonly policies = Policies;

  subs: Array<Subscription> = [];

  constructor(private vsDialog: VsDialog,
              private accountsService: AccountsService,
              private notification: MessageService) { }

  ngOnInit() {
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  add() {
    this.vsDialog.open(AccountsDetailComponent, null);
  }

  sync() {
    this.subs.push(this.accountsService.updateAccountsWithCrmService().subscribe(
      () => {
        this.notification.info(
          'accounts.notification.started_sync',
          'accounts.notification.started_sync_title'
        );
      }
    ));
  }

}
