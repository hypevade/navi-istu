using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Istu.Navigation.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class upgrade_users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");
        }
    }
}
