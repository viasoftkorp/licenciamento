namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;

public class LicensedTenantSagaInfo
{
    public bool AmCreatingNewLicensedTenant { get; set; }
    public CurrentSagaStatus Status { get; set; }
    public SagaErrorDetails ErrorDetails { get; set; }
}