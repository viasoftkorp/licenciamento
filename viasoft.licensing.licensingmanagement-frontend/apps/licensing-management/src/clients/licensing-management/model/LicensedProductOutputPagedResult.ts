import { LicensedProductOutput } from "./LicensedProductOutput";

export interface LicensedProductOutputPagedResult {
    totalCount?: number;
    items?: Array<LicensedProductOutput> | null;
}