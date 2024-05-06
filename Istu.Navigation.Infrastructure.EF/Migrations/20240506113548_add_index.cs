using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Istu.Navigation.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class add_index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ImageLinks_ObjectId",
                table: "ImageLinks",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Edges_BuildingId_ToObject_FromObject",
                table: "Edges",
                columns: new[] { "BuildingId", "ToObject", "FromObject" });

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Title",
                table: "Buildings",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImageLinks_ObjectId",
                table: "ImageLinks");

            migrationBuilder.DropIndex(
                name: "IX_Edges_BuildingId_ToObject_FromObject",
                table: "Edges");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_Title",
                table: "Buildings");
        }
    }
}
