using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace userapi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cc3a779f-be57-45b6-b009-a4f669e198d8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "de196f53-1a52-47c3-83f5-5d6d8d6431fb");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "06066308-fb9f-4203-8b4a-e5c44541bd04", null, "User", "USER" },
                    { "35fdd9d6-df20-4b3f-a0ea-13bc24a131de", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "06066308-fb9f-4203-8b4a-e5c44541bd04");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "35fdd9d6-df20-4b3f-a0ea-13bc24a131de");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "cc3a779f-be57-45b6-b009-a4f669e198d8", null, "Admin", "ADMIN" },
                    { "de196f53-1a52-47c3-83f5-5d6d8d6431fb", null, "User", "USER" }
                });
        }
    }
}
