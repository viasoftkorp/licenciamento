import { NamedUserOutput } from './named-user-output.interface';

export interface NamedUserAppOutput extends NamedUserOutput {
    licensedAppId: string;
}
