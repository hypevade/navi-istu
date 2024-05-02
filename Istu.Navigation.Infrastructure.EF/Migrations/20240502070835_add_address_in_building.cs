using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Istu.Navigation.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class add_address_in_building : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Buildings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Buildings");
        }
    }
}
