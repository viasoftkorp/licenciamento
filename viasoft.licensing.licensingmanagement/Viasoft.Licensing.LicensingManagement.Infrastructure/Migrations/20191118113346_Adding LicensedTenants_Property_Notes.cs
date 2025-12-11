using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingLicensedTenants_Property_Notes : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingLicensedTenants_Property_Notes(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Notes",
                table: "LicensedTenant",
                nullable: true, schema:_schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "LicensedTenant", schema:_schemaNameProvider.GetSchemaName());
        }
    }
}
