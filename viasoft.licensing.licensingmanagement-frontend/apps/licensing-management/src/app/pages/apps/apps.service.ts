import { Injectable } from '@angular/core';
import { IVsBaseCrudService } from '@viasoft/common';
import { AppGetAllInput } from '@viasoft/licensing-management/app/tokens/inputs/app-get-all.input';
import {
    AppCreateInput,
    AppCreateOutput,
    AppDeleteOutput,
    AppUpdateInput,
    AppUpdateOutput
} from '@viasoft/licensing-management/clients/licensing-management';
import { AppServiceProxy } from '@viasoft/licensing-management/clients/licensing-management';

@Injectable({
    providedIn: 'root'
})
export class AppsService implements IVsBaseCrudService<
    AppCreateInput,
    AppCreateOutput,
    AppUpdateInput,
    AppUpdateOutput,
    AppDeleteOutput,
    AppCreateOutput,
    AppGetAllInput
> {

    softwareIdFilterBy: string;
    licensedApps: string[] = [];

    constructor(private apps: AppServiceProxy) { }

    getAll(
        input: AppGetAllInput
    ) {
        return this.apps.getAll(
            this.softwareIdFilterBy,
            this.licensedApps,
            input.filter,
            input.advancedFilter,
            input.sorting,
            input.skipCount,
            input.maxResultCount);
    }

    getAllActiveApps(
        filter: string,
        advancedFilter: string,
        sorting: string,
        skipCount: number,
        maxResultCount: number,
    ) {
        return this.apps.getAllActiveApps(this.softwareIdFilterBy,
            this.licensedApps,
            filter,
            advancedFilter,
            sorting,
            skipCount,
            maxResultCount);
    }

    getById(id: string) {
        return this.apps.getById(id);
    }

    GetAllAppsInBundle(bundleId: string) {
        return this.apps.getAllAppsInBundle(bundleId);
    }

    create(app: AppCreateInput) {
        return this.apps.create(app);
    }

    update(app: AppUpdateInput) {
        return this.apps.update(app);
    }

    delete(id: string) {
        return this.apps._delete(id);
    }

}
