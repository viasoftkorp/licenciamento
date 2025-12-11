using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.Migrations
{
    public partial class AddingadditionalinformationtoLicenseUsageInRealTimeEntity : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingadditionalinformationtoLicenseUsageInRealTimeEntity(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrowserInfo",
                table: "LicenseUsageInRealTime",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "DatabaseName",
                table: "LicenseUsageInRealTime",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "LicenseUsageInRealTime",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "HostUser",
                table: "LicenseUsageInRealTime",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "LicenseUsageInRealTime",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "LocalIpAddress",
                table: "LicenseUsageInRealTime",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "OsInfo",
                table: "LicenseUsageInRealTime",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "SoftwareVersion",
                table: "LicenseUsageInRealTime",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_LicenseUsageInRealTime_TenantId",
                table: "LicenseUsageInRealTime",
                column: "TenantId",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_LicenseUsageInRealTime_TenantId_User",
                table: "LicenseUsageInRealTime",
                columns: new[] { "TenantId", "User" },
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LicenseUsageInRealTime_TenantId",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropIndex(
                name: "IX_LicenseUsageInRealTime_TenantId_User",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "BrowserInfo",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "DatabaseName",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "HostUser",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "Language",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "LocalIpAddress",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "OsInfo",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "SoftwareVersion",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
