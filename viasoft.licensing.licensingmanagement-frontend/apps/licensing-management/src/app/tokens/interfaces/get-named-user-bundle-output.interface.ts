import { GetNamedUserValidationCode } from '../enum/get-named-user-validation-code.enum';
import { NamedUserBundleOutput } from './named-user-bundle-output.interface';

export interface GetNamedUserBundleOutput {
    namedUserBundleLicenseOutputs: {
        items: NamedUserBundleOutput[],
        totalCount: number
    };
    namedUserFromBundleValidationCode: GetNamedUserValidationCode;
}
