/*
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IdentityServerProject.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "441d8974-bc00-4b4d-aeb7-2f47e88d5b4d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53bdd66b-c5c4-4321-aafa-e7225fd4ae7b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a37cb404-69e9-41c9-a6ee-15431520e112", null, "admin", "ADMIN" },
                    { "dc3660d0-40da-4fb4-bd6c-a247a4380e0d", null, "user", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a37cb404-69e9-41c9-a6ee-15431520e112");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dc3660d0-40da-4fb4-bd6c-a247a4380e0d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "441d8974-bc00-4b4d-aeb7-2f47e88d5b4d", null, "user", "USER" },
                    { "53bdd66b-c5c4-4321-aafa-e7225fd4ae7b", null, "admin", "ADMIN" }
                });
        }
    }
}
*/