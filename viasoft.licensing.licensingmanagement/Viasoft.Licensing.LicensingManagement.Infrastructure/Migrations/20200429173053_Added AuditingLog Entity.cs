using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddedAuditingLogEntity : Migration
    {
        private readonly ISchemaNameProvider _schemaNameProvider;
        
        public AddedAuditingLogEntity(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditingLog",
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
                    UserName = table.Column<string>(maxLength: 450, nullable: true),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Details = table.Column<string>(maxLength: 9000, nullable: true),
                    ActionName = table.Column<string>(maxLength: 450, nullable: true),
                    Action = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditingLog", x => x.Id);
                }, schema:_schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditingLog", schema:_schemaNameProvider.GetSchemaName());
        }
    }
}
