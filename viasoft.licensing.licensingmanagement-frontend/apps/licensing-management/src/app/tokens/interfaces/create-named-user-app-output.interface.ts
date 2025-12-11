import { CreateNamedUserAppValidationCode } from '../enum/create-named-user-app-validation-code.enum';
import { NamedUserAppOutput } from './named-user-app-output.interface';

export interface CreateNamedUserAppOutput {
    validationCode: CreateNamedUserAppValidationCode;
    output: NamedUserAppOutput;
}
