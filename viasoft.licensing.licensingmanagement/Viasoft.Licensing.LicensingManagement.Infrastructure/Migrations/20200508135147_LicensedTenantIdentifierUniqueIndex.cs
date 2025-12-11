using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class LicensedTenantIdentifierUniqueIndex : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public LicensedTenantIdentifierUniqueIndex(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LicensedTenant_Identifier",
                table: "LicensedTenant",
                column: "Identifier",
                unique: true, schema:_schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LicensedTenant_Identifier",
                table: "LicensedTenant", schema:_schemaNameProvider.GetSchemaName());
        }
    }
}
