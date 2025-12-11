using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Viasoft.Core.Storage.Schema;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.Migrations
{
    public partial class AddingAccounts : Migration
    {
        
        private readonly ISchemaNameProvider _schemaNameProvider;

        public AddingAccounts(ISchemaNameProvider schemaNameProvider)
        {
            _schemaNameProvider = schemaNameProvider;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
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
                    Phone = table.Column<string>(maxLength: 450, nullable: true),
                    WebSite = table.Column<string>(maxLength: 450, nullable: true),
                    Email = table.Column<string>(maxLength: 450, nullable: true),
                    BillingEmail = table.Column<string>(maxLength: 450, nullable: true),
                    TradingName = table.Column<string>(maxLength: 450, nullable: true),
                    CompanyName = table.Column<string>(maxLength: 450, nullable: true),
                    CnpjCpf = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Street = table.Column<string>(maxLength: 450, nullable: true),
                    Number = table.Column<string>(maxLength: 450, nullable: true),
                    Detail = table.Column<string>(maxLength: 450, nullable: true),
                    Neighborhood = table.Column<string>(maxLength: 450, nullable: true),
                    City = table.Column<string>(maxLength: 450, nullable: true),
                    State = table.Column<string>(maxLength: 450, nullable: true),
                    Country = table.Column<string>(maxLength: 450, nullable: true),
                    ZipCode = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                }, schema: _schemaNameProvider.GetSchemaName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account", _schemaNameProvider.GetSchemaName());
        }
    }
}
