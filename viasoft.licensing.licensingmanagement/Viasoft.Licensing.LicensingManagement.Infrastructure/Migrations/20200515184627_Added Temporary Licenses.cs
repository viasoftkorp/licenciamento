using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddedTemporaryLicenses : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;
        public AddedTemporaryLicenses(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDateOfTemporaryLicenses",
                table: "LicensedBundle",
                nullable: true,
                schema:_schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTemporaryLicenses",
                table: "LicensedBundle",
                nullable: false,
                defaultValue: 0,
                schema:_schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDateOfTemporaryLicenses",
                table: "LicensedApp",
                nullable: true,
                schema:_schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTemporaryLicenses",
                table: "LicensedApp",
                nullable: false,
                defaultValue: 0,
                schema:_schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDateOfTemporaryLicenses",
                table: "LicensedBundle",
                schema:_schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "NumberOfTemporaryLicenses",
                table: "LicensedBundle",
                schema:_schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "ExpirationDateOfTemporaryLicenses",
                table: "LicensedApp",
                schema:_schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "NumberOfTemporaryLicenses",
                table: "LicensedApp",
                schema:_schemaNameProvider.GetSchemaName());
        }
    }
}
