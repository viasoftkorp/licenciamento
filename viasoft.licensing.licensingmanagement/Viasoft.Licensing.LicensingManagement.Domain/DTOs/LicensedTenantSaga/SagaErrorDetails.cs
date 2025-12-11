namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;

public class SagaErrorDetails
{
    public string WhereHappened { get; set; }
    public string Description { get; set; }
    public string Exception { get; set; }
}