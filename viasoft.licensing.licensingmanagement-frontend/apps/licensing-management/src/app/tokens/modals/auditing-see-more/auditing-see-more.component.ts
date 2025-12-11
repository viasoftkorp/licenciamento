import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {AuditingLogOutput} from "@viasoft/licensing-management/clients/licensing-management";
import {FormBuilder, FormGroup} from "@angular/forms";
import {DatePipe} from "@angular/common";
import {TranslateService} from "@ngx-translate/core";
import {AuditingDataType} from "@viasoft/licensing-management/clients/licensing-management/model/auditing-data-type";

@Component({
  selector: 'app-auditing-see-more',
  templateUrl: './auditing-see-more.component.html',
  styleUrls: ['./auditing-see-more.component.scss']
})
export class AuditingSeeMoreComponent implements OnInit {

  public form: FormGroup;

  public get userName() {
    return this.data.userName;
  }

  public get dateTime() {
    return this.data.dateTime;
  }

  public get type() {
    return this.data.type;
  }

  public get actionName() {
    return this.data.actionName;
  }

  public get details() {
    return this.data.details;
  }

  constructor(
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<any>,
    private datePipe: DatePipe,
    private translateService: TranslateService,
    @Inject(MAT_DIALOG_DATA) private data: AuditingLogOutput
  ) {
  }

  ngOnInit(): void {
    this.createForm();
  }

  private createForm() {
      this.form = this.formBuilder.group({
        userName: [this.data.userName],
        dateTime: [this.datePipe.transform(this.data.dateTime,'dd.MM.yyyy HH:mm')],
        type: [this.translateService.instant(`Auditing.Modal.Type.${AuditingDataType[this.data.type]}`)],
        actionName: [this.data.actionName],
        details: [this.data.details]
      })
  }
}
