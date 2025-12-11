import { NamedUserOutput } from './named-user-output.interface';

export interface NamedUserBundleOutput extends NamedUserOutput {
    licensedBundleId: string;
}
