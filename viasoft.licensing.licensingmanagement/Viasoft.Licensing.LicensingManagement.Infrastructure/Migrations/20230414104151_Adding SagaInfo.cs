using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

#nullable disable

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingSagaInfo : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingSagaInfo(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "SagaInfo",
                table: "LicensedTenant",
                type: "varbinary(max)",
                nullable: true,
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SagaInfo",
                table: "LicensedTenant",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
