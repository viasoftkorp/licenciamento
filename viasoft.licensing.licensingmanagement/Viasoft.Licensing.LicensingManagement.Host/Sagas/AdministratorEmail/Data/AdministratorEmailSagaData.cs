using System;
using Rebus.Sagas.Idempotent;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail.Data;

public class AdministratorEmailSagaData : IdempotentSagaData
{
    public string SagaId { get; set; }
    public Guid TenantId { get; set; }
    public Guid? NewUserId { get; set; }
    public string OldUserEmail { get; set; }
    public string NewUserEmail { get; set; }
    public bool AmCreatingNewLicensedTenant { get; set; }
    public LicensingStatus? CurrentLicensedTenantStatus { get; set; }
    public CurrentSagaStatus CurrentSagaStatus { get; set; }
}