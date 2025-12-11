import { Component, OnInit } from '@angular/core';
import { LicensingsService } from './licensings.service';
import { VsJwtProviderService } from '@viasoft/http';

@Component({
  selector: 'app-licensings',
  templateUrl: './licensings.component.html',
  styleUrls: ['./licensings.component.scss']
})
export class LicensingsComponent implements OnInit {

  licensingIdentifier: string;

  constructor(private licensingsService: LicensingsService, private jwt: VsJwtProviderService) { }

  ngOnInit() {
    this.licensingIdentifier = this.jwt.getTenantIdFromJwt(this.jwt.getStoredJwt());
    this.licensingsService.setTenantId(this.licensingIdentifier);
  }
}
