using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingUniqueIndexes : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingUniqueIndexes(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LicensedTenant_Identifier",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_InfrastructureConfigurations_LicensedTenantId",
                table: "InfrastructureConfigurations",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_Software_TenantId_Identifier",
                table: "Software",
                columns: new[] { "TenantId", "Identifier" },
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_LicensedTenant_AdministratorEmail",
                table: "LicensedTenant",
                column: "AdministratorEmail",
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_LicensedTenant_AccountId_TenantId",
                table: "LicensedTenant",
                columns: new[] { "AccountId", "TenantId" },
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_LicensedTenant_Identifier_TenantId",
                table: "LicensedTenant",
                columns: new[] { "Identifier", "TenantId" },
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_LicensedBundle_TenantId_LicensedTenantId_BundleId",
                table: "LicensedBundle",
                columns: new[] { "TenantId", "LicensedTenantId", "BundleId" },
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_LicensedApp_TenantId_LicensedTenantId_AppId",
                table: "LicensedApp",
                columns: new[] { "TenantId", "LicensedTenantId", "AppId" },
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureConfigurations_LicensedTenantId",
                table: "InfrastructureConfigurations",
                column: "LicensedTenantId",
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_FileTenantQuota_LicenseTenantId",
                table: "FileTenantQuota",
                column: "LicenseTenantId",
                unique: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_FileAppQuota_LicensedTenantId_AppId",
                table: "FileAppQuota",
                columns: new[] { "LicensedTenantId", "AppId" },
                unique: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_BundledApp_BundleId_AppId_TenantId",
                table: "BundledApp",
                columns: new[] { "BundleId", "AppId", "TenantId" },
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_Bundle_Identifier_TenantId",
                table: "Bundle",
                columns: new[] { "Identifier", "TenantId" },
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_App_Identifier_TenantId",
                table: "App",
                columns: new[] { "Identifier", "TenantId" },
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_Account_CnpjCpf",
                table: "Account",
                column: "CnpjCpf",
                unique: true,
                filter: "[IsDeleted] = 0",
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Software_TenantId_Identifier",
                table: "Software",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_LicensedTenant_AdministratorEmail",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_LicensedTenant_AccountId_TenantId",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_LicensedTenant_Identifier_TenantId",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_LicensedBundle_TenantId_LicensedTenantId_BundleId",
                table: "LicensedBundle",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_LicensedApp_TenantId_LicensedTenantId_AppId",
                table: "LicensedApp",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_InfrastructureConfigurations_LicensedTenantId",
                table: "InfrastructureConfigurations",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_FileTenantQuota_LicenseTenantId",
                table: "FileTenantQuota",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_FileAppQuota_LicensedTenantId_AppId",
                table: "FileAppQuota",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_BundledApp_BundleId_AppId_TenantId",
                table: "BundledApp",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_Bundle_Identifier_TenantId",
                table: "Bundle",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_App_Identifier_TenantId",
                table: "App",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_Account_CnpjCpf",
                table: "Account",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_LicensedTenant_Identifier",
                table: "LicensedTenant",
                column: "Identifier",
                unique: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureConfigurations_LicensedTenantId",
                table: "InfrastructureConfigurations",
                column: "LicensedTenantId",
                unique: true,
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
