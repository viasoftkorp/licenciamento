using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.Migrations
{
    public partial class AddingLicensingIdentifiercolumnandremovingTenantIdcolumnfromLicenseUsage : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingLicensingIdentifiercolumnandremovingTenantIdcolumnfromLicenseUsage(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "LicenseUsageInRealTime",
                newName: "LicensingIdentifier",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.RenameIndex(
                name: "IX_LicenseUsageInRealTime_TenantId_User",
                table: "LicenseUsageInRealTime",
                newName: "IX_LicenseUsageInRealTime_LicensingIdentifier_User",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.RenameIndex(
                name: "IX_LicenseUsageInRealTime_TenantId",
                table: "LicenseUsageInRealTime",
                newName: "IX_LicenseUsageInRealTime_LicensingIdentifier",
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicensingIdentifier",
                table: "LicenseUsageInRealTime",
                newName: "TenantId",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.RenameIndex(
                name: "IX_LicenseUsageInRealTime_LicensingIdentifier_User",
                table: "LicenseUsageInRealTime",
                newName: "IX_LicenseUsageInRealTime_TenantId_User",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.RenameIndex(
                name: "IX_LicenseUsageInRealTime_LicensingIdentifier",
                table: "LicenseUsageInRealTime",
                newName: "IX_LicenseUsageInRealTime_TenantId",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
