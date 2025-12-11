using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingLicensedAppIndex : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingLicensedAppIndex(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LicensedApp_LicensedTenantId_TenantId",
                table: "LicensedApp",
                columns: new[] { "LicensedTenantId", "TenantId" },
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName())
                .Annotation("SqlServer:Include", new[] { "AppId" });
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LicensedApp_LicensedTenantId_TenantId",
                table: "LicensedApp",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
