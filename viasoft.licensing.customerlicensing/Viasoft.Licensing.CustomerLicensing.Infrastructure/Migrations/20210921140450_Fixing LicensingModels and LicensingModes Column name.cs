using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.Migrations
{
    public partial class FixingLicensingModelsandLicensingModesColumnname : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public FixingLicensingModelsandLicensingModesColumnname(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "licensingModel",
                table: "LicenseUsageInRealTime",
                newName: "LicensingModel",
                schema:_schemaNameProvider.GetSchemaName());

            migrationBuilder.RenameColumn(
                name: "licensingMode",
                table: "LicenseUsageInRealTime",
                newName: "LicensingMode",
                schema:_schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicensingModel",
                table: "LicenseUsageInRealTime",
                newName: "licensingModel",
                schema:_schemaNameProvider.GetSchemaName());

            migrationBuilder.RenameColumn(
                name: "LicensingMode",
                table: "LicenseUsageInRealTime",
                newName: "licensingMode",
                schema:_schemaNameProvider.GetSchemaName());
        }
    }
}
