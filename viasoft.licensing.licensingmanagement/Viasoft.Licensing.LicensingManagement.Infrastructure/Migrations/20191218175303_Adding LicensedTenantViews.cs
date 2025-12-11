using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingLicensedTenantViews : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingLicensedTenantViews(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LicensedTenantView",
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
                    Status = table.Column<int>(nullable: false),
                    Identifier = table.Column<Guid>(nullable: false),
                    LicensedTenantId = table.Column<Guid>(nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(nullable: true),
                    NumberOfDaysToExpiration = table.Column<int>(nullable: false),
                    LicensedCnpjs = table.Column<string>(maxLength: 450, nullable: true),
                    AdministratorEmail = table.Column<string>(maxLength: 450, nullable: true),
                    AccountId = table.Column<Guid>(nullable: false),
                    AccountTradingName = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensedTenantView", x => x.Id);
                },
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicensedTenantView",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
