import { AddNamedUserToProductValidationCode } from "src/app/common/enums/AddNamedUserToProductValidationCode";
import { NamedUserProductLicenseOutput } from "./NamedUserProductLicenseOutput";

export interface AddNamedUserToProductOutput {
    validationCode: AddNamedUserToProductValidationCode | null;
    output: NamedUserProductLicenseOutput | null;
}