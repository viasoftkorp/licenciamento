import { LicenseTenantCreateOutput } from './license-tenant-create-output.interface';

export interface PagedResultLicenseTenantCreateOutput {
    totalCount: number;
    items: LicenseTenantCreateOutput;
}
