using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingInfrastructureConfiguration : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingInfrastructureConfiguration(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfrastructureConfigurations",
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
                    LicensedTenantId = table.Column<Guid>(nullable: false),
                    GatewayAddress = table.Column<string>(maxLength: 450, nullable: true),
                    DesktopDatabaseName = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfrastructureConfigurations", x => x.Id);
                },
                schema: _schemaNameProvider.GetSchemaName());

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureConfigurations_LicensedTenantId",
                table: "InfrastructureConfigurations",
                column: "LicensedTenantId",
                unique: true,
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfrastructureConfigurations",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
