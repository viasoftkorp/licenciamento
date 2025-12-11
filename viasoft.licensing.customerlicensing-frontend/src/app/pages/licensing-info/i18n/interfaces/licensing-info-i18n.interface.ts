import { IKeyTranslate } from "@viasoft/common";

export interface LicensingInfoI18N extends IKeyTranslate {
    LicensingInfo: {
        Active: string;
        AssignLicense: string;
        TransferLicense: string;
        RevokeLicense: string;
        CurrentUser: string;
        Errors: {
            UnknownError: string;
            NoProduct: string;
            ProductIsNotNamed: string;
            NoNamedUser: string;
            NoTenantWithSuchId: string;
            TooManyNamedUsers: string;
            NamedUserEmailAlreadyInUse: string;
        }
        Identifier: string;
        LicensingInformation: string;
        LicensedCnpjs: string;
        LicensesInUse: string;
        UsedLicenses: string;
        Modes: {
            Online: string;
            Offline: string;
        }
        Models: {
            Floating: string;
            Named: string;
        }
        Modal: {
            SearchUser: string;
            Save: string;
            Cancel: string;
            Identifier: string
        }
        Notification: {
            ActionIrreversible: string;
            ConfirmDeletion: string;
        }
        Product: string;
        Products: {
            UsedLicenses: string;
            LicensingModel: string;
            LicensingMode: string;
            Name: string;
            NumberOfLicenses: string;
            Products: string;
            SubscriptionStatus: string;
            Status: {
                ProductBlocked: string;
                ProductActive: string;
                BundleBlocked: string;
                BundleActive: string;
            }
            Users: string;
        }
        UsersGrid: {
            TenantId: string;
            User: string;
            AppIdentifier: string;
            AppName: string;
            ProductIdentifier: string;
            ProductName: string;
            SoftwareIdentifier: string;
            SoftwareName: string;
            SoftwareVersion: string;
            HostName: string;
            HostUser: string;
            LocalIPAddress: string;
            Language: string;
            OSInfo: string;
            BrowserInfo: string;
            DatabaseName: string;
            Remove: string;
            StartTime: string;
            AcessDuration: string;
            LastUpdate: string;
            CNPJ: string;
            Additionallicense: string;
            EndTime: string;
            Status: {
                Status: string;
                Online: string;
                Offline: string;
            }
            UsageTime: string;
            DeviceIdentifier: string;
            LastSeen: string;
        }
        User: string;
        Status: {
            Active: string;
            Blocked: string;
            NeedsApproval: string;
            Trial: string;
            Status: string;
        }
    }
}