using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.Migrations
{
    public partial class AddingtheOwnedAppCountEntity : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingtheOwnedAppCountEntity(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnedAppCount",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    LicensingIdentifier = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedAppCount", x => x.Id);
                },
                schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnedAppCount",
                schema: _schemaNameProvider.GetSchemaName());
        }
    }
}
