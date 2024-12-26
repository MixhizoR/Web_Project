﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropPulse.Migrations
{
    /// <inheritdoc />
    public partial class fixSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Photos",
                table: "Properties",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photos",
                table: "Properties");
        }
    }
}
