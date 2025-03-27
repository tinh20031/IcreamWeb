using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IcreamShopApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class dropmess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IceCreams_Categories_CategoryId",
                table: "IceCreams");

            migrationBuilder.UpdateData(
                table: "IceCreams",
                keyColumn: "IceCreamId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 27, 17, 32, 4, 912, DateTimeKind.Local).AddTicks(3365));

            migrationBuilder.AddForeignKey(
                name: "FK_IceCreams_Categories_CategoryId",
                table: "IceCreams",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IceCreams_Categories_CategoryId",
                table: "IceCreams");

            migrationBuilder.UpdateData(
                table: "IceCreams",
                keyColumn: "IceCreamId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 23, 18, 59, 25, 448, DateTimeKind.Local).AddTicks(6607));

            migrationBuilder.AddForeignKey(
                name: "FK_IceCreams_Categories_CategoryId",
                table: "IceCreams",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
