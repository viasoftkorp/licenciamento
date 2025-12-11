using FluentAssertions;
using Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail.Data;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Sagas.AdministratorEmail;

public static class AdministratorSagaDataTestExtensions
{
    public static void AssertSagaData(this AdministratorEmailSagaData data, AdministratorEmailSagaData expected)
    {
        data.SagaId.Should().Be(expected.SagaId);
        data.TenantId.Should().Be(expected.TenantId);
        data.NewUserId.Should().Be(expected.NewUserId);
        data.OldUserEmail.Should().Be(expected.OldUserEmail);
        data.AmCreatingNewLicensedTenant.Should().Be(expected.AmCreatingNewLicensedTenant);
        data.CurrentSagaStatus.Should().Be(expected.CurrentSagaStatus);
    }
}