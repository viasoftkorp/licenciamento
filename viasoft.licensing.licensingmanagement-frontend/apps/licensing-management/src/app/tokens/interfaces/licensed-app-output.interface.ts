import { Domain } from '../enum/domain.enum';
import { LicensedAppStatus } from '../enum/licensed-app-status.enum';
import { LicensingMode } from '../enum/licensing-mode.enum';
import { LicensingModel } from '../enum/licensing-model.enum';

export interface LicensedAppOutput {
    id: string;
    name: string;
    softwareId: string;
    softwareName: string;
    status: LicensedAppStatus;
    numberOfLicenses: number;
    additionalNumberOfLicenses: number;
    domain: Domain;
    numberOfTemporaryLicenses: number;
    expirationDateOfTemporaryLicenses: Date;
    licensingModel: LicensingModel;
    licensingMode: LicensingMode;
    licensedAppId: string;
}
