using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingLicensingModeandLicensingModelstoLicensedBundle : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;
        
        public AddingLicensingModeandLicensingModelstoLicensedBundle(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicensingMode",
                table: "LicensedBundle",
                type: "int",
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<int>(
                name: "LicensingModel",
                table: "LicensedBundle",
                type: "int",
                nullable: false,
                defaultValue: 0,
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicensingMode",
                table: "LicensedBundle",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "LicensingModel",
                table: "LicensedBundle",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
