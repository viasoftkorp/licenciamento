import { LicensingInfoI18N } from "../interfaces/licensing-info-i18n.interface";

export const LICENSING_INFO_I18N_EN: LicensingInfoI18N = {
    LicensingInfo: {
        Active: 'Active',
        AssignLicense: 'Assign license',
        TransferLicense: 'Transfer license',
        RevokeLicense: 'Revoke license',
        CurrentUser: 'Current user',
        Errors: {
            UnknownError: 'Unknown error',
            NoNamedUser: 'Couldn\'t find the user',
            NoProduct: 'Couldn\'t find the product',
            ProductIsNotNamed: 'This license model is not named',
            NoTenantWithSuchId: 'Couldn\'t find the licensed tenant',
            TooManyNamedUsers: 'You do not have available licenses',
            NamedUserEmailAlreadyInUse: 'Couldn\'t add the named user due to the fact that the email is already in use'
        },
        Identifier: 'Identifier',
        LicensingInformation: 'Licensing information',
        LicensedCnpjs: 'Licensed CNPJs',
        LicensesInUse: 'Licenses in use',
        UsedLicenses: 'Used licenses',
        Modes: {
            Online: 'Online',
            Offline: 'Offline'
        },
        Models: {
            Floating: 'Floating',
            Named: 'Named'
        },
        Modal: {
            SearchUser: 'Search user',
            Save: 'Save',
            Cancel: 'Cancel',
            Identifier: 'Identifier'
        },
        Notification: {
            ConfirmDeletion: 'Are you sure you want to remove \"{{name}}\"?',
            ActionIrreversible: 'This action cannot be reverted'
        },
        Product: 'Product',
        Products: {
            UsedLicenses: 'Used licenses',
            LicensingModel: 'License type',
            LicensingMode: 'Mode',
            Name: 'Product name',
            NumberOfLicenses: 'Acquired quantity',
            Products: 'Products',
            SubscriptionStatus: 'Subscription status',
            Status: {
                ProductBlocked: 'Blocked',
                ProductActive: 'Active',
                BundleBlocked: 'Blocked',
                BundleActive: 'Active'
            },
            Users: 'Users'
        },
        Status: {
            Active: 'Active',
            Blocked: 'Blocked',
            NeedsApproval: 'NeedsApproval',
            Trial: 'Trial',
            Status: 'Status'
        },
        UsersGrid: {
            TenantId: 'Tenant Id',
            User: 'User',
            AppIdentifier: 'App Identifier',
            AppName: 'App Name',
            ProductIdentifier: 'Product identifier',
            ProductName: 'Product name',
            SoftwareIdentifier: 'Software Identifier',
            SoftwareName: 'Software Name',
            SoftwareVersion: 'Software Version',
            HostName: 'Hostname',
            HostUser: 'Host User',
            LocalIPAddress: 'Local IP Adress',
            Language: 'Language',
            OSInfo: 'OS Info',
            BrowserInfo: 'Browser Info',
            DatabaseName: 'Database Name',
            Remove: 'Remove',
            StartTime: 'Login time',
            AcessDuration: 'Access duration',
            LastUpdate: 'Last update',
            CNPJ: 'CNPJ',
            Additionallicense: 'Additional license',
            EndTime: 'Logout time',
            Status: {
                Status: 'Status',
                Online: 'Online',
                Offline: 'Offline'
            },
            UsageTime: 'Usage time in minutes',
            DeviceIdentifier: 'Device identifier',
            LastSeen: 'Last seen'
        },
        User: 'User'
    }
}