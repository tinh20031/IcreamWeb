using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IcreamShopApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakePasswordHashNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "IceCreams",
                keyColumn: "IceCreamId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 23, 18, 59, 25, 448, DateTimeKind.Local).AddTicks(6607));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "IceCreams",
                keyColumn: "IceCreamId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 5, 15, 14, 43, 413, DateTimeKind.Local).AddTicks(9273));
        }
    }
}
