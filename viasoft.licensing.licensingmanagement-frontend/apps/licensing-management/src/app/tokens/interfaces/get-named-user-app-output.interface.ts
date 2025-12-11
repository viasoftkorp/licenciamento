import { GetNamedUserValidationCode } from '../enum/get-named-user-validation-code.enum';
import { NamedUserAppOutput } from './named-user-app-output.interface';

export interface GetNamedUserAppOutput {
    namedUserAppLicenseOutputs: {
        items: NamedUserAppOutput[],
        totalCount: number
    };
    validationCode: GetNamedUserValidationCode;
}
