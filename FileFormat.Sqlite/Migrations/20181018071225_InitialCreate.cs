using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileFormat.Sqlite.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    NodeKey = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Nodes_Nodes_NodeKey",
                        column: x => x.NodeKey,
                        principalTable: "Nodes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Datas",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<byte[]>(nullable: true),
                    NodeKey = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datas", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Datas_Nodes_NodeKey",
                        column: x => x.NodeKey,
                        principalTable: "Nodes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Datas_NodeKey",
                table: "Datas",
                column: "NodeKey");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_NodeKey",
                table: "Nodes",
                column: "NodeKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Datas");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "Nodes");
        }
    }
}
