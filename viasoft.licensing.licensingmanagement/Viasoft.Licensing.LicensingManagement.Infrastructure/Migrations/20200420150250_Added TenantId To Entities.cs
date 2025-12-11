using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddedTenantIdToEntities : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddedTenantIdToEntities(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Software",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "LicensedTenantView",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "LicensedTenant",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "LicensedBundle",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "LicensedApp",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "BundledApp",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Bundle",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "App",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Account",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Software",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LicensedTenantView",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LicensedBundle",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LicensedApp",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "BundledApp",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Bundle",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "App",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Account",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
