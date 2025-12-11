import { LicensingMode } from '../enum/licensing-mode.enum';
import { NamedUserTypes } from '../enum/named-user-types.enum';

export interface NamedUsersDialogDataInput {
    licensedTenantId: string;
    licensedEntityId: string;
    namedUserType: NamedUserTypes;
    licensingMode: LicensingMode;
    numberOfLicenses: number;
    deviceId: string;
    licensedTenantIdentifier: string;
}
