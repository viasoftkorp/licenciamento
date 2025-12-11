using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.Migrations
{
    public partial class AddingtheAccountIdandAccountNamecolumns : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingtheAccountIdandAccountNamecolumns(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "LicenseUsageInRealTime",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "LicenseUsageInRealTime",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "LicenseUsageInRealTime",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
