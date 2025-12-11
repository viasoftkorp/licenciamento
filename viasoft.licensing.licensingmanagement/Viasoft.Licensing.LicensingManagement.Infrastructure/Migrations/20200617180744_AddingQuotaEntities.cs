using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingQuotaEntities : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;
        
        public AddingQuotaEntities(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileAppQuota",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LicensedTenantId = table.Column<Guid>(nullable: false),
                    AppId = table.Column<Guid>(nullable: false),
                    AppName = table.Column<string>(maxLength: 450, nullable: true),
                    QuotaLimit = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAppQuota", x => x.Id);
                },
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateTable(
                name: "FileTenantQuota",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LicenseTenantId = table.Column<Guid>(nullable: false),
                    QuotaLimit = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTenantQuota", x => x.Id);
                },
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileAppQuota",
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.DropTable(
                name: "FileTenantQuota",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
