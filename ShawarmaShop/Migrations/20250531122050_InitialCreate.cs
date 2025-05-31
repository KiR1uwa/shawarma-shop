using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShawarmaShop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shawarmas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Ingredients = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shawarmas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    ClientID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShawarmaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Shawarmas_ShawarmaId",
                        column: x => x.ShawarmaId,
                        principalTable: "Shawarmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, "john@example.com", "John Doe", "+48123456789" },
                    { 2, "jane@example.com", "Jane Smith", "+48987654321" },
                    { 3, "mike@example.com", "Mike Johnson", "+48555666777" },
                    { 4, "anna@example.com", "Anna Kowalski", "+48111222333" }
                });

            migrationBuilder.InsertData(
                table: "Shawarmas",
                columns: new[] { "Id", "Ingredients", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Chicken, lettuce, tomato, cucumber, sauce", "Classic Chicken", 15.50m },
                    { 2, "Beef, onion, pepper, cheese, spicy sauce", "Beef Deluxe", 18.00m },
                    { 3, "Falafel, lettuce, tomato, cucumber, hummus", "Vegetarian", 12.50m },
                    { 4, "Lamb, hot pepper, onion, garlic sauce", "Spicy Lamb", 20.00m },
                    { 5, "Chicken, beef, vegetables, mixed sauce", "Mixed Meat", 22.00m }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "ClientID", "Comment", "DateTime" },
                values: new object[,]
                {
                    { 1, 1, "Extra sauce please", new DateTime(2024, 5, 28, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, "No onions", new DateTime(2024, 5, 29, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, null, new DateTime(2024, 5, 30, 16, 45, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "Quantity", "ShawarmaId" },
                values: new object[,]
                {
                    { 1, 1, 2, 1 },
                    { 2, 1, 1, 3 },
                    { 3, 2, 1, 2 },
                    { 4, 3, 3, 4 },
                    { 5, 3, 1, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ShawarmaId",
                table: "OrderItems",
                column: "ShawarmaId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientID",
                table: "Orders",
                column: "ClientID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Shawarmas");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
