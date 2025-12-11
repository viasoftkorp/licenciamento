import { Injectable } from '@angular/core';
import { IVsBaseCrudService } from '@viasoft/common';
import { SoftwareServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';
import { SoftwareGetAllInput } from '@viasoft/licensing-management/app/tokens/inputs/software-get-all.input';

import {
  SoftwareCreateInput,
  SoftwareCreateOutput,
  SoftwareDeleteOutput,
  SoftwareUpdateInput,
  SoftwareUpdateOutput
} from '@viasoft/licensing-management/clients/licensing-management';

@Injectable({
  providedIn: 'root'
})
export class SoftwaresService implements IVsBaseCrudService<
SoftwareCreateInput,
SoftwareCreateOutput,
SoftwareUpdateInput,
SoftwareUpdateOutput,
SoftwareDeleteOutput,
SoftwareCreateOutput,
SoftwareGetAllInput
> {

  onlyActiveSoftwares: boolean;

  constructor(private softwares: SoftwareServiceProxy) { }

  getAll(input: SoftwareGetAllInput) {
    return this.softwares.getAll(
      this.onlyActiveSoftwares,
      input.filter,
      input.advancedFilter,
      input.sorting,
      input.skipCount,
      input.maxResultCount);
  }

  getById(id: string) {
    return this.softwares.getById(id);
  }

  create(software: SoftwareCreateInput) {
    return this.softwares.create(software);
  }

  update(software: SoftwareUpdateInput) {
    return this.softwares.update(software);
  }

  delete(id: string) {
    return this.softwares._delete(id);
  }
}
