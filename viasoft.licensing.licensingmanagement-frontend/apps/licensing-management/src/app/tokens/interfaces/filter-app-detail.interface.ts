import { LicensingModel } from '../enum/licensing-model.enum';

export interface FilterAppDetail {
    bundleId: string;
    licensingModel: LicensingModel;
}
