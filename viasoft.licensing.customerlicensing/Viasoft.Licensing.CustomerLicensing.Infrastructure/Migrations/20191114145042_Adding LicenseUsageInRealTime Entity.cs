using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.Migrations
{
    public partial class AddingLicenseUsageInRealTimeEntity : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingLicenseUsageInRealTimeEntity(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LicenseUsageInRealTime",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    AppIdentifier = table.Column<string>(maxLength: 450, nullable: false),
                    AppName = table.Column<string>(maxLength: 450, nullable: false),
                    BundleIdentifier = table.Column<string>(maxLength: 450, nullable: true),
                    BundleName = table.Column<string>(maxLength: 450, nullable: true),
                    SoftwareName = table.Column<string>(maxLength: 450, nullable: false),
                    SoftwareIdentifier = table.Column<string>(maxLength: 450, nullable: false),
                    AppLicenses = table.Column<int>(nullable: false),
                    AppLicensesConsumed = table.Column<int>(nullable: false),
                    AdditionalLicenses = table.Column<int>(nullable: false),
                    AdditionalLicensesConsumed = table.Column<int>(nullable: false),
                    AdditionalLicense = table.Column<bool>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    User = table.Column<string>(maxLength: 450, nullable: true),
                    Cnpj = table.Column<string>(maxLength: 450, nullable: true),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    LicensingStatus = table.Column<int>(nullable: false),
                    AppStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseUsageInRealTime", x => x.Id);
                }, schema:_schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicenseUsageInRealTime", schema:_schemaNameProvider.GetSchemaName());
        }
    }
}
