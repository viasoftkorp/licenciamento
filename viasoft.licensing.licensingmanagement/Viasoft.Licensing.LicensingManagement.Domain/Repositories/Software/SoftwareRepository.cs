using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.Software
{
    public class SoftwareRepository : ISoftwareRepository, ITransientDependency
    {

        private readonly IRepository<Entities.Software> _softwares;

        public SoftwareRepository(IRepository<Entities.Software> softwares)
        {
            _softwares = softwares;
        }
        
        public async Task<Dictionary<Guid, string>> GetSoftwareNamesFromIdList(List<Guid> ids)
        {
            return await _softwares
                .Select(software => new {software.Id, software.Name})
                .Where(software => ids.Contains(software.Id))
                .ToDictionaryAsync(software => software.Id, software => software.Name);
        }
        
    }
}