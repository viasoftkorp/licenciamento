export const LICENSING_ENVIRONMENT_I18N_EN = {
  Environment: {
    Organization: {
      AddOrganizationUnit: 'Add organization unit',
      Units: 'Units',
      UnitsFieldName: {
        Name: 'Name',
        Description: 'Description'
      },
      Tooltips: {
        Add: 'Add environment',
        Edit: 'Edit unit',
        Activate: 'Activate',
        Deactivate: 'Deactivate'
      },
      AddUnitModal: {
        Title: 'Organizational unit',
        Name: 'Name',
        Description: 'Description',
        Save: 'Save',
        Cancel: 'Cancel',
        Errors: {
          unknown: 'Unknown error, try again later',
          NameConflict: 'Conflicted name, rename to continue',
          OrganizationNotFound: 'Organization not found',
          IdConflict: 'Id conflict'
        }
      },
      AddEnvironmentModal: {
        Title: 'Organizational unit',
        Name: 'Name',
        Description: 'Description',
        IsWeb: 'Is Web',
        IsDesktop: 'Is Desktop',
        IsProduction: 'Is Production',
        IsMobile: 'Is Mobile',
        DatabaseName: 'Database name',
        Version: 'Version',
        Save: 'Save',
        Cancel: 'Cancel',
        Warnings: {
          Warning: 'Warning!',
          UpdateDbConfirm: 'I am aware of the risks and wish to continue',
          updateDatabaseName: 'When changing the database, the Web and Korp records may diverge, causing the system to lose integrity.',
          updateDatabaseVersion: 'When changing the Version, the system may become inoperable or generate registration problems.'
        },
        Errors: {
          InvalidDbVersion: 'The database version must be in the correct format. Example: 2023.1.2 or 2023.12.34',
          InvalidDbNameRegexValidate: 'The database name cannot contain special characters such as accents, symbols, and spaces.',
          emptyDatabase: 'Database Name is required when Desktop field is checked',
          unknown: 'Unknown error, try again later',
          NameConflict: 'Conflicted name, rename to continue',
          NotFound: 'Not found',
          InProductionConflict: 'There is already an environment for Production, uncheck to continue',
          NotTypedEnvironment: 'Mark the environment as Desktop or Web to continue',
          InvalidDatabaseName: 'Invalid database name, fill in to continue',
          OrganizationUnitNotFound: 'Unknown error, try again later',
          IdConflict: 'Id conflict'
        }
      },
      Errors: {
        userHasNoAuthorization: 'User without authorization'
      }
    },
    Environment: {
      EnvironmentUnitsOf: 'Environment units of ',
      EnvironmentUnits: 'Environment units',
      SelectAnUnitToViewEnvironments: 'Select an unit to view environments',
      UnitEnvironment: {
        Tooltips: {
          Activate: 'Activate',
          Deactivate: 'Deactivate'
        },
        FieldName: {
          Name: 'Name',
          Description: 'Description',
          IsWeb: 'Is Web',
          IsDesktop: 'Is Desktop',
          IsProduction: 'Is Production',
          DatabaseName: 'Database name',
          Version: 'Version',
        }
      }
    }
  }
}
