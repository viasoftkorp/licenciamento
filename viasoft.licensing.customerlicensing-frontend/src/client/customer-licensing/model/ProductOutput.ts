import { ProductStatus } from "src/app/common/enums/ProductStatus";
import { ProductType } from "src/app/common/enums/ProductType";
import { LicensingModels } from "./LicensingModels";
import { LicensingModes } from "./LicensingModes";

export interface ProductOutput {
    id?: string | null;
    name?: string | null;
    identifier?: string | null;
    isActive?: boolean | null;
    softwareId?: string | null;
    softwareName?: string | null;
    numberOfLicenses?: number | null;
    licensingModel?: LicensingModels | null;
    licensingMode?: LicensingModes | null;
    numberOfUsedLicenses?: number | null;
    productType?: ProductType | null;
    status?: ProductStatus | null;
}