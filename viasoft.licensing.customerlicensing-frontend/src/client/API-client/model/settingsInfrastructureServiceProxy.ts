import {Inject, Injectable} from '@angular/core';
import {
  DeployCommand,
  InfrastructureSettings, UninstallVersionCommand, UpdateVersionCommand
} from "../../../app/pages/settings/infrastructure-settings/infrastructure-settings.interface";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {API_GATEWAY, ensureTrailingSlash} from "@viasoft/http";
import { first } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class SettingsInfrastructureServiceProxy {

  private baseUrl: string

  constructor(
    @Inject(API_GATEWAY) private apiGateway: string,
    private httpclient: HttpClient
  ) { }

  private getbaseUrl(): void {
    this.baseUrl = `${ensureTrailingSlash(this.apiGateway)}licensing/customer-licensing/settings`
  }
  public getGatewayAndVersions(tenantID) {
    this.getbaseUrl();
    return this.httpclient.get<InfrastructureSettings>(this.baseUrl + "?tenantId=" + tenantID)
  }

  public updateGateway(body) {

    return this.httpclient.put<InfrastructureSettings>(this.baseUrl, JSON.stringify(body), {
      headers: new HttpHeaders({'Content-Type': 'application/json'}),
      observe: 'body'
    })
  }

  public copyToClipboardImplementation(text: any): void {
    this.httpclient.get<DeployCommand>( this.baseUrl + '/' + text).pipe(first()).subscribe(res => {
      navigator.clipboard.writeText(res.deployCommand);
    })
  }

  public copyToClipboardUpdate(text: any): void {
    this.httpclient.get<UpdateVersionCommand>( this.baseUrl + '/' + text + '/update').pipe(first()).subscribe(res => {
      navigator.clipboard.writeText(res.updateVersionCommand);
    })
  }

  public copyToClipboardUninstall(text: any): void {
    this.httpclient.get<UninstallVersionCommand>( this.baseUrl + '/' + text + '/uninstall').pipe(first()).subscribe(res => {
      navigator.clipboard.writeText(res.uninstallVersionCommand);
    })
  }
}
