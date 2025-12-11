using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.Migrations
{
    public partial class AddingLicensingModelsColumn : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingLicensingModelsColumn(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "licensingMode",
                table: "LicenseUsageInRealTime",
                type: "int",
                nullable: true,
                schema:_schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<int>(
                name: "licensingModels",
                table: "LicenseUsageInRealTime",
                type: "int",
                nullable: false,
                defaultValue: 0,
                schema:_schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "licensingMode",
                table: "LicenseUsageInRealTime",
                schema:_schemaNameProvider.GetSchemaName());


            migrationBuilder.DropColumn(
                name: "licensingModels",
                table: "LicenseUsageInRealTime",
                schema:_schemaNameProvider.GetSchemaName());

        }
    }
}
