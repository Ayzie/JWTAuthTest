using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTAuthTest.Migrations
{
    /// <inheritdoc />
    public partial class ChangedSignupSecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignUpSecret",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignUpSecret",
                table: "AspNetUsers");
        }
    }
}
