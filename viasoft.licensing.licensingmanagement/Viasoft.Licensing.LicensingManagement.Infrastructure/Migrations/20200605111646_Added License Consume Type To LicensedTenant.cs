using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddedLicenseConsumeTypeToLicensedTenant : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;
        
        public AddedLicenseConsumeTypeToLicensedTenant(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicenseConsumeType",
                table: "LicensedTenant",
                nullable: false,
                defaultValue: 0,
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseConsumeType",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
