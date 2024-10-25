using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Items",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "fk_orderItem_sneakerWarehouse",
                table: "order_items");

            migrationBuilder.AlterColumn<Guid>(
                name: "order_id",
                table: "order_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SneakerWarehouse_OrderItems",
                table: "order_items",
                column: "sneaker_warehouse_id",
                principalTable: "sneaker_warehouses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orderItem_order",
                table: "order_items",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SneakerWarehouse_OrderItems",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "fk_orderItem_order",
                table: "order_items");

            migrationBuilder.AlterColumn<Guid>(
                name: "order_id",
                table: "order_items",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Items",
                table: "order_items",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orderItem_sneakerWarehouse",
                table: "order_items",
                column: "sneaker_warehouse_id",
                principalTable: "sneaker_warehouses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
