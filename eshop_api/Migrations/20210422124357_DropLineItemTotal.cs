using Microsoft.EntityFrameworkCore.Migrations;

namespace eshop_api.Migrations
{
    public partial class DropLineItemTotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineItemTotal",
                table: "ShoppingCartRecords");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "c05eddcd-c52d-4ae1-b461-57d8157d74e2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "626a90b1-1e53-4470-a5ce-ac08b52c6e6b");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "86c81ea8-3554-43ae-b6ee-b95d7a45f586", "AQAAAAEAACcQAAAAEMH1t5QqqdfVEkJYLCNS9bbwaVY44OlxMckrhgOz2Hz1xA6QJVCoWvRtTNi1TuAgHw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LineItemTotal",
                table: "ShoppingCartRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "88e0affb-5a41-436b-8015-a054468aede6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "5080c664-b5f3-40fb-b26e-daea13235192");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "55d80b1a-61d2-42eb-837d-b00061ef9398", "AQAAAAEAACcQAAAAEOEe7Ee8rbQsQnOquD7j/yf7NPtS2WjKyJicMZG9plEmXQta9GNSNk0XkiOOwmZ0KQ==" });
        }
    }
}
