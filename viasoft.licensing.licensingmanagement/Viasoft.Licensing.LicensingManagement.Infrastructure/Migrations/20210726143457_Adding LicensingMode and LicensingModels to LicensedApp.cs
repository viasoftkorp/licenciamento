using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingLicensingModeandLicensingModelstoLicensedApp : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingLicensingModeandLicensingModelstoLicensedApp(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicensingMode",
                table: "LicensedApp",
                type: "int",
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<int>(
                name: "LicensingModel",
                table: "LicensedApp",
                type: "int",
                nullable: false,
                defaultValue: 0,
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicensingMode",
                table: "LicensedApp",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "LicensingModel",
                table: "LicensedApp",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
