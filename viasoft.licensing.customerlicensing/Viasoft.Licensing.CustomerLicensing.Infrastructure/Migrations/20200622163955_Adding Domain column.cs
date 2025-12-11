using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.Migrations
{
    public partial class AddingDomaincolumn : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingDomaincolumn(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Domain",
                table: "LicenseUsageInRealTime",
                nullable: false,
                defaultValue: 0,
                schema: _schemaNameProvider.GetSchemaName());
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
