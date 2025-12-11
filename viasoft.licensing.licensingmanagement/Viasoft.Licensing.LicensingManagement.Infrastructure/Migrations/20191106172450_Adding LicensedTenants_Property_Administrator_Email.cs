using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingLicensedTenants_Property_Administrator_Email : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingLicensedTenants_Property_Administrator_Email(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministratorEmail",
                table: "LicensedTenant",
                maxLength: 450,
                nullable: true, schema:_schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministratorEmail",
                table: "LicensedTenant", schema:_schemaNameProvider.GetSchemaName());
        }
    }
}
