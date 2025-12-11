import { LicensedBundleOutput } from './licensed-bundle-output.interface';

export interface LicensedBundleOutputPagedResult {
    items: LicensedBundleOutput[];
    totalCount: number;
}