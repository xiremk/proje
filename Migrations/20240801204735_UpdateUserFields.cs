/*
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IdentityServerProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "733ce9d5-4fea-492f-a22e-1265a2b65027");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f9b05946-6c26-426b-bd77-34f5825f5320");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2bdb2742-d564-48bb-bfc4-f06cec6873ef", null, "admin", "ADMIN" },
                    { "4da2e884-949f-4efa-84fd-e7888e0c4eac", null, "user", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2bdb2742-d564-48bb-bfc4-f06cec6873ef");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4da2e884-949f-4efa-84fd-e7888e0c4eac");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "733ce9d5-4fea-492f-a22e-1265a2b65027", null, "user", "USER" },
                    { "f9b05946-6c26-426b-bd77-34f5825f5320", null, "admin", "ADMIN" }
                }); 
        }
    }
}
*/