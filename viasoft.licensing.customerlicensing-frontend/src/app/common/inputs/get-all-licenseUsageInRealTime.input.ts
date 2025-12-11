export class GetAllLicenseUsageInRealTimeInput {
    licensingIdentifier: string;
    skipCount: number;
    maxResultCount: number;
    advancedFilter?: string;
    sorting?: string;
    filter: string;
}
