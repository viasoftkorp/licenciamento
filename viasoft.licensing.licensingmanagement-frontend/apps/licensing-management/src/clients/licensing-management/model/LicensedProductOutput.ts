import { LicensingMode } from "@viasoft/licensing-management/app/tokens/enum/licensing-mode.enum";
import { LicensingModel } from "@viasoft/licensing-management/app/tokens/enum/licensing-model.enum";
import { ProductStatus } from "@viasoft/licensing-management/app/tokens/enum/ProductStatus";
import { ProductType } from "@viasoft/licensing-management/app/tokens/enum/ProductType";

export interface LicensedProductOutput {
    id?: string;
    productId?: string;
    name?: string | null;
    identifier?: string | null;
    isActive?: boolean;
    softwareId?: string;
    softwareName?: string | null;   
    numberOfLicenses?: number;
    licensingModel?: LicensingModel;
    licensingMode?: LicensingMode | null;
    productType?: ProductType;
    expirationDateTime?: Date | null;
    status?: ProductStatus | null;
}