using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropPulse.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Properties",
                newName: "PhotoJPG");

            migrationBuilder.AddColumn<string>(
                name: "Photos",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photos",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "PhotoJPG",
                table: "Properties",
                newName: "PhotoUrl");
        }
    }
}
