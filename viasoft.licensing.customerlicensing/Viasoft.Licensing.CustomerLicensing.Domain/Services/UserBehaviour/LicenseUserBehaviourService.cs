using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.CustomerLicensing.Domain.Consts;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserApp;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.UserBehaviour;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.UserBehaviour
{
    public class LicenseUserBehaviourService : ILicenseUserBehaviourService, ITransientDependency
    {
        private readonly IRepository<Entities.LicenseUsageInRealTime> _licenseUsageInRealTime;
        private readonly IMapper _mapper;
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        
        public LicenseUserBehaviourService(IRepository<Entities.LicenseUsageInRealTime> licenseUsageInRealTime, IMapper mapper, IApiClientCallBuilder apiClientCallBuilder)
        {
            _licenseUsageInRealTime = licenseUsageInRealTime;
            _mapper = mapper;
            _apiClientCallBuilder = apiClientCallBuilder;
        }

        public async Task<PagedResultDto<LicenseUserBehaviourOutput>> GetUsersBehaviour(GetAllLicenseUserBehaviour input)
        {
            var query = GetUsersBehaviourQuery(input);

            var totalCount = await query.CountAsync();

            query = query.PageBy(input.SkipCount, input.MaxResultCount);

            var items = _mapper.Map<List<LicenseUserBehaviourOutput>>(await query.ToListAsync());
            
            var output = new PagedResultDto<LicenseUserBehaviourOutput>
            {
                Items = items,
                TotalCount = totalCount
            };

            return output;
        }
        

        public async Task<PagedResultDto<LicenseUserBehaviourNamedOnlineOutput>> GetUserBehaviourNamedOnline(
            string productIdentifier, GetUserBehaviorFromProductInput input)
        {
            var namedUsers =
                input.ProductType == ProductType.LicensedBundle
                    ? await GetNamedUsersFromBundle(input.LicensedTenantId, input.ProductId)
                    : await GetNamedUsersFromApp(input.LicensedTenantId, input.ProductId);

            var usersBehaviour = await GetUsersBehaviourNamedOnline(productIdentifier, input);

            var resultItems = new List<LicenseUserBehaviourNamedOnlineOutput>();
            var usersOnline = new HashSet<string>();

            foreach (var user in usersBehaviour)
            {
                if (namedUsers.ContainsKey(user.User))
                {
                    usersOnline.Add(user.User);
                    var userBehaviourNamedOnline = new LicenseUserBehaviourNamedOnlineOutput()
                    {
                        LicensingIdentifier = user.LicensingIdentifier,
                        AppIdentifier = user.AppIdentifier,
                        AppName = user.AppName,
                        BundleIdentifier = user.BundleIdentifier,
                        BundleName = user.BundleName,
                        SoftwareName = user.SoftwareName,
                        SoftwareIdentifier = user.SoftwareIdentifier,
                        User = user.User,
                        SoftwareVersion = user.SoftwareVersion,
                        HostName = user.HostName,
                        HostUser = user.HostUser,
                        LocalIpAddress = user.LocalIpAddress,
                        Language = user.Language,
                        OsInfo = user.OsInfo,
                        BrowserInfo = user.BrowserInfo,
                        DatabaseName = user.DatabaseName,
                        StartTime = user.StartTime,
                        LastUpdate = user.LastUpdate,
                        AccountName = user.AccountName,
                        Domain = user.Domain,
                        AccessDuration = user.AccessDuration,
                        Status = UserBehaviourStatus.Online
                    };
                    resultItems.Add(userBehaviourNamedOnline);
                }
            }

            foreach (var namedUser in namedUsers)
            {
                if (!usersOnline.Contains(namedUser.Key))
                {
                    var userBehaviourNamedOnline = new LicenseUserBehaviourNamedOnlineOutput()
                    {
                        User = namedUser.Key,
                        Status = UserBehaviourStatus.Offline,
                        BundleIdentifier = usersBehaviour.Any() ? usersBehaviour.First().BundleIdentifier : string.Empty,
                        SoftwareName = usersBehaviour.Any() ? usersBehaviour.First().SoftwareName : string.Empty,
                        SoftwareIdentifier = usersBehaviour.Any() ? usersBehaviour.First().SoftwareIdentifier : string.Empty
                    };
                    resultItems.Add(userBehaviourNamedOnline);
                }
            }

            var totalCount = resultItems.Count;
            
            resultItems = resultItems.AsQueryable()
                .ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting)
                .PageBy(input.SkipCount, input.MaxResultCount)
                .ToList();

            
            return new PagedResultDto<LicenseUserBehaviourNamedOnlineOutput>()
            {
                TotalCount = totalCount,
                Items = resultItems
            };
        }

        public async Task<PagedResultDto<LicenseUserBehaviourNamedOfflineOutput>> GetUserBehaviourNamedOffline(
            string productIdentifier, GetUserBehaviorFromProductInput input)
        {
            var namedUsers =
                input.ProductType == ProductType.LicensedBundle
                    ? await GetNamedUsersFromBundle(input.LicensedTenantId, input.ProductId)
                    : await GetNamedUsersFromApp(input.LicensedTenantId, input.ProductId);

            var usersBehaviour = await GetUsersBehaviourNamedOffline(productIdentifier, input);

            var resultItems = new List<LicenseUserBehaviourNamedOfflineOutput>();
            
            var registeredUsers = new List<string>();
            
            foreach (var user in usersBehaviour)
            {
                if (namedUsers.TryGetValue(user.User, out var deviceId))
                {
                    registeredUsers.Add(user.User);
                    var userBehaviourNamedOnline = new LicenseUserBehaviourNamedOfflineOutput()
                    {
                        LicensingIdentifier = user.LicensingIdentifier,
                        AppIdentifier = user.AppIdentifier,
                        AppName = user.AppName,
                        BundleIdentifier = user.BundleIdentifier,
                        BundleName = user.BundleName,
                        SoftwareName = user.SoftwareName,
                        SoftwareIdentifier = user.SoftwareIdentifier,
                        User = user.User,
                        SoftwareVersion = user.SoftwareVersion,
                        HostName = user.HostName,
                        HostUser = user.HostUser,
                        LocalIpAddress = user.LocalIpAddress,
                        Language = user.Language,
                        OsInfo = user.OsInfo,
                        BrowserInfo = user.BrowserInfo,
                        DatabaseName = user.DatabaseName,
                        StartTime = user.StartTime,
                        LastUpdate = user.LastUpdate,
                        AccountName = user.AccountName,
                        Domain = user.Domain,
                        AccessDuration = user.AccessDuration,
                        DeviceId = deviceId
                    };
                    resultItems.Add(userBehaviourNamedOnline);
                }
            }

            foreach (var namedUser in namedUsers)
            {
                if (!registeredUsers.Contains(namedUser.Key))
                {
                    var userBehaviourNamedOnline = new LicenseUserBehaviourNamedOfflineOutput()
                    {
                        User = namedUser.Key,
                        DeviceId = namedUser.Value,
                        BundleIdentifier = usersBehaviour.Any() ? usersBehaviour.First().BundleIdentifier : string.Empty,
                        SoftwareName = usersBehaviour.Any() ? usersBehaviour.First().SoftwareName : string.Empty,
                        SoftwareIdentifier = usersBehaviour.Any() ? usersBehaviour.First().SoftwareIdentifier : string.Empty
                    };
                    resultItems.Add(userBehaviourNamedOnline);
                }
            }

            var totalCount = resultItems.Count;

            resultItems = resultItems.AsQueryable()
                .ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting)
                .PageBy(input.SkipCount, input.MaxResultCount)
                .ToList();

            
            return new PagedResultDto<LicenseUserBehaviourNamedOfflineOutput>()
            {
                TotalCount = totalCount,
                Items = resultItems
            };
        }

        private async Task<Dictionary<string, string>> GetNamedUsersFromApp(Guid licensedTenantId, Guid licensedAppId)
        {
            var filter = new PagedFilteredAndSortedRequestInput()
            {
                SkipCount = 0,
                MaxResultCount = int.MaxValue
            };
            
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(
                    ExternalServicesConsts.LicensingManagement.LicensedBundle.GetNamedUserFromApp(licensedTenantId, licensedAppId, filter))
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                
                .Build();

            var gatewayCallResponse = await gatewayCall.CallAsync<GetNamedUserFromAppOutput>();
            var response = await gatewayCallResponse.GetResponse();
            var usersEmail = response.NamedUserAppLicenseOutputs.Items
                .Select(e => new KeyValuePair<string,string>(e.NamedUserEmail, e.DeviceId))
                .ToDictionary(e => e.Key, e => e.Value);
            return usersEmail;
        } 

        private async Task<Dictionary<string, string>> GetNamedUsersFromBundle(Guid licensedTenantId, Guid licensedBundleId)
        {
            var filter = new PagedFilteredAndSortedRequestInput()
            {
                SkipCount = 0,
                MaxResultCount = int.MaxValue
            };
            
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(
                    ExternalServicesConsts.LicensingManagement.LicensedBundle.GetNamedUserFromBundle(licensedTenantId, licensedBundleId, filter))
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                
                .Build();

            var gatewayCallResponse = await gatewayCall.CallAsync<GetNamedUserFromBundleOutput>();
            var response = await gatewayCallResponse.GetResponse();
            var usersEmail = response.NamedUserBundleLicenseOutputs.Items
                .Select(e => new KeyValuePair<string,string>(e.NamedUserEmail, e.DeviceId))
                .ToDictionary(e => e.Key, e => e.Value);
            return usersEmail;
        }
        
        private async Task<List<LicenseUserBehaviourOutput>> GetUsersBehaviourNamedOnline(string productIdentifier, GetUserBehaviorFromProductInput input)
        {
            var query = GetUsersBehaviourNamedOnlineQuery(productIdentifier, input);
            var items = await query.Select(e => new LicenseUserBehaviourOutput()
            {
                Domain = e.Domain,
                Language = e.Language,
                User = e.User,
                AccessDuration = e.AccessDuration,
                AccountName = e.AccountName,
                AppIdentifier = e.AppIdentifier,
                AppName = e.AppName,
                BrowserInfo = e.BrowserInfo,
                BundleIdentifier = e.BundleIdentifier,
                BundleName = e.BundleName,
                DatabaseName = e.DatabaseName,
                HostName = e.HostName,
                HostUser = e.HostUser,
                LastUpdate = e.LastUpdate,
                LicensingIdentifier = e.LicensingIdentifier,
                OsInfo = e.OsInfo,
                SoftwareIdentifier = e.SoftwareIdentifier,
                SoftwareName = e.SoftwareName,
                SoftwareVersion = e.SoftwareVersion,
                StartTime = e.StartTime,
                LocalIpAddress = e.LocalIpAddress
            }).ToListAsync(); 
            return items;
        }
        
        private async Task<List<LicenseUserBehaviourOutput>> GetUsersBehaviourNamedOffline(string productIdentifier, GetUserBehaviorFromProductInput input)
        {
            var query = GetUsersBehaviourNamedOfflineQuery(productIdentifier, input);
            var items = await query.Select(e => new LicenseUserBehaviourOutput()
            {
                Domain = e.Domain,
                Language = e.Language,
                User = e.User,
                AccessDuration = e.AccessDuration,
                AccountName = e.AccountName,
                AppIdentifier = e.AppIdentifier,
                AppName = e.AppName,
                BrowserInfo = e.BrowserInfo,
                BundleIdentifier = e.BundleIdentifier,
                BundleName = e.BundleName,
                DatabaseName = e.DatabaseName,
                HostName = e.HostName,
                HostUser = e.HostUser,
                LastUpdate = e.LastUpdate,
                LicensingIdentifier = e.LicensingIdentifier,
                OsInfo = e.OsInfo,
                SoftwareIdentifier = e.SoftwareIdentifier,
                SoftwareName = e.SoftwareName,
                SoftwareVersion = e.SoftwareVersion,
                StartTime = e.StartTime,
                LocalIpAddress = e.LocalIpAddress
            }).ToListAsync(); 
            return items;
        }
        
        public async Task<PagedResultDto<LicenseUserBehaviourOutput>> GetUsersBehaviourFloating(string productIdentifier, GetUserBehaviorFromProductInput input)
        {
            var query = GetUsersBehaviourFloatingQuery(productIdentifier, input);
            
            var totalCount = await query.CountAsync();

            query = query.PageBy(input.SkipCount, input.MaxResultCount);

            var items = _mapper.Map<List<LicenseUserBehaviourOutput>>(await query.ToListAsync());
            
            var output = new PagedResultDto<LicenseUserBehaviourOutput>
            {
                Items = items,
                TotalCount = totalCount
            };

            return output;
        }

        private static (Expression<Func<Entities.LicenseUsageInRealTime, string>>, bool) DefaultGetAllSorting()
        {
            return (license => license.User, true);
        }

        private IQueryable<Entities.LicenseUsageInRealTime> GetUsersBehaviourQuery(GetAllLicenseUserBehaviour input)
        {
            IQueryable<Entities.LicenseUsageInRealTime> query;
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                query = _licenseUsageInRealTime.AsQueryable().ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting);
            }
            else
            { 
                var (defaultSorting, ascSorting) = DefaultGetAllSorting();
                query = _licenseUsageInRealTime.AsQueryable().ApplyAdvancedFilter(input.AdvancedFilter, defaultSorting, ascSorting);
            }

            query = input.LicensingIdentifier == null ? query.Where(license => license.User != null) : query.Where(license => license.User != null && license.LicensingIdentifier == input.LicensingIdentifier);
            
            return query;
        }
        
        private IQueryable<Entities.LicenseUsageInRealTime> GetUsersBehaviourFloatingQuery(string productIdentifier, GetUserBehaviorFromProductInput input)
        {
            IQueryable<Entities.LicenseUsageInRealTime> query;
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                query = _licenseUsageInRealTime.AsQueryable().ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting);
            }
            else
            { 
                var (defaultSorting, ascSorting) = DefaultGetAllSorting();
                query = _licenseUsageInRealTime.AsQueryable().ApplyAdvancedFilter(input.AdvancedFilter, defaultSorting, ascSorting);
            }
            
            query = query.Where(license =>
                license.User != null 
                && (input.ProductType == ProductType.LicensedBundle ? license.BundleIdentifier : license.AppIdentifier) == productIdentifier 
                && license.LicensingIdentifier == input.LicensingIdentifier
                && license.LicensingModel == LicensingModels.Floating);
            
            return query;
        }
        
        private IQueryable<Entities.LicenseUsageInRealTime> GetUsersBehaviourNamedOnlineQuery(string productIdentifier, GetUserBehaviorFromProductInput input)
        {
            IQueryable<Entities.LicenseUsageInRealTime> query;
            
            query = _licenseUsageInRealTime.AsQueryable();

            query = query.Where(license =>
                    license.User != null 
                    && (input.ProductType == ProductType.LicensedBundle ? license.BundleIdentifier : license.AppIdentifier) == productIdentifier 
                    && license.LicensingIdentifier == input.LicensingIdentifier
                    && license.LicensingModel == LicensingModels.Named && license.LicensingMode == LicensingModes.Online);
            
            return query;
        }
        
        private IQueryable<Entities.LicenseUsageInRealTime> GetUsersBehaviourNamedOfflineQuery(string productIdentifier, GetUserBehaviorFromProductInput input)
        {
            IQueryable<Entities.LicenseUsageInRealTime> query;
            
            query = _licenseUsageInRealTime.AsQueryable();

            query = query.Where(license =>
                    license.User != null 
                    && (input.ProductType == ProductType.LicensedBundle ? license.BundleIdentifier : license.AppIdentifier) == productIdentifier 
                    && license.LicensingIdentifier == input.LicensingIdentifier
                    && license.LicensingModel == LicensingModels.Named && license.LicensingMode == LicensingModes.Offline);

            return query;
        }
    }
}