using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingLicensedBundles : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingLicensedBundles(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LicensedBundle",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    BundleId = table.Column<Guid>(nullable: false),
                    LicensedTenantId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    NumberOfLicenses = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensedBundle", x => x.Id);
                }, schema:_schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicensedBundle", schema:_schemaNameProvider.GetSchemaName());
        }
    }
}
