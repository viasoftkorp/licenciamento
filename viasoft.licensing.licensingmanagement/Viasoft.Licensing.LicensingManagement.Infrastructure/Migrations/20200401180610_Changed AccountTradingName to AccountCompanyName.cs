using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class ChangedAccountTradingNametoAccountCompanyName : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public ChangedAccountTradingNametoAccountCompanyName(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountTradingName",
                table: "LicensedTenantView",
                newName: "AccountCompanyName",
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountCompanyName",
                table: "LicensedTenantView",
                newName: "AccountTradingName",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
