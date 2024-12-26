using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropPulse.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_User_PropOwnerId",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "PropOwnerId",
                table: "Properties",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_PropOwnerId",
                table: "Properties",
                newName: "IX_Properties_UserID");

            migrationBuilder.AlterColumn<string>(
                name: "Photos",
                table: "Properties",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_User_UserID",
                table: "Properties",
                column: "UserID",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_User_UserID",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Properties",
                newName: "PropOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_UserID",
                table: "Properties",
                newName: "IX_Properties_PropOwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "Photos",
                table: "Properties",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_User_PropOwnerId",
                table: "Properties",
                column: "PropOwnerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
