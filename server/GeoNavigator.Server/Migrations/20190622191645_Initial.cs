using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeoNavigator.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PosX = table.Column<long>(nullable: false),
                    PosZ = table.Column<long>(nullable: false),
                    PosY = table.Column<long>(nullable: false),
                    Dimension = table.Column<int>(nullable: false),
                    Ore = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veins",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    YMin = table.Column<int>(nullable: false),
                    YMax = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VeinDimensions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VeinInfoId = table.Column<Guid>(nullable: false),
                    Dimension = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeinDimensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VeinDimensions_Veins_VeinInfoId",
                        column: x => x.VeinInfoId,
                        principalTable: "Veins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VeinOres",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VeinInfoId = table.Column<Guid>(nullable: false),
                    Ore = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeinOres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VeinOres_Veins_VeinInfoId",
                        column: x => x.VeinInfoId,
                        principalTable: "Veins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VeinDimensions_VeinInfoId",
                table: "VeinDimensions",
                column: "VeinInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_VeinOres_VeinInfoId",
                table: "VeinOres",
                column: "VeinInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "VeinDimensions");

            migrationBuilder.DropTable(
                name: "VeinOres");

            migrationBuilder.DropTable(
                name: "Veins");
        }
    }
}
