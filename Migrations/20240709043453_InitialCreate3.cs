using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace userapi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { "87ceab6d-1d21-4767-8af9-1fe58690f305", null, "Admin", "ADMIN" },
                    { "bdbf40fc-f316-463d-8f6f-c336fd4d2acf", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87ceab6d-1d21-4767-8af9-1fe58690f305");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bdbf40fc-f316-463d-8f6f-c336fd4d2acf");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "06066308-fb9f-4203-8b4a-e5c44541bd04", null, "User", "USER" },
                    { "35fdd9d6-df20-4b3f-a0ea-13bc24a131de", null, "Admin", "ADMIN" }
                });
        }
    }
}
