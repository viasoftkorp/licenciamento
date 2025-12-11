import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {
  LicensedTenantSettingsKeys
} from "@viasoft/licensing-management/app/tokens/classes/licensedTenantSettingsKeys.class";

@Component({
  selector: 'app-licensed-tenant-settings-info',
  templateUrl: './licensed-tenant-settings-info.component.html',
  styleUrls: ['./licensed-tenant-settings-info.component.scss']
})
export class LicensedTenantSettingsInfoComponent {
  public infoType: string;
  public title: string;
  public useSimpleHardwareId = LicensedTenantSettingsKeys.UseSimpleHardwareId;

  constructor(
    private readonly dialogRef: MatDialogRef<LicensedTenantSettingsInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { infoType: string }
  ) {
    this.infoType = this.data.infoType;
    if (this.data.infoType === this.useSimpleHardwareId) {
      this.title = "licensings.licensesServer.useSimpleHardwareIdInfoTitle";
    }
  }

  cancel() {
    this.dialogRef.close();
  }
}
