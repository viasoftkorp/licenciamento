export const en = {
  Permissions: {
    Account: 'Account',
    AccountCreate: 'Create Account',
    AccountDelete: 'Remove Account',
    AccountUpdate: 'Update Account',
    License: 'License',
    LicenseCreate: 'Create License',
    LicenseDelete: 'Remove License',
    LicenseUpdate: 'Update License',
    Software: 'Software',
    SoftwareCreate: 'Create Software',
    SoftwareDelete: 'Remove Software',
    SoftwareUpdate: 'Update Software',
    App: 'App',
    AppCreate: 'Create App',
    AppDelete: 'Remove App',
    AppUpdate: 'Update App',
    Product: 'Product',
    ProductCreate: 'Create Product',
    ProductDelete: 'Remove Product',
    ProductUpdate: 'Update Product',
    BatchOperation: 'Batch operations',
    BatchOperationInsertAppInLicenses: 'Add App in licensings',
    BatchOperationRemoveAppInLicenses: 'Remove App in licensings',
    BatchOperationInsertProductsInLicenses: 'Add products in licensings',
    BatchOperationInsertAppsInLicenses: 'Add Apps in licensings',
    BatchOperationInsertAppsInProducts: 'Add Apps in products'
  },

  common: {
    help: 'Help',
    ok: 'OK',
    save: 'Save',
    apply_batch_operations: 'Apply batch operations',
    cancel: 'Cancel',
    add: 'Add',
    remove: 'Remove',
    delete: 'Delete',
    back: 'Back',
    new: 'New',
    save_header: 'Save header',
    notification: {
      confirm_deletion: 'Are you sure you want to delete \"{{name}}\"?',
      action_irreversible: 'This action cannot be reverted',
      attention_batch_operation: 'Attention the following value will be applied to all selected items!',
      apply_batch_operation: 'Attention batch operations are being applied, wait for the process to be completed.',
      end_batch_operation: 'Btach operation successfully completed.'
    },
    error: {
      invalidDate: 'Expiration date invalid',
      expiration_date_must_be_valid: 'Expiration date must be greater than or equal to the current date',
      numberOfLicensesIsRequired: 'At least one license is required',
      could_not_delete: 'Could not delete \"{{name}}\"',
      could_not_create: 'Could not create \"{{name}}\"',
      could_not_edit: 'Could not edit \"{{name}}\"',
      software_is_being_used: 'This software is being used by other applications and/or products',
      product_is_being_used: 'This product has already been used in a license',
      app_is_being_used: 'This application has already been used in a license',
      account_is_being_used: 'This account is being used by license \"{{name}}\"',
      identifier_already_exists: 'Identifier already exists',
      field_is_required: 'This field is required',
      app_already_licensed: 'There are applications already licensed, ignoring them.',
      some_apps_already_licensed: 'One or more applications are already licensed by another package (s) or have been assigned separately, in this case they have been ignored.',
      number_of_licenses_equals_zero: 'The number of licenses needs to be greater than zero.',
      number_of_licenses_less_than_zero: 'The number of licenses can\'t be less than zero.',
      additional_number_of_licenses_less_than_zero: 'The additional number of licenses can\'t be zero.',
      cant_remove_default_app: 'This app is a default app and can\'t be removed from the license.',
      could_not_find_cnpj: 'Could not find a registration with the informed CNPJ/CPF',
      cnpj_already_registered: 'This CNPJ is already registered in account \"{{name}}\"',
      cnpj_already_in_use: 'This CNPJ is already is use',
      invalid_email: 'Invalid email.',
      only_numbers: 'Please input only numbers.',
      could_not_find_adress: 'Could not find a adress.',
      invalid_phone: 'Invalid phone.',
      invalid_phone_value: 'The phone number "{{phoneNumber}}" is invalid.',
      invalid_accountId: 'Invalid account',
      invalid_expirationDate: 'Invalid expiration date',
      error_apply_batch_operation: 'There were errors when applying the batch operation.',
      administration_email_is_being_used: 'This administrator email is beign used by license \"{{name}}\"',
      invalid_administration_email: 'Invalid administration email',
      AddLooseAppToLicenseErrors: {
        Unknown: 'An unknown error occurred while adding this loose application',
        DuplicatedIdentifier: 'This application is already being used in this license'
      },
      CouldntGetInfrastructureConfiguration: 'Couldn\'t get infrastructure configuration'
    }
  },

  Auditing: {
    UserName: 'User name',
    ActionName: 'Action name',
    DateTime: 'Occurrence date',
    Details: 'Actions details',
    Title: 'Auditing',
    BatchAction: 'Batch Action',
    Type: 'Action Type',
    Modal: {
      Close: 'Close',
      AuditingSeeMore: 'Auditing details',
      Type: {
        InsertAppInLicenses: 'Add App in licensings',
        RemoveAppInLicenses: 'Remove App in licensings',
        InsertMultipleBundlesInMultipleLicenses: 'Add multiple bundles in multiple licenses',
        InsertMultipleAppsInMultipleLicenses: 'Add multiple apps in multiple licenses',
        InsertMultipleAppsInMultipleBundles: 'Add multiple apps in multiple bundles',
        InsertMultipleAppsFromBundlesInLicenses: 'Add multiple apps from bundles in licenses'
      }
    }
  },

  licensings: {
    connectionInfoTitle: 'Information about license consumption in connection mode',
    accessInfoTitle: 'Information on license consumption in access mode',
    connectionInfoOne: `In this consumption mode the license server will try to fetch the licenses for the current application first in a product,
    if the user has a product with the application then the server will perform a check to validate that the application is already in use,
    if it is no license will be consumed, otherwise a license of the product will be consumed.`,
    connectionInfoTwo: `If the server does not find the application in a product, then a check will be made to see if the user has the application as a standalone,
    if so, the server will perform a check to validate that the application is already in use,
    if it is no license will be consumed, otherwise a separate application license will be consumed.`,
    accessInfoOne: `In this consumption mode the license server will try to fetch the licenses for the current application first in a product,
    if the user has a product with the application then the server will consume a license for that product.`,
    accessInfoTwo: `If the server does not find the application in a product, then a check will be made to see if the user has the loose application,
    if so, the server will consume a license for this separate application..`,
    infoLicenseConsumeConnection: 'The license will be consumed by connection, regardless of the application access number.',
    infoLicenseConsumeAcess: 'The license will be consumed by access, consuming license for each access to the applications.',
    connectionLicenseConsume: 'Consume license by connection',
    acessLicenseConsume: 'Consume license by acess',
    licensing: 'Licensing',
    numberOfTemporaryLicenses: 'Temporary licenses',
    licensings: 'Licensings',
    accountId: 'Account',
    status: 'Status',
    cnpjCpf: 'Licensed entities',
    name: 'Name',
    approval: 'Pending Approval',
    blocked: 'Blocked',
    trial: 'Trial',
    active: 'Active',
    readOnly: 'Read Only',
    expirationDateTime: 'Expiration Date',
    numberOfDaysToExpiration: 'Number of Days until expiration',
    numberOfLicenses: '№ of licenses',
    identifier: 'Identifier',
    additionalLicenses: 'Additional Licenses',
    administratorEmail: 'Administrator email',
    invalidEmail: 'Invalid email',
    notes: 'Observations',
    hardwareId: 'Hardware identifier',
    namedLicense: 'Named license',
    offlineMode: 'Offline Mode',
    licensingModelTooltip: 'Defines whether the license model is named or floating',
    licensingModeTooltip: 'Defines whether the license mode is online or offline',
    copyTenantIdTooltip: 'Copy Identifier',
    manageNamedUsersTitle: 'Manage named licenses',
    manageNamedUsersSubTitle: {
      intro: 'This application has',
      middle: 'available licenses',
      mode: 'mode'
    },
    user: 'User',
    addUser: 'Add user',
    transferLicense: 'Transfer license',
    currentUser: 'Current user:',
    searchUser: 'Search user',
    errors: {
      unknownError: 'Unknown error',
      notANamedLicense: 'This license model is not named',
      noLicensedProductWithSuchId: 'Couldn\'t find the licensed product',
      noTenantWithSuchId: 'Couldn\'t find the licensed tenant',
      TooManyNamedUsers: 'You do not have available licenses',
      noLicensedAppWithSuchId: 'Couldn\'t find the licensed app',
      namedUserEmailAlreadyInUse: 'Couldn\'t add the named user due to the fact that the email is already in use'
    },
    licensesServer: {
      title: 'Licenses Server',
      useHardwareIdSimpleConfigurations: 'Use simple hardware identifier configurations',
      useSimpleHardwareIdInfoTitle: 'Information about using simple hardware identifier configurations',
      useSimpleHardwareIdInfoContent: `This option should be marked by customers who make use of cloud servers, such as Oracle Cloud, Microsoft Azure, and so on.
      This is necessary because these providers keep changing the virtual machine physical location after reboot, hence, changing the hardware identifier.`
    },
    tooltipForSagaInfo: {
      status: {
        processing: 'The status will automatically update to active when the admin account is created',
        failure: 'There was an error trying to create the administrator account'
      },
      email: {
        success: 'Administrator account has been successfully created',
        processing: 'Administrator account is being created',
        failure: 'There was an error trying to create the administrator account'
      }
    }
  },

  softwares: {
    software: 'Software',
    softwares: 'Softwares',
    new_software: 'New Software',
    name: 'Name',
    identifier: 'Identifier',
    active: 'Active',
  },

  products: {
    main_title: 'Manage products',
    batch_operation: 'Batch operation',
    batch_operation_products: 'Batch operation - licencing',
    product: 'Product',
    products: 'Products',
    new_product: 'New product',
    name: 'Name',
    identifier: 'Identifier',
    active: 'Active',
    software: 'Software',
    custom: 'Custom',
    addAppInLincense: 'Add application to existing licenses for this product?',
    removeAppInLicense: 'Remove application to existing licenses for this product?'
  },
  apps: {
    main_title: 'Manage apps',
    batch_operation: 'Batch operation',
    batch_operation_products: 'Batch operation - products',
    batch_operation_licenses: 'Batch operation - licencing',
    app: 'Application',
    apps: 'Applications',
    apps_in_product: 'Applications in product ',
    new_apps: 'New applications',
    active: 'Active',
    name: 'Name',
    identifier: 'Identifier',
    software: 'Software',
    loose: 'Loose applications',
    default: 'Default',
    domains: {
      domain: 'Domain',
      VD: 'Sales',
      CP: 'Purchases',
      FA: 'Billing',
      FI: 'Financial',
      RMA: 'RMA',
      LG: 'Logistics',
      FC: 'Fiscal',
      CT: 'Accounting',
      EN: 'Engineering',
      PD: 'Production',
      MT: 'Maintenance',
      QA: 'Quality assurance',
      RH: 'Human resources',
      CG: 'Configurations',
      DEV: 'Development',
      CST: 'Customized',
      LS: 'Licensing',
      MOB: 'Mobile',
      REP: 'Reports',
      PROJ: 'Projects'
    }
  },
  accounts: {
    accounts: 'Accounts',
    account: 'Account',
    generalInfo: 'General info',
    adressInfo: 'Account adress',
    billingInfo: 'Billing adress',
    sync: 'Sync with CRM',
    details: {
      companyName: 'Company name',
      cnpjCpf: 'CNPJ or CPF',
      phone: 'Telephone',
      webSite: 'Website',
      email: 'Email',
      billingEmail: 'Billing email',
      tradingName: 'Trading name',
      accountStatus: {
        status: 'Status',
        active: 'Active',
        blocked: 'Inactive'
      },
      street: 'Street',
      number: 'Number',
      detail: 'Complement',
      neighborhood: 'Neighborhood',
      city: 'City',
      state: 'State',
      country: 'Country',
      zipCode: 'Zip Code'
    },
    notification: {
      started_sync_title: 'Synchronization in progress',
      started_sync: 'You will receive a notification when the synchronization ends.'
    }
  },
  filequota: {
    title: 'Storage Quotas',
    selectedApp: 'Selected app',
    addApp: {
      title: 'Select an app to add its quota',
      search: 'Search for app',
      grid: {
        appName: 'App Name'
      }
    }
  },
  file_provider: {
    configurationTitle: 'File Storage Settings',
    grid: {
      appName: 'Application',
      quotaLimit: 'Quota Limit'
    },
    form: {
      maxSizePerFile: 'Allowed size per file (MB)',
      quotaLimit: 'Allowed quota size (MB)',
      tenantQuotaLimit: 'Allowed Tenant quota size (MB)',
      byteSizeNoLimitHint: 'Set to -1 to turn inlimited',
      acceptedExtensions: 'Allowed extensions',
      acceptedExtensionsHint: 'Comma-separated list of media types (leave it blank for accepting all)',
      errors: {
        limitMustBeSet: 'A limit must be set, leave -1 to leave unlimited',
        unknownErrorDuringSave: 'Error trying to save, please try again later'
      }
    }
  },
  infrastructureConfiguration: {
    title: 'Infrastructure configurations',
    gateway: 'Gateway Address',
    desktopDatabaseName: 'Desktop database name',
    error: {
      invalidGateway: 'Invalid gateway'
    }
  },
  Organization: {
    Title: 'Organization',
    Unit: {
      Title: 'Organization Unit',
      Activate: 'Activate',
      Deactivate: 'Deactivate',
      AddEnvironment: 'Add Environment',
      EditUnit: 'Edit',
      SelectAnUnitToViewEnvironments: 'Select an organizational unit to view its environments',
      Fields: {
        Name: 'Name',
        Description: 'Description'
      },
      Errors: {
        NameConflict: 'Conflicted name, rename to continue',
        Unknown: 'Unknown error, try again later',
      },
      Warnings: {
        EnvironmentsWillNotBeActivatedAutomatically: 'The environments belonging to this unit will not be active automatically.'
      }
    },
    UnitEnvironment: {
      Title: 'Environment',
      Environments: 'Environments',
      EnvironmentOf: 'Environments belonging to unit \"{{unitName}}\"',
      Activate: 'Activate',
      Deactivate: 'Deactivate',
      CopyEnvironmentIdTooltip: 'Copy environment Id',
      Fields: {
        Name: 'Name',
        Description: 'Description',
        IsProduction: 'Production Environment',
        IsMobile: 'Mobile Environment',
        IsWeb: 'Web Environment',
        IsDesktop: 'Desktop Environment',
        DatabaseName: 'DatabaseName',
        Version: 'Desktop Database Version'
      },
      Errors: {
        InvalidDbVersion: 'The database version must be in the correct format. Example: 2023.1.2 or 2023.12.34',
        InvalidDbNameRegexValidate: 'The database name cannot contain special characters such as accents, symbols, and spaces.',
        NameConflict: 'Conflicted name, rename to continue',
        OrganizationUnitNotFound: 'Unknown error, try again later',
        InProductionConflict: 'There is already an environment for Production, uncheck to continue',
        NotTypedEnvironment: 'Mark the environment as Desktop or Web to continue',
        InvalidDatabaseName: 'Invalid database name, fill in to continue',
        Unknown: 'Unknown error, try again later',
        EmptyDatabase: 'Database Name is required when Desktop field is checked'
      }
    }
  }
};
