import { Component, Input, OnInit } from '@angular/core';
import { VsTableCell } from '@viasoft/components';
import { UserBehaviourStatus } from 'src/client/customer-licensing/model/UserBehaviourStatus';

@Component({
  selector: 'app-status-cell',
  templateUrl: './status-cell.component.html',
  styleUrls: ['./status-cell.component.scss']
})
export class StatusCellComponent implements VsTableCell, OnInit {

  @Input() public data: any;
  public status: string;
  public iconColor: string;

  ngOnInit(): void {
    this.status = 'LicensingInfo.UsersGrid.Status.' + UserBehaviourStatus[this.data.status];
    this.iconColor = this.data.status == UserBehaviourStatus.Online ? "#2D9D78" : "";
  }

}
