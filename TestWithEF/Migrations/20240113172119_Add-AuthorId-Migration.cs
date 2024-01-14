using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWithEF.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorIdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AuthorId",
                table: "Orders",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Authors_AuthorId",
                table: "Orders",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Authors_AuthorId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AuthorId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Orders");
        }
    }
}
