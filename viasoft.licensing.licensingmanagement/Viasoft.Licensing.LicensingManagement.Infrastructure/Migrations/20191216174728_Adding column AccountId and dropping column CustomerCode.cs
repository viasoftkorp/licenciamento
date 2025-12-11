using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingcolumnAccountIdanddroppingcolumnCustomerCode : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingcolumnAccountIdanddroppingcolumnCustomerCode(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName()
                );

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "LicensedTenant",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "LicensedTenant",
                maxLength: 450,
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
