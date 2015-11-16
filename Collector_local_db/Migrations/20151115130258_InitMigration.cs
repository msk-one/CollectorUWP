using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Collector_local_db.Migrations
{
    public partial class InitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });
            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Curln = table.Column<string>(nullable: true),
                    Cursi = table.Column<string>(nullable: true),
                    Cursn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.CurrencyId);
                });
            migrationBuilder.CreateTable(
                name: "Type",
                columns: table => new
                {
                    TypeId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Typename = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type", x => x.TypeId);
                });
            migrationBuilder.CreateTable(
                name: "Object",
                columns: table => new
                {
                    ObjectId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryCategoryId = table.Column<int>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Object", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_Object_Category_CategoryCategoryId",
                        column: x => x.CategoryCategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId");
                });
            migrationBuilder.CreateTable(
                name: "Entry",
                columns: table => new
                {
                    EntryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<float>(nullable: false),
                    CurrencyCurrencyId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Deadline = table.Column<DateTime>(nullable: false),
                    Desc = table.Column<string>(nullable: true),
                    ObjectObjectId = table.Column<int>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    TypeTypeId = table.Column<int>(nullable: true),
                    Who = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entry", x => x.EntryId);
                    table.ForeignKey(
                        name: "FK_Entry_Currency_CurrencyCurrencyId",
                        column: x => x.CurrencyCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "CurrencyId");
                    table.ForeignKey(
                        name: "FK_Entry_Object_ObjectObjectId",
                        column: x => x.ObjectObjectId,
                        principalTable: "Object",
                        principalColumn: "ObjectId");
                    table.ForeignKey(
                        name: "FK_Entry_Type_TypeTypeId",
                        column: x => x.TypeTypeId,
                        principalTable: "Type",
                        principalColumn: "TypeId");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Entry");
            migrationBuilder.DropTable("Currency");
            migrationBuilder.DropTable("Object");
            migrationBuilder.DropTable("Type");
            migrationBuilder.DropTable("Category");
        }
    }
}
