using System;
using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Mock.UtilsReturnsToMethods
{
    public class ReturnLicenseTenantById
    {
        protected static LicenseByTenantIdOld BuildLicenseForTenantWithSimpleLicenseWithinBundle()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.SingleLicenseWithinBundle.LicensedBundleId,
                    Name = Apps.SingleLicenseWithinBundle.Name,
                    Identifier = Apps.SingleLicenseWithinBundle.Identifier,
                    AppId = Apps.SingleLicenseWithinBundle.Id,
                    Status = Apps.SingleLicenseWithinBundle.Status,
                    NumberOfLicenses = Apps.SingleLicenseWithinBundle.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicenseWithinBundle.AdditionalNumberOfLicenses
                }
            };
            
            var licensedBundleDetails = new List<LicensedBundleDetails>
            {
                new LicensedBundleDetails
                {
                    BundleId = Bundles.Simple.Id,
                    Identifier = Bundles.Simple.Identifier,
                    Name = Bundles.Simple.Name,
                    Status = Bundles.Simple.Status,
                    NumberOfLicenses = Bundles.Simple.NumberOfLicenses 
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.SimpleLicenseWithinBundle.Id,
                Tenants.SimpleLicenseWithinBundle.Status,
                Tenants.SimpleLicenseWithinBundle.ExpirationDateTime,
                Tenants.SimpleLicenseWithinBundle.Cnpjs,
                new LicensedTenantDetails(licensedBundleDetails, licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId"
            );
            
            return licenseByTenantId;
        }
        
        protected static LicenseByTenantIdOld BuildLicenseForTenantWithSimpleLicenseWithinBundleAndLicenseAccessType()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.MultipleLicenseWithinBundle.LicensedBundleId,
                    Name = Apps.MultipleLicenseWithinBundle.Name,
                    Identifier = Apps.MultipleLicenseWithinBundle.Identifier,
                    AppId = Apps.MultipleLicenseWithinBundle.Id,
                    Status = Apps.MultipleLicenseWithinBundle.Status,
                    NumberOfLicenses = Apps.MultipleLicenseWithinBundle.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.MultipleLicenseWithinBundle.AdditionalNumberOfLicenses
                }
            };
            
            var licensedBundleDetails = new List<LicensedBundleDetails>
            {
                new LicensedBundleDetails
                {
                    BundleId = Bundles.Multiple.Id,
                    Identifier = Bundles.Multiple.Identifier,
                    Name = Bundles.Multiple.Name,
                    Status = Bundles.Multiple.Status,
                    NumberOfLicenses = Bundles.Multiple.NumberOfLicenses 
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.SimpleLicenseWithinBundle.Id,
                Tenants.SimpleLicenseWithinBundle.Status,
                Tenants.SimpleLicenseWithinBundle.ExpirationDateTime,
                Tenants.SimpleLicenseWithinBundle.Cnpjs,
                new LicensedTenantDetails(licensedBundleDetails, licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId",
                LicenseConsumeType.Access
            );
            
            return licenseByTenantId;
        }

        protected static LicenseByTenantIdOld BuildLicenseForSingleTenantConfigurationFromJson()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.SingleLicense.LicensedBundleId,
                    Name = Apps.SingleLicense.Name,
                    Identifier = Apps.SingleLicense.Identifier,
                    AppId = Apps.SingleLicense.Id,
                    Status = Apps.SingleLicense.Status,
                    NumberOfLicenses = Apps.SingleLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicense.AdditionalNumberOfLicenses
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.SingleTenantConfigurationFromJson.Id,
                Tenants.SingleTenantConfigurationFromJson.Status,
                Tenants.SingleTenantConfigurationFromJson.ExpirationDateTime,
                Tenants.SingleTenantConfigurationFromJson.Cnpjs,
                new LicensedTenantDetails(new List<LicensedBundleDetails>(), licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId"
            );
            
            return licenseByTenantId;
        }

        protected static LicenseByTenantIdOld BuildLicenseForTwoTenantsConfigurationFromJson()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.SingleLicense.LicensedBundleId,
                    Name = Apps.SingleLicense.Name,
                    Identifier = Apps.SingleLicense.Identifier,
                    AppId = Apps.SingleLicense.Id,
                    Status = Apps.SingleLicense.Status,
                    NumberOfLicenses = Apps.SingleLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicense.AdditionalNumberOfLicenses
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.TwoTenantsConfigurationFromJson.Id,
                Tenants.TwoTenantsConfigurationFromJson.Status,
                Tenants.TwoTenantsConfigurationFromJson.ExpirationDateTime,
                Tenants.TwoTenantsConfigurationFromJson.Cnpjs,
                new LicensedTenantDetails(new List<LicensedBundleDetails>(), licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId"
            );
            
            return licenseByTenantId;
        }

        protected static LicenseByTenantIdOld BuildLicenseForTenantWithWrongHardwareId()
        {
            return new LicenseByTenantIdOld
            {
                HardwareId = "errow",
                Cnpjs = new List<string>
                {
                    "81634257000105"
                },
                Identifier = Guid.Parse("bed286e0-d5e4-11eb-9d92-fc4596fac591"),
                Status = LicensingStatus.Active,
                ExpirationDateTime = null,
                LicenseConsumeType = LicenseConsumeType.Connection,
                LicensedTenantDetails = new LicensedTenantDetails
                {
                    OwnedApps = new List<LicensedAppDetails>
                    {
                        new LicensedAppDetails
                        {
                            Identifier = "App",
                            Name = "BigApp",
                            Status = LicensedAppStatus.AppActive,
                            AppId = Guid.NewGuid(),
                            SoftwareIdentifier = "Software",
                            SoftwareName = "BigSoftware",
                            LicensedBundleId = null,
                            NumberOfLicenses = 10,
                            AdditionalNumberOfLicenses = 0
                        }
                    },
                    OwnedBundles = new List<LicensedBundleDetails>(),
                    AccountDetails = new LicensedAccountDetails
                    {
                        Email = "newell_shields@gmail.com",
                        Status = LicensedAccountStatusEnum.Active,
                        Phone = "41998776513",
                        CnpjCpf = "81634257000105",
                        CompanyName = "CompanyName",
                        TradingName = "TradingName",
                        WebSite = "Website.com.br"
                    }
                }
            };
        }

        protected static LicenseByTenantIdOld BuildLicenseForTenantWithSimpleLicense()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.SingleLicense.LicensedBundleId,
                    Name = Apps.SingleLicense.Name,
                    Identifier = Apps.SingleLicense.Identifier,
                    AppId = Apps.SingleLicense.Id,
                    Status = Apps.SingleLicense.Status,
                    NumberOfLicenses = Apps.SingleLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicense.AdditionalNumberOfLicenses
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.SimpleLicense.Id,
                Tenants.SimpleLicense.Status,
                Tenants.SimpleLicense.ExpirationDateTime,
                Tenants.SimpleLicense.Cnpjs,
                new LicensedTenantDetails(new List<LicensedBundleDetails>(), licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId"
            );
            
            return licenseByTenantId;
        }
        
        protected static LicenseByTenantIdOld BuildLicenseForTenantWithSimpleLicenseConnectionLicenseType()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.SingleLicense.LicensedBundleId,
                    Name = Apps.SingleLicense.Name,
                    Identifier = Apps.SingleLicense.Identifier,
                    AppId = Apps.SingleLicense.Id,
                    Status = Apps.SingleLicense.Status,
                    NumberOfLicenses = Apps.SingleLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicense.AdditionalNumberOfLicenses
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.SimpleLicenseWithConnectionLicenseType.Id,
                Tenants.SimpleLicenseWithConnectionLicenseType.Status,
                Tenants.SimpleLicenseWithConnectionLicenseType.ExpirationDateTime,
                Tenants.SimpleLicenseWithConnectionLicenseType.Cnpjs,
                new LicensedTenantDetails(new List<LicensedBundleDetails>(), licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId",
                Tenants.SimpleLicenseWithConnectionLicenseType.ConsumeType
            );
            
            return licenseByTenantId;
        }
        
        protected static LicenseByTenantIdOld BuildLicenseForTenantWithSimpleLicenseAccessLicenseType()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.MultipleLicense.LicensedBundleId,
                    Name = Apps.MultipleLicense.Name,
                    Identifier = Apps.MultipleLicense.Identifier,
                    AppId = Apps.MultipleLicense.Id,
                    Status = Apps.MultipleLicense.Status,
                    NumberOfLicenses = Apps.MultipleLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.MultipleLicense.AdditionalNumberOfLicenses
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.SimpleLicenseWithAccessLicenseType.Id,
                Tenants.SimpleLicenseWithAccessLicenseType.Status,
                Tenants.SimpleLicenseWithAccessLicenseType.ExpirationDateTime,
                Tenants.SimpleLicenseWithAccessLicenseType.Cnpjs,
                new LicensedTenantDetails(new List<LicensedBundleDetails>(), licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId",
                Tenants.SimpleLicenseWithAccessLicenseType.ConsumeType
            );
            
            return licenseByTenantId;
        }
        
        protected static LicenseByTenantIdOld BuildLicenseForTenantWithAdditionalLicense()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.BundledPlusSingleAdditionalLicense.LicensedBundleId,
                    Name = Apps.BundledPlusSingleAdditionalLicense.Name,
                    Identifier = Apps.BundledPlusSingleAdditionalLicense.Identifier,
                    AppId = Apps.BundledPlusSingleAdditionalLicense.Id,
                    Status = Apps.BundledPlusSingleAdditionalLicense.Status,
                    NumberOfLicenses = Apps.BundledPlusSingleAdditionalLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses
                },
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.BundledWithNoAdditionalLicense.LicensedBundleId,
                    Name = Apps.BundledWithNoAdditionalLicense.Name,
                    Identifier = Apps.BundledWithNoAdditionalLicense.Identifier,
                    AppId = Apps.BundledWithNoAdditionalLicense.Id,
                    Status = Apps.BundledWithNoAdditionalLicense.Status,
                    NumberOfLicenses = Apps.BundledWithNoAdditionalLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.BundledWithNoAdditionalLicense.AdditionalNumberOfLicenses
                },
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.SingleLicense.LicensedBundleId,
                    Name = Apps.SingleLicense.Name,
                    Identifier = Apps.SingleLicense.Identifier,
                    AppId = Apps.SingleLicense.Id,
                    Status = Apps.SingleLicense.Status,
                    NumberOfLicenses = Apps.SingleLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicense.AdditionalNumberOfLicenses
                },
                new LicensedAppDetails
                {
                    Name = Apps.SingleLicenseWithinBundle.Name,
                    Identifier = Apps.SingleLicenseWithinBundle.Identifier,
                    AppId = Apps.SingleLicenseWithinBundle.Id,
                    Status = Apps.SingleLicenseWithinBundle.Status,
                    NumberOfLicenses = Apps.SingleLicenseWithinBundle.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicenseWithinBundle.AdditionalNumberOfLicenses
                }
            };

            var licensedBundleDetails = new List<LicensedBundleDetails>
            {
                new LicensedBundleDetails
                {
                    BundleId = Bundles.Simple.Id,
                    Identifier = Bundles.Simple.Identifier,
                    Name = Bundles.Simple.Name,
                    Status = Bundles.Simple.Status,
                    NumberOfLicenses = Bundles.Simple.NumberOfLicenses 
                }
            };
            
            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.AdditionalLicense.Id,
                Tenants.AdditionalLicense.Status,
                Tenants.AdditionalLicense.ExpirationDateTime,
                Tenants.AdditionalLicense.Cnpjs,
                new LicensedTenantDetails(licensedBundleDetails, licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId"
            );
            
            return licenseByTenantId;
        }
        
        protected static LicenseByTenantIdOld BuildLicenseForTenantWithAdditionalLicenseForAccessType()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.BundledPlusSingleAdditionalLicense.LicensedBundleId,
                    Name = Apps.BundledPlusSingleAdditionalLicense.Name,
                    Identifier = Apps.BundledPlusSingleAdditionalLicense.Identifier,
                    AppId = Apps.BundledPlusSingleAdditionalLicense.Id,
                    Status = Apps.BundledPlusSingleAdditionalLicense.Status,
                    NumberOfLicenses = Apps.BundledPlusSingleAdditionalLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses
                },
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.BundledWithNoAdditionalLicense.LicensedBundleId,
                    Name = Apps.BundledWithNoAdditionalLicense.Name,
                    Identifier = Apps.BundledWithNoAdditionalLicense.Identifier,
                    AppId = Apps.BundledWithNoAdditionalLicense.Id,
                    Status = Apps.BundledWithNoAdditionalLicense.Status,
                    NumberOfLicenses = Apps.BundledWithNoAdditionalLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.BundledWithNoAdditionalLicense.AdditionalNumberOfLicenses
                },
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.SingleLicense.LicensedBundleId,
                    Name = Apps.SingleLicense.Name,
                    Identifier = Apps.SingleLicense.Identifier,
                    AppId = Apps.SingleLicense.Id,
                    Status = Apps.SingleLicense.Status,
                    NumberOfLicenses = Apps.SingleLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicense.AdditionalNumberOfLicenses
                },
                new LicensedAppDetails
                {
                    Name = Apps.SingleLicenseWithinBundle.Name,
                    Identifier = Apps.SingleLicenseWithinBundle.Identifier,
                    AppId = Apps.SingleLicenseWithinBundle.Id,
                    Status = Apps.SingleLicenseWithinBundle.Status,
                    NumberOfLicenses = Apps.SingleLicenseWithinBundle.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicenseWithinBundle.AdditionalNumberOfLicenses
                }
            };

            var licensedBundleDetails = new List<LicensedBundleDetails>
            {
                new LicensedBundleDetails
                {
                    BundleId = Bundles.Simple.Id,
                    Identifier = Bundles.Simple.Identifier,
                    Name = Bundles.Simple.Name,
                    Status = Bundles.Simple.Status,
                    NumberOfLicenses = Bundles.Simple.NumberOfLicenses 
                }
            };
            
            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.AdditionalLicense.Id,
                Tenants.AdditionalLicense.Status,
                Tenants.AdditionalLicense.ExpirationDateTime,
                Tenants.AdditionalLicense.Cnpjs,
                new LicensedTenantDetails(licensedBundleDetails, licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId",
                LicenseConsumeType.Access
            );
            
            return licenseByTenantId;
        }

        protected static LicenseByTenantIdOld BuildLicenseForTenantWithBlockedAppAdditionalLicense()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.BlockedAdditionalLicense.LicensedBundleId,
                    Name = Apps.BlockedAdditionalLicense.Name,
                    Identifier = Apps.BlockedAdditionalLicense.Identifier,
                    AppId = Apps.BlockedAdditionalLicense.Id,
                    Status = Apps.BlockedAdditionalLicense.Status,
                    NumberOfLicenses = Apps.BlockedAdditionalLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.BlockedAdditionalLicense.AdditionalNumberOfLicenses
                }
            };
            
            var licensedBundleDetails = new List<LicensedBundleDetails>
            {
                new LicensedBundleDetails
                {
                    BundleId = Bundles.Simple.Id,
                    Identifier = Bundles.Simple.Identifier,
                    Name = Bundles.Simple.Name,
                    Status = Bundles.Simple.Status,
                    NumberOfLicenses = Bundles.Simple.NumberOfLicenses 
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.BlockedAppAdditionalLicense.Id,
                Tenants.BlockedAppAdditionalLicense.Status,
                Tenants.BlockedAppAdditionalLicense.ExpirationDateTime,
                Tenants.BlockedAppAdditionalLicense.Cnpjs,
                new LicensedTenantDetails(licensedBundleDetails, licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId"
            );
            
            return licenseByTenantId;
        }
        
        protected static LicenseByTenantIdOld BuildLicenseForTenantWithBlockedAppLicense()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.BlockedLicense.LicensedBundleId,
                    Name = Apps.BlockedLicense.Name,
                    Identifier = Apps.BlockedLicense.Identifier,
                    AppId = Apps.BlockedLicense.Id,
                    Status = Apps.BlockedLicense.Status,
                    NumberOfLicenses = Apps.BlockedLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.BlockedLicense.AdditionalNumberOfLicenses
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.BlockedAppLicense.Id,
                Tenants.BlockedAppLicense.Status,
                Tenants.BlockedAppLicense.ExpirationDateTime,
                Tenants.BlockedAppLicense.Cnpjs,
                new LicensedTenantDetails(new List<LicensedBundleDetails>(), licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId"
            );
            
            return licenseByTenantId;
        }
        
        protected static LicenseByTenantIdOld BuildLicenseForTenantWithBlockedLicense()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.SingleLicense.LicensedBundleId,
                    Name = Apps.SingleLicense.Name,
                    Identifier = Apps.SingleLicense.Identifier,
                    AppId = Apps.SingleLicense.Id,
                    Status = Apps.SingleLicense.Status,
                    NumberOfLicenses = Apps.SingleLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicense.AdditionalNumberOfLicenses
                }
            };
            
            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.BlockedLicense.Id,
                Tenants.BlockedLicense.Status,
                Tenants.BlockedLicense.ExpirationDateTime,
                Tenants.BlockedLicense.Cnpjs,
                new LicensedTenantDetails(new List<LicensedBundleDetails>(), licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId"
            );
            
            return licenseByTenantId;
        }
        
        protected static LicenseByTenantIdOld BuildLicenseForTenantDetails()
        {
            var licensedAppDetails = new List<LicensedAppDetails>
            {
                new LicensedAppDetails
                {
                    LicensedBundleId = Apps.SingleLicense.LicensedBundleId,
                    Name = Apps.SingleLicense.Name,
                    Identifier = Apps.SingleLicense.Identifier,
                    AppId = Apps.SingleLicense.Id,
                    Status = Apps.SingleLicense.Status,
                    NumberOfLicenses = Apps.SingleLicense.NumberOfLicenses,
                    AdditionalNumberOfLicenses = Apps.SingleLicense.AdditionalNumberOfLicenses
                }
            };

            var licenseByTenantId = new LicenseByTenantIdOld
            (
                Tenants.LicenseForDetails.Id,
                Tenants.LicenseForDetails.Status,
                Tenants.LicenseForDetails.ExpirationDateTime,
                Tenants.LicenseForDetails.Cnpjs,
                new LicensedTenantDetails(new List<LicensedBundleDetails>(), licensedAppDetails)
                {
                    AccountDetails = new LicensedAccountDetails()
                },
                "hardwareId"
            );
            
            return licenseByTenantId;
        }
    }
}