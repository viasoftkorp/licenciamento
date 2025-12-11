import { LicensedBundleStatus } from '../enum/licensed-bundle-status.enum';
import { LicensingMode } from '../enum/licensing-mode.enum';
import { LicensingModel } from '../enum/licensing-model.enum';

export class LicensedBundleUpdateInput {
    licensedTenantId: string;
    bundleId: string;
    status: LicensedBundleStatus;
    numberOfLicenses: number;
    numberOfTemporaryLicenses: number;
    expirationDateOfTemporaryLicenses?: Date | null;
    licensingModel: LicensingModel;
    licensingMode: LicensingMode;
    expirationDateTime?: Date | null;
}
