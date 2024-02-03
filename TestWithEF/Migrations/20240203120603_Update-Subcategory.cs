using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWithEF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subcategories_WarehouseLayoutGroups_WarehouseLayoutGroupId",
                table: "Subcategories");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseLayoutGroupId",
                table: "Subcategories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Subcategories_WarehouseLayoutGroups_WarehouseLayoutGroupId",
                table: "Subcategories",
                column: "WarehouseLayoutGroupId",
                principalTable: "WarehouseLayoutGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subcategories_WarehouseLayoutGroups_WarehouseLayoutGroupId",
                table: "Subcategories");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseLayoutGroupId",
                table: "Subcategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subcategories_WarehouseLayoutGroups_WarehouseLayoutGroupId",
                table: "Subcategories",
                column: "WarehouseLayoutGroupId",
                principalTable: "WarehouseLayoutGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
