import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'progress-bar',
  templateUrl: './progress-bar.component.html',
  styleUrls: ['./progress-bar.component.scss']
})
export class ProgressBarComponent implements OnInit {

  @Input() public color: string;
  @Input() public percentage: string;
  public grey = '#F3F2F1';

  ngOnInit(): void {
    if(!this.color) {
      this.color = this.grey;
    }
  }

}
