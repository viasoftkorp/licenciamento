import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { LicenseType } from '../../classes/licenseType.class';

@Component({
  selector: 'app-license-consume-info',
  templateUrl: './license-consume-info.component.html',
  styleUrls: ['./license-consume-info.component.scss']
})
export class LicenseConsumeInfoComponent {

  infoType: string;
  title: string;
  connectionConst = LicenseType.Connection;
  accessConst = LicenseType.Access;

  constructor(private dialogRef: MatDialogRef<LicenseConsumeInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { infoType: string }) {
    this.infoType = this.data.infoType;
    if (this.data.infoType === LicenseType.Connection) {
      this.title = "licensings.connectionInfoTitle";
    } else {
      this.title = "licensings.accessInfoTitle";
    }
  }

  cancel() {
    this.dialogRef.close();
  }
}
