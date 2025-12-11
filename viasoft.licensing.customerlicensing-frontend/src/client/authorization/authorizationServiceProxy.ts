import { Inject, Injectable, Optional }                      from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams,
         HttpResponse, HttpEvent, HttpParameterCodec }       from '@angular/common/http';
import { Observable }                                        from 'rxjs';
import { Configuration } from './configuration';
import { BASE_PATH } from './variables';
import { CustomHttpParameterCodec } from './encoder';
import { ensureTrailingSlash } from '@viasoft/http';
import { VS_BACKEND_URL } from '@viasoft/client-core';




@Injectable()
export class AuthorizationServiceProxy {
    private readonly enpoint: string;
    constructor(@Inject(VS_BACKEND_URL) protected gateway: string, protected httpClient: HttpClient) {
      this.enpoint = `${ensureTrailingSlash(gateway)}licensing/customerLicensing/authorization/authorizations/policies`;
    }

    public GetPolicies(): Observable<string[]> {
      return this.httpClient.get<string[]>(this.enpoint);
    }
  }
