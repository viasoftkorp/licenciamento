import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-licensings-info-cards-gadget',
  templateUrl: './licensings-info-cards-gadget.component.html',
  styleUrls: ['./licensings-info-cards-gadget.component.scss']
})
export class LicensingsInfoCardsGadgetComponent implements OnInit {

  @Input() value: string;
  @Input() description: string;

  constructor() {}

  ngOnInit() {
  }
}
