import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-batch-operations-license-number-select',
  templateUrl: './batch-operations-license-number-select.component.html',
  styleUrls: ['./batch-operations-license-number-select.component.scss']
})
export class BatchOperationsLicenseNumberSelectComponent implements OnInit, OnDestroy {

  form: FormGroup;
  subs: Array<Subscription> = [];

  constructor(private dialogRef: MatDialogRef<BatchOperationsLicenseNumberSelectComponent>) { }

  ngOnInit() {
    this.createForm();
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  createForm() {
    this.form = new FormGroup({
      numberOfLicenses: new FormControl(1, [Validators.required, Validators.min(1)])
    });
  }

  save() {
    this.dialogRef.close(this.form.get('numberOfLicenses').value);
  }

  cancel() {
    this.dialogRef.close();
  }

}
