using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Istu.Navigation.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class upd_user_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IstuToken",
                table: "Users",
                newName: "IstuRefreshToken");

            migrationBuilder.AlterColumn<string>(
                name: "HashPassword",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "IstuAccessToken",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IstuAccessToken",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IstuRefreshToken",
                table: "Users",
                newName: "IstuToken");

            migrationBuilder.AlterColumn<string>(
                name: "HashPassword",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
