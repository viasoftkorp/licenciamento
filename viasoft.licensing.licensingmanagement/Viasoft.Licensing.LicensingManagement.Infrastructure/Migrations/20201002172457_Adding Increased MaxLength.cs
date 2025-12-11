using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingIncreasedMaxLength : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingIncreasedMaxLength(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LicensedCnpjs",
                table: "LicensedTenantView",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AlterColumn<string>(
                name: "LicensedCnpjs",
                table: "LicensedTenant",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true,
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LicensedCnpjs",
                table: "LicensedTenantView",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AlterColumn<string>(
                name: "LicensedCnpjs",
                table: "LicensedTenant",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true,
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
