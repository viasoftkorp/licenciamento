using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.PushNotifications.Abstractions.Contracts;

namespace Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail.Data;

public class LicensedTenantSagaStatusUpdateNotification : NotificationUpdate
{
    public override string UniqueTypeName => "LicensedTenantSagaStatusUpdateNotification";
    public bool AmCreatingNewLicensedTenant { get; set; }
    public CurrentSagaStatus Status { get; set; }
    public LicensingStatus CurrentLicensedTenantStatus { get; set; }
    public string NewEmail { get; set; }
    public long CurrentTick { get; set; }
}