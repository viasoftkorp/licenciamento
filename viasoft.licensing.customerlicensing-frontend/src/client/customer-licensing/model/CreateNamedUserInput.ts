import { ProductType } from "src/app/common/enums/ProductType";

export interface CreateNamedUserInput {
    namedUserId: string;
    namedUserEmail: string;
    deviceId: string;
    productType: ProductType;
}