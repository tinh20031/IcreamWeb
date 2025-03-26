using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IcreamShopApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatevnpay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "IceCreams",
                keyColumn: "IceCreamId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 25, 16, 21, 20, 370, DateTimeKind.Local).AddTicks(8933));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "IceCreams",
                keyColumn: "IceCreamId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 24, 20, 14, 26, 659, DateTimeKind.Local).AddTicks(6890));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId");
        }
    }
}
