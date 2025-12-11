using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Data.Seeder.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.Seeder
{
    public class LicensingStatusSeeder : ISeedData
    {
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly IUnitOfWork _unitOfWork;
        public LicensingStatusSeeder(IRepository<Entities.LicensedTenant> licensedTenants, IUnitOfWork unitOfWork)
        {
            _licensedTenants = licensedTenants;
            _unitOfWork = unitOfWork;
        } 
        
        public async Task SeedDataAsync()
        {
            // atualizando todos os status "pendente aprovação" para bloqueado
            // o status correspondia ao valor "0" que não existe mais no enum
            // por isso está sendo colocado diretamente no código
            
            var licensedTenantsToUpdate = await _licensedTenants.Where(lt => lt.Status == 0).ToListAsync();

            foreach (var licensedTenant in licensedTenantsToUpdate)
            {
                licensedTenant.Status = LicensingStatus.Blocked;
            }
            using (_unitOfWork.Begin())
            {
                foreach (var licensedTenant in licensedTenantsToUpdate)
                {
                    await _licensedTenants.UpdateAsync(licensedTenant);
                }
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}