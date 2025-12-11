using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.Software
{
    public interface ISoftwareRepository
    {
        Task<Dictionary<Guid, string>> GetSoftwareNamesFromIdList(List<Guid> id);
    }
}