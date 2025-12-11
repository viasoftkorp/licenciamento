import { LicensingMode } from '../enum/licensing-mode.enum';
import { LicensingModel } from '../enum/licensing-model.enum';

export interface LicensedBundleOutput {
    id: string;
    name: string;
    identifier: string;
    isActive: boolean;
    isCustom: boolean;
    softwareId: string;
    softwareName: string;
    numberOfLicenses: number;
    numberOfTemporaryLicenses: number;
    expirationDateOfTemporaryLicenses: Date;
    licensingModel: LicensingModel;
    licensingMode: LicensingMode;
    licensedBundleId: string;
}
