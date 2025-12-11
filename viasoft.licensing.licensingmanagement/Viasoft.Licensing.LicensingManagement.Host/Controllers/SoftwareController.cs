using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.BaseCrud;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Software;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.App;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class SoftwareController: BaseCrudController<Software, SoftwareCreateOutput, SoftwareCreateInput, SoftwareUpdateInput, SoftwareUpdateOutput, GetAllSoftwaresInput, SoftwareDeleteOutput, OperationValidation, string>
    {
        private readonly IRepository<Software> _softwares;
        private readonly IMapper _mapper;
        private readonly IAppRepository _appRepository;
        private readonly IBundleRepository _bundleRepository;

        public SoftwareController(IRepository<Software> softwares, IMapper mapper, IAppRepository appRepository, IBundleRepository bundleRepository) : base(mapper, softwares)
        {
            _softwares = softwares;
            _mapper = mapper;
            _appRepository = appRepository;
            _bundleRepository = bundleRepository;
            
            Authorizations.Create = Policy.CreateSoftware;
            Authorizations.Update = Policy.UpdateSoftware;
            Authorizations.Delete = Policy.DeleteSoftware;
        }

        protected override IQueryable<Software> ApplyCustomFilters(IQueryable<Software> query, GetAllSoftwaresInput input)
        {
            return query.WhereIf(input.OnlyActiveSoftwares, software => software.IsActive); 
        }

        [HttpPost]
        [Authorize(Policy.CreateSoftware)]
        public override async Task<SoftwareCreateOutput> Create(SoftwareCreateInput input)
        {
            SoftwareCreateOutput output;
            
            if (_softwares.Any(s => s.Identifier == input.Identifier))
            {
                output  = _mapper.Map<SoftwareCreateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
            }
            else
            {
                var software = _mapper.Map<Software>(input);
                var newSoftware = await _softwares.InsertAsync(software);
                output = _mapper.Map<SoftwareCreateOutput>(newSoftware);
            }
            return output;
        }
        
        [HttpPost]
        [Authorize(Policy.UpdateSoftware)]
        public override async Task<SoftwareUpdateOutput> Update(SoftwareUpdateInput input)
        {
            SoftwareUpdateOutput output;
            if (_softwares.Any(s => s.Identifier == input.Identifier && s.Id != input.Id))
            {
                output  = _mapper.Map<SoftwareUpdateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
            }
            else
            {
                var software = await _softwares.FindAsync(input.Id);
                _mapper.Map(input, software);
                var updatedSoftware = await _softwares.UpdateAsync(software);
                output = _mapper.Map<SoftwareUpdateOutput>(updatedSoftware);
            }

            return output;
        }
        
        [HttpDelete]
        [Authorize(Policy.DeleteSoftware)]
        public override async Task<SoftwareDeleteOutput> Delete(Guid id)
        {
            var softwareUsedByOtherRegister = await _appRepository.IsSoftwareBeingUsedByApps(id) || await _bundleRepository.IsSoftwareBeingUsedByBundles(id);

            if (!softwareUsedByOtherRegister)
            {
                await _softwares.DeleteAsync(id);
                return new SoftwareDeleteOutput
                {
                    Success = true
                };
            }
                
            return new SoftwareDeleteOutput
            {
                Success = false,
                Errors = new List<BaseCrudResponseError<OperationValidation>>
                {
                    new BaseCrudResponseError<OperationValidation>
                    {
                        ErrorCode = OperationValidation.UsedByOtherRegister
                    }
                }
            };
        }

        protected override (Expression<Func<Software, string>>, bool) DefaultGetAllSorting()
        {
            return (software => software.Identifier, true) ;
        }
    }
}