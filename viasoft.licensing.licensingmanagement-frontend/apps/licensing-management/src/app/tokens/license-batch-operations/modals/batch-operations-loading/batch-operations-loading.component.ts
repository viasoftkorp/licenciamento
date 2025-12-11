import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { LicenseBatchOperationsService } from '../../license-batch-operations.service';
@Component({
  selector: 'app-batch-operations-loading',
  templateUrl: './batch-operations-loading.component.html',
  styleUrls: ['./batch-operations-loading.component.scss']
})
export class BatchOperationsLoadingComponent implements OnInit {

  public loading = true;
  public showError = true;
  public showEndMsg = false;

  constructor(private dialogRef: MatDialogRef<BatchOperationsLoadingComponent>, private licenseBatchOperationsService: LicenseBatchOperationsService) { }

  ngOnInit() {
    this.licenseBatchOperationsService.getErrorOrFinishInBatchOperation().subscribe((hasError) => {
      this.loading = false;
      this.showEndMsg = true;
      this.showError = hasError;
    });
  }

  close() {
    this.dialogRef.close();
  }

}
