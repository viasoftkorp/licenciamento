import { Injectable } from '@angular/core';
import { LicenseUserBehaviourServiceProxy } from '../../../../client/customer-licensing';
import { Observable } from 'rxjs';
import { LicenseUserBehaviourOutputPagedResultDto } from '../../../../client/customer-licensing';
import { LicensingsGetAll } from '../../../common/inputs/licensings-get-all.input';

@Injectable()
export class LicensingsGridService {

  constructor(private licenseUserBehaviourServiceproxy: LicenseUserBehaviourServiceProxy) { }

  getUsersBehaviour(input: LicensingsGetAll): Observable<LicenseUserBehaviourOutputPagedResultDto> {
    return this.licenseUserBehaviourServiceproxy.getUsersBehaviour(
      'America/Sao_Paulo',
      input.licensingIdentifier,
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount
    );
  }

}
