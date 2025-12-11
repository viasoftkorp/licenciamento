using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Validator;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Validators.LicensingCrud
{
    public class LicensingCrudValidatorTests : LicensingManagementTestBase
    {
        [Fact]
        public async Task MustValidateForCreateSuccessfully()
        {
            // arrange
            var tenantId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var tenantsToInsert = new List<LicensedTenant>
            {
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste1@teste.com.br",
                    LicensedCnpjs = "05397795000137",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste2@teste.com.br",
                    LicensedCnpjs = "98947777000162",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste3@teste.com.br",
                    LicensedCnpjs = "76023375000139",
                    TenantId = tenantId,
                    LicensedCnpjList = {"76023375000139"}
                }
            };
            var tenantToValidate = new LicenseTenantCreateInput
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                Notes = null,
                Status = LicensingStatus.Active,
                AccountId = accountId,
                AdministratorEmail = "teste4@teste.com.br",
                LicensedCnpjs = "34960488000110"
            };

            var account = new Account
            {
                Id = accountId,
                TenantId = tenantId,
                Status = AccountStatus.Active,
                CnpjCpf = "34960488000110"
            };
            
            var accountRepo = ServiceProvider.GetService<IRepository<Account>>();
            var repo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var validator = ActivatorUtilities.CreateInstance<LicensingCrudValidator>(ServiceProvider);

            await accountRepo.InsertAsync(account, true);
            
            foreach (var tenant in tenantsToInsert)
            {
                await repo.InsertAsync(tenant, true);
            }
            
            // act
            var (isValid, output) = await validator.ValidateLicensingForCreate(tenantToValidate);
            
            // assert
            output.Should().BeNull();
            isValid.Should().BeTrue();
        }

        [Fact]
        public async Task MustValidateCreateForDuplicatedIdentifier()
        {
            // arrange
            var tenantId = Guid.NewGuid();
            var tenantsToInsert = new List<LicensedTenant>
            {
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste1@teste.com.br",
                    LicensedCnpjs = "05397795000137",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste2@teste.com.br",
                    LicensedCnpjs = "98947777000162",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste3@teste.com.br",
                    LicensedCnpjs = "76023375000139",
                    TenantId = tenantId,
                    LicensedCnpjList = {"76023375000139"}
                }
            };
            var tenantToValidate = new LicenseTenantCreateInput
            {
                Id = Guid.NewGuid(),
                Identifier = tenantsToInsert[0].Identifier,
                Notes = null,
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = "teste4@teste.com.br",
                LicensedCnpjs = "34960488000110"
            };
            var repo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var validator = ActivatorUtilities.CreateInstance<LicensingCrudValidator>(ServiceProvider);
            
            foreach (var tenant in tenantsToInsert)
            {
                await repo.InsertAsync(tenant, true);
            }
            
            //act
            var (isValid, output) = await validator.ValidateLicensingForCreate(tenantToValidate);
            
            //assert
            isValid.Should().BeFalse();
            output.OperationValidation.Should().Be(OperationValidation.DuplicatedIdentifier);
        }

        [Fact]
        public async Task MustValidateCreateForDuplicatedAccountId()
        {
            // arrange
            var tenantId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var tenantsToInsert = new List<LicensedTenant>
            {
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste1@teste.com.br",
                    LicensedCnpjs = "05397795000137",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = accountId,
                    AdministratorEmail = "teste2@teste.com.br",
                    LicensedCnpjs = "98947777000162",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste3@teste.com.br",
                    LicensedCnpjs = "76023375000139",
                    TenantId = tenantId,
                    LicensedCnpjList = {"76023375000139"}
                }
            };
            var tenantToValidate = new LicenseTenantCreateInput
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                Notes = null,
                Status = LicensingStatus.Active,
                AccountId = accountId,
                AdministratorEmail = "teste4@teste.com.br",
                LicensedCnpjs = "34960488000110"
            };
            var account = new Account
            {
                Id = accountId,
                TenantId = tenantId,
                Status = AccountStatus.Active,
                CnpjCpf = "34960488000110"
            };
            var accountRepo = ServiceProvider.GetService<IRepository<Account>>();
            var repo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var validator = ActivatorUtilities.CreateInstance<LicensingCrudValidator>(ServiceProvider);
            
            await accountRepo.InsertAsync(account, true);

            foreach (var tenant in tenantsToInsert)
            {
                await repo.InsertAsync(tenant, true);
            }
            
            //act
            var (isValid, output) = await validator.ValidateLicensingForCreate(tenantToValidate);
            
            //assert
            isValid.Should().BeFalse();
            output.OperationValidation.Should().Be(OperationValidation.AccountIdAlreadyInUse);
        }

        [Fact]
        public async void MustValidateCreateForDuplicatedAdministratorEmail()
        {
            // arrange
            var tenantId = Guid.NewGuid();
            var tenantsToInsert = new List<LicensedTenant>
            {
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste1@teste.com.br",
                    LicensedCnpjs = "05397795000137",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste2@teste.com.br",
                    LicensedCnpjs = "98947777000162",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste3@teste.com.br",
                    LicensedCnpjs = "76023375000139",
                    TenantId = tenantId,
                    LicensedCnpjList = {"76023375000139"}
                }
            };
            var tenantToValidate = new LicenseTenantCreateInput
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                Notes = null,
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = tenantsToInsert[2].AdministratorEmail,
                LicensedCnpjs = "34960488000110"
            };
            var repo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var validator = ActivatorUtilities.CreateInstance<LicensingCrudValidator>(ServiceProvider);
            
            foreach (var tenant in tenantsToInsert)
            {
                await repo.InsertAsync(tenant, true);
            }
            
            //act
            var (isValid, output) = await validator.ValidateLicensingForCreate(tenantToValidate);
            
            //assert
            isValid.Should().BeFalse();
            output.OperationValidation.Should().Be(OperationValidation.AdministrationEmailAlreadyInUse);
        }

        [Fact]
        public async void MustValidateForUpdateSuccessfully()
        {
            // arrange
            var tenantId = Guid.NewGuid();
            var tenantsToInsert = new List<LicensedTenant>
            {
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste1@teste.com.br",
                    LicensedCnpjs = "05397795000137",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste2@teste.com.br",
                    LicensedCnpjs = "98947777000162",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste3@teste.com.br",
                    LicensedCnpjs = "76023375000139",
                    TenantId = tenantId,
                    LicensedCnpjList = {"76023375000139"}
                }
            };
            var tenantToValidate = new LicenseTenantUpdateInput
            {
                Id = tenantsToInsert[0].Id,
                Identifier = tenantsToInsert[0].Identifier,
                Notes = "new notes for testing",
                Status = LicensingStatus.Active,
                AccountId = tenantsToInsert[0].AccountId,
                AdministratorEmail = "teste4@teste.com.br",
                LicensedCnpjs = "34960488000110"
            };
            var repo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var validator = ActivatorUtilities.CreateInstance<LicensingCrudValidator>(ServiceProvider);
            
            foreach (var tenant in tenantsToInsert)
            {
                await repo.InsertAsync(tenant, true);
            }
            
            //act
            var (isValid, output) = await validator.ValidateLicensingForUpdate(tenantToValidate);
            
            //assert
            isValid.Should().BeTrue();
            output.Should().BeNull();
        }

        [Fact]
        public async void MustValidateUpdateForDuplicatedIdentifier()
        {
            // arrange
            var tenantId = Guid.NewGuid();
            var tenantsToInsert = new List<LicensedTenant>
            {
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste1@teste.com.br",
                    LicensedCnpjs = "05397795000137",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste2@teste.com.br",
                    LicensedCnpjs = "98947777000162",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste3@teste.com.br",
                    LicensedCnpjs = "76023375000139",
                    TenantId = tenantId,
                    LicensedCnpjList = {"76023375000139"}
                }
            };
            var tenantToValidate = new LicenseTenantUpdateInput
            {
                Id = tenantsToInsert[0].Id,
                Identifier = tenantsToInsert[1].Identifier,
                Notes = null,
                Status = LicensingStatus.Active,
                AccountId = tenantsToInsert[0].AccountId,
                AdministratorEmail = "teste4@teste.com.br",
                LicensedCnpjs = "34960488000110"
            };
            var repo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var validator = ActivatorUtilities.CreateInstance<LicensingCrudValidator>(ServiceProvider);
            
            foreach (var tenant in tenantsToInsert)
            {
                await repo.InsertAsync(tenant, true);
            }
            
            //act
            var (isValid, output) = await validator.ValidateLicensingForUpdate(tenantToValidate);
            
            //assert
            isValid.Should().BeFalse();
            output.OperationValidation.Should().Be(OperationValidation.DuplicatedIdentifier);
        }

        [Fact]
        public async void MustValidateUpdateForDuplicatedAccountId()
        {
            // arrange
            var tenantId = Guid.NewGuid();
            var tenantsToInsert = new List<LicensedTenant>
            {
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste1@teste.com.br",
                    LicensedCnpjs = "05397795000137",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste2@teste.com.br",
                    LicensedCnpjs = "98947777000162",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste3@teste.com.br",
                    LicensedCnpjs = "76023375000139",
                    TenantId = tenantId,
                    LicensedCnpjList = {"76023375000139"}
                }
            };
            var tenantToValidate = new LicenseTenantUpdateInput
            {
                Id = tenantsToInsert[0].Id,
                Identifier = tenantsToInsert[0].Identifier,
                Notes = null,
                Status = LicensingStatus.Active,
                AccountId = tenantsToInsert[1].AccountId,
                AdministratorEmail = "teste4@teste.com.br",
                LicensedCnpjs = "34960488000110"
            };
            var repo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var validator = ActivatorUtilities.CreateInstance<LicensingCrudValidator>(ServiceProvider);
            
            foreach (var tenant in tenantsToInsert)
            {
                await repo.InsertAsync(tenant, true);
            }
            
            //act
            var (isValid, output) = await validator.ValidateLicensingForUpdate(tenantToValidate);
            
            //assert
            isValid.Should().BeFalse();
            output.OperationValidation.Should().Be(OperationValidation.AccountIdAlreadyInUse);
        }

        [Fact]
        public async void MustValidateUpdateForDuplicatedAdministratorEmail()
        {
            // arrange
            var tenantId = Guid.NewGuid();
            var tenantsToInsert = new List<LicensedTenant>
            {
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste1@teste.com.br",
                    LicensedCnpjs = "05397795000137",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste2@teste.com.br",
                    LicensedCnpjs = "98947777000162",
                    TenantId = tenantId,
                    LicensedCnpjList = {"05397795000137"}
                },
                new LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste3@teste.com.br",
                    LicensedCnpjs = "76023375000139",
                    TenantId = tenantId,
                    LicensedCnpjList = {"76023375000139"}
                }
            };
            var tenantToValidate = new LicenseTenantUpdateInput
            {
                Id = tenantsToInsert[0].Id,
                Identifier = tenantsToInsert[0].Identifier,
                Notes = null,
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = tenantsToInsert[2].AdministratorEmail,
                LicensedCnpjs = "34960488000110"
            };
            var repo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var validator = ActivatorUtilities.CreateInstance<LicensingCrudValidator>(ServiceProvider);
            
            foreach (var tenant in tenantsToInsert)
            {
                await repo.InsertAsync(tenant, true);
            }
            
            //act
            var (isValid, output) = await validator.ValidateLicensingForUpdate(tenantToValidate);
            
            //assert
            isValid.Should().BeFalse();
            output.OperationValidation.Should().Be(OperationValidation.AdministrationEmailAlreadyInUse);
        }
    }
}