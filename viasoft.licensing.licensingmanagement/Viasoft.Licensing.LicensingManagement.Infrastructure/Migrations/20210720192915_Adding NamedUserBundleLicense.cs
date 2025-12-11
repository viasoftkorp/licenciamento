using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingNamedUserBundleLicense : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingNamedUserBundleLicense(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NamedUserBundleLicenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicensedTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicensedBundleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NamedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NamedUserEmail = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamedUserBundleLicenses", x => x.Id);
                },
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NamedUserBundleLicenses",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
