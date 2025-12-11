using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingHardwareIdtolicensedTenant : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingHardwareIdtolicensedTenant(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HardwareId",
                table: "LicensedTenantView",
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "HardwareId",
                table: "LicensedTenant",
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HardwareId",
                table: "LicensedTenantView",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "HardwareId",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
