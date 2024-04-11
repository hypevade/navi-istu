using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Istu.Navigation.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class deleteFloors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Floors");

            migrationBuilder.AddColumn<bool>(
                name: "CreatedByAdmin",
                table: "ImageLinks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ImageLinks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByAdmin",
                table: "ImageLinks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ImageLinks");

            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    FloorNumber = table.Column<int>(type: "integer", nullable: false),
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => new { x.BuildingId, x.FloorNumber });
                    table.ForeignKey(
                        name: "FK_Floors_ImageLinks_ImageId",
                        column: x => x.ImageId,
                        principalTable: "ImageLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Floors_Objects_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Floors_ImageId",
                table: "Floors",
                column: "ImageId",
                unique: true);
        }
    }
}
