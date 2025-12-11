using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class DroppingNumberOfDaysToExpirationcolumn : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public DroppingNumberOfDaysToExpirationcolumn(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfDaysToExpiration",
                table: "LicensedTenantView",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "NumberOfDaysToExpiration",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfDaysToExpiration",
                table: "LicensedTenantView",
                nullable: false,
                defaultValue: 0,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDaysToExpiration",
                table: "LicensedTenant",
                nullable: false,
                defaultValue: 0,
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
