using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingExpirationDatetime : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingExpirationDatetime(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDateTime",
                table: "LicensedBundle",
                type: "datetime2",
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDateTime",
                table: "LicensedApp",
                type: "datetime2",
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDateTime",
                table: "LicensedBundle",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "ExpirationDateTime",
                table: "LicensedApp",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
