using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.Migrations
{
    public partial class FixedLicensingModelsColumnname : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public FixedLicensingModelsColumnname(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "licensingModels",
                table: "LicenseUsageInRealTime",
                newName: "licensingModel",
                schema:_schemaNameProvider.GetSchemaName());

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "licensingModel",
                table: "LicenseUsageInRealTime",
                newName: "licensingModels",
                schema:_schemaNameProvider.GetSchemaName());

        }
    }
}
