using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DateTimeProvider;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.Identity.Abstractions;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Service;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Event;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Extension;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Validator;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.Command;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.Message;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.App;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.TenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.Services.BundledApps;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicenseServer;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service
{
    // in this class, multiple calls to InvalidateCacheForTenant are made. for these we need to pass the TenantId so many FindAsync are made
    // frontend should be modified to send it
    public class LicensedTenantService: ILicensedTenantService, ITransientDependency
    {
        private readonly IRepository<LicensedBundle> _licensedBundle;
        private readonly IMapper _mapper;
        private readonly IRepository<BundledApp> _bundledApp;
        private readonly IRepository<LicensedApp> _licensedApp;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppRepository _appRepository;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IRepository<Entities.LicensedTenant> _licensedTenant;
        private readonly IServiceBus _serviceBus;
        private readonly ILicenseServerService _licenseServerService;
        private readonly ILicensingCrudValidator _licensingCrudValidator;
        private readonly ILicensedTenantCacheService _cacheService;
        private readonly ICurrentUser _currentUser;
        private readonly IBundleRepository _bundleRepository;
        private readonly IAccountsService _accountsService;
        private readonly IRemoveDuplicatedBundledAppsService _removeDuplicatedBundledAppsService;
        private readonly ILicensedBundleService _licensedBundleService;
        private readonly ILicensedAppService _licensedAppService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILicensedTenantSettingsRepository _licensedTenantSettingsRepository;
            

        public LicensedTenantService(IRepository<LicensedBundle> licensedBundle, IMapper mapper, IRepository<BundledApp> bundledApp, 
            IRepository<LicensedApp> licensedApp, IUnitOfWork unitOfWork, IRepository<Entities.LicensedTenant> licensedTenant, 
            IAppRepository appRepository, ILicenseRepository licenseRepository, IServiceBus serviceBus, ILicenseServerService licenseServerService, 
            ILicensingCrudValidator licensingCrudValidator, ILicensedTenantCacheService cacheService,
            ICurrentUser currentUser, IBundleRepository bundleRepository, IAccountsService accountsService,
            IRemoveDuplicatedBundledAppsService removeDuplicatedBundledAppsService, ILicensedBundleService licensedBundleService, ILicensedAppService licensedAppService,
            IDateTimeProvider dateTimeProvider, IRepository<Entities.LicensedTenantSettings> licensedTenantSettings,
            ILicensedTenantSettingsRepository licensedTenantSettingsRepository)
        {
            _licensedBundle = licensedBundle;
            _mapper = mapper;
            _bundledApp = bundledApp;
            _licensedApp = licensedApp;
            _unitOfWork = unitOfWork;
            _licensedTenant = licensedTenant;
            _appRepository = appRepository;
            _licenseRepository = licenseRepository;
            _serviceBus = serviceBus;
            _licenseServerService = licenseServerService;
            _licensingCrudValidator = licensingCrudValidator;
            _cacheService = cacheService;
            _currentUser = currentUser;
            _bundleRepository = bundleRepository;
            _accountsService = accountsService;
            _removeDuplicatedBundledAppsService = removeDuplicatedBundledAppsService;
            _licensedBundleService = licensedBundleService;
            _licensedAppService = licensedAppService;
            _dateTimeProvider = dateTimeProvider;
            _licensedTenantSettingsRepository = licensedTenantSettingsRepository;
        }
        
        public async Task<LicensedBundleCreateOutput> AddBundleToLicense(LicensedBundleCreateInput input)
        {
            LicensedBundleCreateOutput output;
            if (!HasValidNumberOfLicenses(input.NumberOfLicenses, input.NumberOfTemporaryLicenses, input.ExpirationDateOfTemporaryLicenses))
            {
                return LicensedBundleCreateOutput.Fail(input, OperationValidation.InvalidNumberOfLicenses);
            }

            if (input.LicensingModel == LicensingModels.Named && !input.LicensingMode.HasValue)
            {
                return LicensedBundleCreateOutput.Fail(input, OperationValidation.NoLicensingMode);
            }
            
            var alreadyInLicenses = (await _bundleRepository.GetBundlesAlreadyInLicenses(new List<Guid> {input.BundleId}, new List<Guid> {input.LicensedTenantId})).Any();
            if (alreadyInLicenses)
            {
                output = _mapper.Map<LicensedBundleCreateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
            }
            else
            {
                var bundledApps = await _bundledApp.Where(a => a.BundleId == input.BundleId).ToListAsync();
                var alreadyLicensedApp = await _appRepository.GetAppsAlreadyLicensed(bundledApps.Select(a => a.AppId).ToList(), new List<Guid>{ input.LicensedTenantId });
                var licensedTenantIdentifier = await _licensedTenant.Where(l => l.Id == input.LicensedTenantId).Select(l => l.Identifier).FirstAsync();
                
                using (_unitOfWork.Begin())
                {
                    var newLicensedBundle = await CreateLicensedBundle(input);
                    output = _mapper.Map<LicensedBundleCreateOutput>(newLicensedBundle);

                    foreach (var bundledApp in bundledApps.Where(a => !alreadyLicensedApp.Select(app => app.AppId).Contains(a.AppId)))
                        await AddBundledAppsToLicense(bundledApp.AppId, input);

                    await _unitOfWork.SaveChangesAsync();
                    await PublishLicensingDetailsUpdatedEvent(licensedTenantIdentifier);
                    await _unitOfWork.CompleteAsync();
                }
                
                if (alreadyLicensedApp.Count > 0)
                    output.OperationValidation = OperationValidation.AppAlreadyLicensed;
                await _cacheService.InvalidateCacheForTenant(licensedTenantIdentifier);
            }
            return output;
        }
        
        public async Task<LicenseTenantDeleteOutput> RemoveBundleFromLicense(LicensedBundleDeleteInput input)
        {
            var license = await _licensedTenant.FindAsync(input.LicensedTenantId);
            if (license == null)
                return LicenseTenantDeleteOutput.Fail(OperationValidation.LicenseDoesNotExist);

            var licensedBundle = await _licensedBundle.FirstOrDefaultAsync(l => l.LicensedTenantId == input.LicensedTenantId && l.BundleId == input.BundleId);
            if (licensedBundle == null)
                return LicenseTenantDeleteOutput.Fail(OperationValidation.BundleDoesNotExist);

            using (_unitOfWork.Begin())
            {
                await _licensedBundleService.RemoveLicensedBundleFromLicense(license, licensedBundle);
                await _unitOfWork.SaveChangesAsync();
                await PublishLicensingDetailsUpdatedEvent(license.Identifier);
                await _unitOfWork.CompleteAsync();
            }
            
            await _cacheService.InvalidateCacheForTenant(license.Identifier);

            return new LicenseTenantDeleteOutput
            {
                Success = true
            };
        }

        public async Task<LicensedBundleUpdateOutput> UpdateBundleFromLicense(LicensedBundleUpdateInput input)
        {
            if (!HasValidNumberOfLicenses(input.NumberOfLicenses, input.NumberOfTemporaryLicenses, input.ExpirationDateOfTemporaryLicenses))
            {
                return new LicensedBundleUpdateOutput
                {
                    Status = input.Status,
                    BundleId = input.BundleId,
                    OperationValidation = OperationValidation.InvalidNumberOfLicenses,
                    LicensedTenantId = input.LicensedTenantId,
                    NumberOfLicenses = input.NumberOfLicenses,
                    NumberOfTemporaryLicenses = input.NumberOfTemporaryLicenses,
                    ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses,
                    ExpirationDateTime = input.ExpirationDateTime
                };
            }
            
            if (input.LicensingModel == LicensingModels.Named && !input.LicensingMode.HasValue)
            {
                return new LicensedBundleUpdateOutput
                {
                    Status = input.Status,
                    BundleId = input.BundleId,
                    OperationValidation = OperationValidation.NoLicensingMode,
                    LicensedTenantId = input.LicensedTenantId,
                    NumberOfLicenses = input.NumberOfLicenses,
                    NumberOfTemporaryLicenses = input.NumberOfTemporaryLicenses,
                    ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses,
                    ExpirationDateTime = input.ExpirationDateTime
                };
            }

            var license = await _licensedTenant.FindAsync(input.LicensedTenantId);

            if (license == null)
                return new LicensedBundleUpdateOutput
                {
                    Status = input.Status,
                    BundleId = input.BundleId,
                    OperationValidation = OperationValidation.LicenseDoesNotExist,
                    LicensedTenantId = input.LicensedTenantId,
                    NumberOfLicenses = input.NumberOfLicenses,
                    NumberOfTemporaryLicenses = input.NumberOfTemporaryLicenses,
                    ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses,
                    ExpirationDateTime = input.ExpirationDateTime
                };

            var licensedBundle = await _licensedBundle.FirstOrDefaultAsync(l => l.LicensedTenantId == input.LicensedTenantId && l.BundleId == input.BundleId);

            if (licensedBundle == null)
            {
                return new LicensedBundleUpdateOutput
                {
                    Status = input.Status,
                    BundleId = input.BundleId,
                    OperationValidation = OperationValidation.BundleDoesNotExist,
                    LicensedTenantId = input.LicensedTenantId,
                    NumberOfLicenses = input.NumberOfLicenses,
                    NumberOfTemporaryLicenses = input.NumberOfTemporaryLicenses,
                    ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses,
                    ExpirationDateTime = input.ExpirationDateTime
                };
            }

            LicensedBundle output;

            using (_unitOfWork.Begin())
            {
                output = await _licensedBundleService.UpdateLicensedBundle(license, licensedBundle, input);
                await _unitOfWork.SaveChangesAsync();
                await PublishLicensingDetailsUpdatedEvent(license.Identifier);
                await _unitOfWork.CompleteAsync();
            }
            
            await _cacheService.InvalidateCacheForTenant(license.Identifier);

            return new LicensedBundleUpdateOutput
            {
                Status = output.Status,
                BundleId = output.BundleId,
                OperationValidation = OperationValidation.NoError,
                LicensedTenantId = output.LicensedTenantId,
                NumberOfLicenses = output.NumberOfLicenses,
                NumberOfTemporaryLicenses = output.NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = output.ExpirationDateOfTemporaryLicenses,
                ExpirationDateTime = output.ExpirationDateTime
            };
        }

        public async Task<LicensedAppCreateOutput> AddLooseAppToLicense(LicensedAppCreateInput input)
        {
            if (!HasValidNumberOfLicenses(input.NumberOfLicenses, input.NumberOfTemporaryLicenses,  input.ExpirationDateOfTemporaryLicenses))
            {
                return LicensedAppCreateOutput.Fail(input, OperationValidation.InvalidNumberOfLicenses);
            }

            if (input.LicensingModel == LicensingModels.Named && !input.LicensingMode.HasValue)
            {
                return LicensedAppCreateOutput.Fail(input, OperationValidation.NoLicensingMode);
            }
            
            if (await IsAppAlreadyLicensed(input.LicensedTenantId, input.AppId))
            {
                var output = _mapper.Map<LicensedAppCreateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
                return output;
            }
            
            var licensedTenantIdentifier = await _licensedTenant
                .Where(l => l.Id == input.LicensedTenantId)
                .Select(l => l.Identifier)
                .FirstAsync();
            
            LicensedApp newLicensedApp;
            using (_unitOfWork.Begin())
            {
                newLicensedApp = await CreateNewLicensedApp(input);
                // We need to save changes to be able to publish these changes
                // This will change in the future, when the Rebus team publishes a fix
                await _unitOfWork.SaveChangesAsync();
                await PublishLicensingDetailsUpdatedEvent(licensedTenantIdentifier);
                await _unitOfWork.CompleteAsync();
            }
            await _cacheService.InvalidateCacheForTenant(licensedTenantIdentifier);
            return _mapper.Map<LicensedAppCreateOutput>(newLicensedApp);
        }

        public async Task<RemoveAppFromLicenseOutput> RemoveAppFromLicense(LicensedAppDeleteInput input)
        {
            var licensedTenant = await _licensedTenant.FindAsync(input.LicensedTenantId);

            if (licensedTenant == null)
                return RemoveAppFromLicenseOutput.Fail(OperationValidation.NoTenantWithSuchId);

            var licensedApp = await _licensedApp.FirstOrDefaultAsync(a => a.AppId == input.AppId && a.LicensedTenantId == licensedTenant.Id);
            
            if (licensedApp == null)
                return RemoveAppFromLicenseOutput.Fail(OperationValidation.NoLicensedAppWithSuchId);
            
            if (await _licenseRepository.CheckIfLicensedAppIsDefault(input.AppId))
                return RemoveAppFromLicenseOutput.Fail(OperationValidation.CantRemoveFromLicenseDefaultApp);
            
            using (_unitOfWork.Begin())
            {
                await _licensedAppService.RemoveLicensedApp(licensedTenant, licensedApp);
                await _unitOfWork.SaveChangesAsync();
                await PublishLicensingDetailsUpdatedEvents(new List<Guid>
                {
                    licensedTenant.Identifier
                });
                await _cacheService.InvalidateCacheForTenants(new List<Guid>
                {
                    licensedTenant.Identifier
                });
                await _unitOfWork.CompleteAsync();
            }
            
            return new RemoveAppFromLicenseOutput { Success = true };
        }

        public async Task<List<Guid>> RemoveAppsFromLicenses(List<LicensedApp> licensedAppToDeleteFromLicenses, Dictionary<Guid, Guid> licensedTenantIdToIdentifier)
        {
            var licenseTenantIdentifiers = new List<Guid>();

            foreach (var licensedAppToDelete in licensedAppToDeleteFromLicenses)
            {
                var licenseIdentifier = licensedTenantIdToIdentifier[licensedAppToDelete.LicensedTenantId];
                if (!licenseTenantIdentifiers.Exists(id => id == licenseIdentifier))
                {
                    licenseTenantIdentifiers.Add(licenseIdentifier);
                }

                await _licensedApp.DeleteAsync(licensedAppToDelete);
            }
            await _unitOfWork.SaveChangesAsync();
            await PublishLicensingDetailsUpdatedEvents(licenseTenantIdentifiers);
            await _cacheService.InvalidateCacheForTenants(licenseTenantIdentifiers);

            return licenseTenantIdentifiers;
        }

        public async Task<LicensedAppUpdateOutput> UpdateBundledAppFromLicense(LicensedAppUpdateInput input)
        {
            var licensedApp = await _licensedApp.FirstOrDefaultAsync(la => la.LicensedTenantId == input.LicensedTenantId && la.AppId == input.AppId);
            if (licensedApp == null)
            {
                var output = _mapper.Map<LicensedAppUpdateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
                return output;
            }
            
            var licensedTenantIdentifier = await _licensedTenant
                .Where(l => l.Id == input.LicensedTenantId).Select(l => l.Identifier)
                .FirstAsync();
            
            licensedApp.AdditionalNumberOfLicenses = input.AdditionalNumberOfLicenses;
            using (_unitOfWork.Begin())
            {
                await _licensedApp.UpdateAsync(licensedApp);
                await _unitOfWork.SaveChangesAsync();
                await PublishLicensingDetailsUpdatedEvent(licensedTenantIdentifier);
                await _unitOfWork.CompleteAsync();
            }
            
            await _cacheService.InvalidateCacheForTenant(licensedTenantIdentifier);
            return _mapper.Map<LicensedAppUpdateOutput>(licensedApp);
        }
        
        public async Task<LicensedAppUpdateOutput> UpdateLooseAppFromLicense(LicensedAppUpdateInput input)
        {
            if (!HasValidNumberOfLicenses(input.NumberOfLicenses, input.NumberOfTemporaryLicenses,
                input.ExpirationDateOfTemporaryLicenses))
                return LicensedAppUpdateOutput.Fail(OperationValidation.InvalidNumberOfLicenses, input);

            if (input.LicensingModel == LicensingModels.Named && !input.LicensingMode.HasValue)
                return LicensedAppUpdateOutput.Fail(OperationValidation.NoLicensingMode, input);

            var licensedTenant = await _licensedTenant.FindAsync(input.LicensedTenantId);

            if (licensedTenant == null)
                return LicensedAppUpdateOutput.Fail(OperationValidation.NoTenantWithSuchId, input);

            var licensedApp = await _licensedApp.FirstOrDefaultAsync(a => a.LicensedBundleId == null && a.LicensedTenantId == licensedTenant.Id && a.AppId == input.AppId);
            
            if (licensedApp == null)
                return LicensedAppUpdateOutput.Fail(OperationValidation.DuplicatedIdentifier, input);

            if (input.ExpirationDateTime.HasValue)
            {
                var currentDate = _dateTimeProvider.UtcNow().Date;
                if (input.ExpirationDateTime.Value.Date < currentDate)
                    return LicensedAppUpdateOutput.Fail(OperationValidation.InvalidDate, input);   
            }

            LicensedApp result;
            
            using (_unitOfWork.Begin())
            {
                result = await _licensedAppService.UpdateLicensedApp(licensedTenant, licensedApp, input);
                // Mandamos para o banco as alterações porque a publicação do evento de Tenant alterado abaixo busca o licenciamento novamente do banco
                await _unitOfWork.SaveChangesAsync();
                
                await PublishLicensingDetailsUpdatedEvent(licensedTenant.Identifier);
                await _unitOfWork.CompleteAsync();
            }
            
            await _cacheService.InvalidateCacheForTenant(licensedTenant.Identifier);
            
            return LicensedAppUpdateOutput.Success(result);
        }
        
        public async Task<LicenseTenantCreateOutput> CreateNewTenantLicensing(LicenseTenantCreateInput input, bool shouldUseUnitOfWork)
        {
            var (isValid, output) = await _licensingCrudValidator.ValidateLicensingForCreate(input);
            if (!isValid)
            {
                return output;
            }
            
            var newLicense = await CreateNewLicenseAndAssignDefaultApps(input, shouldUseUnitOfWork);
            await _cacheService.InvalidateCacheForTenant(newLicense.Identifier);
            output = _mapper.Map<LicenseTenantCreateOutput>(newLicense);
            return output;
        }
        
        public async Task<DeleteLicenseOutput> DeleteTenantLicensing(Guid id)
        {
            if (await _licenseRepository.CheckIfLicensedTenantExists(id))
            {
                var licensedTenant = await _licensedTenant.FindAsync(id);
                await _cacheService.InvalidateCacheForTenant(licensedTenant.Identifier);
                await DeleteAppsFromLicenseAndDeleteLicense(licensedTenant);
                return new DeleteLicenseOutput { Success = true };
            }
            
            return new DeleteLicenseOutput
            {
                Success = false,
                OperationValidation = OperationValidation.LicenseDoesNotExist
            };
        }

        public async Task<LicenseTenantUpdateOutput> UpdateTenantLicensing(LicenseTenantUpdateInput input, LicensedTenantSagaInfo sagaInfo = null)
        {
            var (isValid, output) = await _licensingCrudValidator.ValidateLicensingForUpdate(input);
            if (!isValid)
            {
                return output;
            }
           
            var license = await _licensedTenant.FindAsync(input.Id);

            var didAdminEmailChange = license.HasAdministratorEmailChanged(input.AdministratorEmail);

            var oldAdministratorEmail = license.AdministratorEmail;

            var shouldPublishLicensingDetailsUpdatedEvent = license.HasLicensedCnpjsChanged(input.LicensedCnpjs) || license.HasLicensingStatusChanged(input.Status); 
            
            license.Identifier = input.Identifier;
            license.Notes = Encoding.UTF8.GetBytes(input.Notes);
            license.Status = input.Status;
            license.AccountId = input.AccountId;
            license.AdministratorEmail = input.AdministratorEmail;
            license.HardwareId = input.HardwareId;
            license.LicensedCnpjs = input.LicensedCnpjs;
            license.ExpirationDateTime = input.ExpirationDateTime;
            license.LicenseConsumeType = input.LicenseConsumeType;
            license.SagaInfo = sagaInfo != null ? Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sagaInfo)) : license.SagaInfo;     

            Entities.LicensedTenant updatedLicense;

            var licensedTenantUpdatedCommand = new LicensedTenantUpdatedCommand
            {
                Identifier = input.Identifier,
                LicensedTenantId = input.Id,
                Status = input.Status,
                AccountId = input.AccountId,
                AdministratorEmail = input.AdministratorEmail,
                LicensedCnpjs = input.LicensedCnpjs,
                ExpirationDateTime = input.ExpirationDateTime,
                LicenseConsumeType = input.LicenseConsumeType,
                HardwareId = input.HardwareId
            };
            
            using(_unitOfWork.Begin())
            {
                updatedLicense = await _licensedTenant.UpdateAsync(license);
                await _unitOfWork.SaveChangesAsync();
                await _serviceBus.SendLocal(licensedTenantUpdatedCommand);
                
                //O licenciamento será salvo mesmo que o email de administrador não seja válido
                //Uma Saga para toda a lógica de criação e remoção de um licenciamento é uma possível solução 
                if (didAdminEmailChange)
                {
                    await _serviceBus.SendLocal(new UpdateAdministratorEmailCommand
                    {
                        NewEmail = input.AdministratorEmail,
                        OldEmail = oldAdministratorEmail,
                        TenantId = updatedLicense.Identifier
                    });
                }

                if (shouldPublishLicensingDetailsUpdatedEvent)
                {
                    await PublishLicensingDetailsUpdatedEvent(input.Identifier);
                }
                
                await _unitOfWork.CompleteAsync();
            }
            
            output = _mapper.Map<LicenseTenantUpdateOutput>(updatedLicense);
            return output;
        }

        public async Task<LicensedBundle> CreateLicensedBundle(LicensedBundleCreateInput input)
        {
            var licensedBundle = _mapper.Map<LicensedBundle>(input);
            return await _licensedBundle.InsertAsync(licensedBundle);
        }
        public async Task<LicensedApp> AddBundledAppsToLicense(Guid appId, LicensedBundleCreateInput input)
        {
            var licensedApp = new LicensedApp
            {
                LicensedTenantId = input.LicensedTenantId,
                Status = (LicensedAppStatus) input.Status,
                LicensedBundleId = input.BundleId,
                NumberOfLicenses = input.NumberOfLicenses,
                AdditionalNumberOfLicenses = 0,
                AppId = appId,
                Id = Guid.NewGuid(),
                NumberOfTemporaryLicenses = input.NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses,
                ExpirationDateTime = input.ExpirationDateTime,
                LicensingModel = input.LicensingModel,
                LicensingMode = input.LicensingMode
            };
            return await _licensedApp.InsertAsync(licensedApp);
        }

        public async Task<LicensedApp> CreateNewLicensedApp(LicensedAppCreateInput input)
        {
            var licensedApp = _mapper.Map<LicensedApp>(input);
            return await _licensedApp.InsertAsync(licensedApp);
        }

        private async Task<bool> IsAppAlreadyLicensed(Guid licensedTenantId, Guid appId)
        {
            return await _licensedApp.AnyAsync(la => la.LicensedTenantId == licensedTenantId && la.AppId == appId);
        }
        
        private async Task<Entities.LicensedTenant> InsertLicensedTenant(List<App> defaultApps, Entities.LicensedTenant license, LicenseTenantCreateInput input, 
            List<BundledAppsOutput> listOfBundledIdAndApp)
        {
            var newLicense = await _licensedTenant.InsertAsync(license);
            foreach (var app in defaultApps)
            {
                await CreateNewLicensedApp(new LicensedAppCreateInput
                {
                    AppId = app.Id,
                    LicensedTenantId = input.Id,
                    Status = LicensedAppStatus.AppActive,
                    NumberOfLicenses = int.MaxValue,
                    AdditionalNumberOfLicenses = 0
                });
            }

            if (input.BundleIds is { Count: > 0 })
            {
                foreach (var bundle in input.BundleIds)
                {
                    var licensedBundleCreateInput = new LicensedBundleCreateInput
                    {
                        Status = LicensedBundleStatus.BundleActive,
                        BundleId = bundle,
                        LicensedTenantId = input.Id,
                        NumberOfLicenses = input.NumberOfLicenses,
                        NumberOfTemporaryLicenses = 0,
                        ExpirationDateOfTemporaryLicenses = null
                    };
                    await CreateLicensedBundle(licensedBundleCreateInput);
                }

                var licensedApps = listOfBundledIdAndApp.Select(ids => new LicensedApp
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedAppStatus.AppActive,
                    AppId = ids.AppId,
                    NumberOfLicenses = input.NumberOfLicenses,
                    ExpirationDateOfTemporaryLicenses = null,
                    NumberOfTemporaryLicenses = 0,
                    AdditionalNumberOfLicenses = 0,
                    LicensedBundleId = ids.BundleId,
                    LicensedTenantId = input.Id
                });

                foreach (var licensedApp in licensedApps)
                {
                    await _licensedApp.InsertAsync(licensedApp);
                }
            }

            return newLicense;
        }

        private async Task<Entities.LicensedTenant> CreateNewLicenseAndAssignDefaultApps(LicenseTenantCreateInput input, bool shouldUseUnitOfWork)
        {
            Entities.LicensedTenant newLicense;
            List<BundledAppsOutput> listOfBundledIdAndApp = new List<BundledAppsOutput>();
            
            //O status do licenciamento será sempre salvo como Blocked e será atualizado para Active quando AdministratorEmailSaga for finalizada
            input.Status = LicensingStatus.Blocked;
            
            var license = new Entities.LicensedTenant
            {
                Id = input.Id,
                Identifier = input.Identifier,
                Notes = Encoding.UTF8.GetBytes(input.Notes ?? String.Empty),
                Status = input.Status,
                AccountId = input.AccountId,
                AdministratorEmail = input.AdministratorEmail,
                HardwareId = input.HardwareId,
                LicensedCnpjs = input.LicensedCnpjs,
                LicenseConsumeType = input.LicenseConsumeType,
                ExpirationDateTime = input.ExpirationDateTime
            };

            var defaultApps = await _appRepository.GetAllDefaultApps();
            var accountName = await _accountsService.GetAccountCompanyName(license.AccountId);

            if (input.BundleIds != null && input.BundleIds.Count > 0)
            {
                var defaultAppsIds = defaultApps.Select(a => a.Id).ToList();
                listOfBundledIdAndApp = await _removeDuplicatedBundledAppsService.GetBundledApps(input.BundleIds, defaultAppsIds);
            }
            
            var licensedTenantCreated = new LicensedTenantCreated
            {
                Id = license.Id,
                AdministratorEmail = license.AdministratorEmail,
                TenantId = license.Identifier,
                Status = license.Status,
                AccountName = accountName
            };
            var licensedTenantCreatedViewCommand = new LicensedTenantCreatedCommand
            {
                Identifier = license.Identifier,
                Status = license.Status,
                AccountId = license.AccountId,
                AdministratorEmail = license.AdministratorEmail,
                LicensedCnpjs = license.LicensedCnpjs,
                LicensedTenantId = license.Id,
                LicenseConsumeType = input.LicenseConsumeType,
                HardwareId = license.HardwareId
            };

            var createNewLicensingCommand = new CreateNewLicensingCommand
            {
                Email = license.AdministratorEmail,
                TenantId = license.Identifier,
                AdministratorUserId = input.AdministratorUserId
            };

            // TODO remove this when SDKBACK-194 is completed
            // Necesssary only when the db is empty, due to not having a userId.
            if (_currentUser.Id == Guid.Empty)
                _currentUser.Id = Guid.NewGuid();
            
            if (shouldUseUnitOfWork)
            {
                using (_unitOfWork.Begin())
                {
                    newLicense = await SaveAndPublishEvent(input, defaultApps, license, listOfBundledIdAndApp, licensedTenantCreatedViewCommand, 
                        licensedTenantCreated, createNewLicensingCommand);
                    await _unitOfWork.CompleteAsync();
                }    
            }
            else
            {
                newLicense = await SaveAndPublishEvent(input, defaultApps, license, listOfBundledIdAndApp, licensedTenantCreatedViewCommand, 
                    licensedTenantCreated, createNewLicensingCommand);
            }
            
            await PublishLicensingDetailsUpdatedEvent(newLicense.Identifier);
            
            return newLicense;
        }

        private async Task<Entities.LicensedTenant> SaveAndPublishEvent(LicenseTenantCreateInput input, List<App> defaultApps, Entities.LicensedTenant license,
            List<BundledAppsOutput> listOfBundledIdAndApp, LicensedTenantCreatedCommand licensedTenantCreatedViewCommand,
            LicensedTenantCreated licensedTenantCreated, CreateNewLicensingCommand createNewLicensingCommand)
        {
            var newLicense = await InsertLicensedTenant(defaultApps, license, input, listOfBundledIdAndApp);
            await _licensedTenantSettingsRepository.AddLicensedTenantSettings(newLicense.Identifier);
            await _serviceBus.SendLocal(licensedTenantCreatedViewCommand);
            await _serviceBus.Publish(licensedTenantCreated);
            await _serviceBus.SendLocal(createNewLicensingCommand);
            return newLicense;
        }

        private async Task DeleteAppsFromLicenseAndDeleteLicense(Entities.LicensedTenant licensedTenant)
        {
            var licensedApps = await _licenseRepository.GetAllLicensedAppsFromTenantId(licensedTenant.Id);
            var licensedBundles = await _licenseRepository.GetAllLicensedBundlesFromTenantId(licensedTenant.Id);

            using (_unitOfWork.Begin())
            {
                foreach (var app in licensedApps)
                    await _licensedApp.DeleteAsync(app);
                
                foreach (var bundle in licensedBundles)
                    await _licensedBundle.DeleteAsync(bundle);

                await _licensedTenantSettingsRepository.RemoveLicensedTenantSettings(licensedTenant.Identifier);
                
                await _licensedTenant.DeleteAsync(licensedTenant);
                await _serviceBus.SendLocal(new DeleteLicensedTenantCommand { LicensedTenantId = licensedTenant.Id });
                await _serviceBus.Publish(new LicensedTenantDeletedMessage { LicensedTenantId = licensedTenant.Id });
                
                await _unitOfWork.CompleteAsync();
            }
        }

        private bool HasValidNumberOfLicenses(int numberOfLicenses, int numberOfTemporaryLicenses, DateTime? expirationDateOfTemporaryLicenses)
        {
            if (expirationDateOfTemporaryLicenses.HasValue && expirationDateOfTemporaryLicenses.Value.Date >= DateTime.UtcNow.Date)
            {
                return numberOfLicenses + numberOfTemporaryLicenses > 0;
            }
            return numberOfLicenses > 0;
        }


        public async Task PublishLicensingDetailsUpdatedEvent(Guid licensedTenantIdentifier)
        {
            var licenseByTenantId = await _licenseServerService.GetLicenseByIdentifier(licensedTenantIdentifier);
            await PublishDetails(licenseByTenantId, licensedTenantIdentifier);
        }
        
        public async Task PublishLicensingDetailsUpdatedEvents(List<Guid> licensedTenantIdentifier)
        {
            var licenseByTenantIds = await _licenseServerService.GetLicensesByIdentifiers(licensedTenantIdentifier);
            foreach (var licenseByTenantId in licenseByTenantIds)
            {
                await PublishDetails(licenseByTenantId, licenseByTenantId.Identifier);
            }
        }

        public async Task<NamedUserBundleLicenseOutput> AddNamedUserToLicensedBundle(Guid licensedTenantId, Guid licensedBundleId,
            CreateNamedUserBundleLicenseInput input)
        {
            var licensedTenant = await _licensedTenant.FindAsync(licensedTenantId);

            if (licensedTenant == null)
                return new NamedUserBundleLicenseOutput
                {
                    OperationValidation = OperationValidation.NoTenantWithSuchId
                };

            NamedUserBundleLicenseOutput namedUser;
            
            using (_unitOfWork.Begin())
            {
                namedUser = await _licensedBundleService.AddNamedUserToLicensedBundle(licensedTenant, licensedBundleId, input);
                if (namedUser.OperationValidation == OperationValidation.NoError)
                {
                    await _unitOfWork.SaveChangesAsync();
                    await PublishLicensingDetailsUpdatedEvent(licensedTenant.Identifier);
                    await _unitOfWork.CompleteAsync();
                }
            }

            return namedUser;
        }

        public async Task<UpdateNamedUsersFromBundleOutput> UpdateNamedUsersFromBundle(Guid licensedTenantId,
            Guid licensedBundleId, UpdateNamedUserBundleLicenseInput input, Guid namedUserId)
        {
            var licensedTenant = await _licensedTenant.FindAsync(licensedTenantId);

            if (licensedTenant == null)
                return new UpdateNamedUsersFromBundleOutput
                {
                    Success = false,
                    ValidationCode = UpdateNamedUsersFromBundleValidationCode.NoLicensedTenant
                };

            UpdateNamedUsersFromBundleOutput namedUser;
            
            using (_unitOfWork.Begin())
            {
                namedUser = await _licensedBundleService.UpdateNamedUsersFromBundle(licensedTenant, licensedBundleId, input, namedUserId);

                if (namedUser.ValidationCode == UpdateNamedUsersFromBundleValidationCode.NoError)
                {
                    await _unitOfWork.SaveChangesAsync();
                    await PublishLicensingDetailsUpdatedEvent(licensedTenant.Identifier);
                    await _unitOfWork.CompleteAsync();
                }
            }

            return namedUser;
        }
        
        public async Task<RemoveNamedUserFromBundleOutput> RemoveNamedUserFromBundle(Guid licensedTenantId, Guid licensedBundleId, Guid namedUserId)
        {
            var licensedTenant = await _licensedTenant.FindAsync(licensedTenantId);

            if (licensedTenant == null)
                return new RemoveNamedUserFromBundleOutput
                {
                    Success = false,
                    ValidationCode = RemoveNamedUserFromBundleValidationCode.NoLicensedTenant
                };

            RemoveNamedUserFromBundleOutput namedUser;
            
            using (_unitOfWork.Begin())
            {
                namedUser = await _licensedBundleService.RemoveNamedUserFromBundle(licensedTenant, licensedBundleId, namedUserId);

                if (namedUser.ValidationCode == RemoveNamedUserFromBundleValidationCode.NoError)
                {
                    await _unitOfWork.SaveChangesAsync();
                    await PublishLicensingDetailsUpdatedEvent(licensedTenant.Identifier);
                    await _unitOfWork.CompleteAsync();
                }
            }

            return namedUser;
        }

        public async Task<GetNamedUserFromBundleOutput> GetNamedUserFromBundle(Guid licensedTenantId,
            Guid licensedBundleId, GetAllNamedUserBundleInput input)
        {
            var licensedTenant = await _licensedTenant.FindAsync(licensedTenantId);

            if (licensedTenant == null)
                return new GetNamedUserFromBundleOutput
                {
                    NamedUserFromBundleValidationCode = GetNamedUserFromBundleValidationCode.NoLicensedTenant
                };

            return await _licensedBundleService.GetNamedUserFromBundle(licensedTenant, licensedBundleId, input);
        }

        public async Task<GetNamedUserFromLicensedAppOutput> GetNamedUserFromLicensedApp(Guid licensedTenantId,
            Guid licensedAppId, GetAllNamedUserAppInput input)
        {
            var licensedTenant = await _licensedTenant.FindAsync(licensedTenantId);

            if (licensedTenant == null)
                return new GetNamedUserFromLicensedAppOutput
                {
                    ValidationCode = GetNamedUserFromLicensedAppValidationCode.NoLicensedTenant
                };

            return await _licensedAppService.GetNamedUserFromLicensedApp(licensedTenant, licensedAppId, input);
        }

        public async Task<AddNamedUserToLicensedAppOutput> AddNamedUserToLicensedApp(Guid licensedTenantId, Guid licensedAppId, AddNamedUserToLicensedAppInput input)
        {
            var licensedTenant = await _licensedTenant.FindAsync(licensedTenantId);

            if (licensedTenant == null)
                return new AddNamedUserToLicensedAppOutput
                {
                    ValidationCode = AddNamedUserToLicensedAppValidationCode.NoLicensedTenant
                };

            AddNamedUserToLicensedAppOutput namedUser;
            
            using (_unitOfWork.Begin())
            {
                namedUser = await _licensedAppService.AddNamedUserToLicensedApp(licensedTenant, licensedAppId, input);
                if (namedUser.ValidationCode == AddNamedUserToLicensedAppValidationCode.NoError)
                {
                    await _unitOfWork.SaveChangesAsync();
                    await PublishLicensingDetailsUpdatedEvent(licensedTenant.Identifier);
                    await _unitOfWork.CompleteAsync();
                }
            }

            return namedUser;
        }

        public async Task<UpdateNamedUsersFromAppOutput> UpdateNamedUsersFromApp(Guid licensedTenantId,
            Guid licensedAppId, UpdateNamedUsersFromAppInput input, Guid namedUserId)
        {
            var licensedTenant = await _licensedTenant.FindAsync(licensedTenantId);

            if (licensedTenant == null)
                return new UpdateNamedUsersFromAppOutput
                {
                    ValidationCode = UpdateNamedUserAppLicenseValidationCode.NoLicensedTenant
                };

            UpdateNamedUsersFromAppOutput namedUser;
            
            using (_unitOfWork.Begin())
            {
                namedUser = await _licensedAppService.UpdateNamedUsersFromApp(licensedTenant, licensedAppId, input, namedUserId);
                if (namedUser.ValidationCode == UpdateNamedUserAppLicenseValidationCode.NoError)
                {
                    await _unitOfWork.SaveChangesAsync();
                    await PublishLicensingDetailsUpdatedEvent(licensedTenant.Identifier);
                    await _unitOfWork.CompleteAsync();
                }
            }

            return namedUser;
        }

        public async Task<DeleteNamedUsersFromAppOutput> DeleteNamedUsersFromApp(Guid licensedTenantId,
            Guid licensedAppId, Guid namedUserId)
        {
            var licensedTenant = await _licensedTenant.FindAsync(licensedTenantId);

            if (licensedTenant == null)
                return new DeleteNamedUsersFromAppOutput
                {
                    ValidationCode = DeleteNamedUsersFromAppValidationCode.NoLicensedTenant
                };

            DeleteNamedUsersFromAppOutput namedUser;
            
            using (_unitOfWork.Begin())
            {
                namedUser = await _licensedAppService.DeleteNamedUsersFromApp(licensedTenant, licensedAppId, namedUserId);
                if (namedUser.ValidationCode == DeleteNamedUsersFromAppValidationCode.NoError)
                {
                    await _unitOfWork.SaveChangesAsync();
                    await PublishLicensingDetailsUpdatedEvent(licensedTenant.Identifier);
                    await _unitOfWork.CompleteAsync();
                }
            }

            return namedUser;
        }

        private async Task PublishDetails(LicenseByIdentifier licenseByIdentifier, Guid identifier)
        {
            var licensingDetailsUpdated = new LicensingDetailsUpdated
            {
                TenantId = identifier,
                UpdatedDateTime = DateTime.UtcNow,
                LicenseByIdentifier = licenseByIdentifier
            };
            await _serviceBus.Publish(licensingDetailsUpdated);
        }
    }
}