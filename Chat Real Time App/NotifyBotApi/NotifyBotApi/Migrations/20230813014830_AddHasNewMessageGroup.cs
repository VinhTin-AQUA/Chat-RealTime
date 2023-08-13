using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotifyBotApi.Migrations
{
    /// <inheritdoc />
    public partial class AddHasNewMessageGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasNewMessage",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasNewMessage",
                table: "Groups");
        }
    }
}
