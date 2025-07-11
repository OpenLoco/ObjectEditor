using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class RenameStringTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblStringTable_Objects_ObjectId",
                table: "TblStringTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblStringTable",
                table: "TblStringTable");

            migrationBuilder.RenameTable(
                name: "TblStringTable",
                newName: "StringTable");

            migrationBuilder.RenameIndex(
                name: "IX_TblStringTable_RowText",
                table: "StringTable",
                newName: "IX_StringTable_RowText");

            migrationBuilder.RenameIndex(
                name: "IX_TblStringTable_ObjectId",
                table: "StringTable",
                newName: "IX_StringTable_ObjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StringTable",
                table: "StringTable",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StringTable_Objects_ObjectId",
                table: "StringTable",
                column: "ObjectId",
                principalTable: "Objects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StringTable_Objects_ObjectId",
                table: "StringTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StringTable",
                table: "StringTable");

            migrationBuilder.RenameTable(
                name: "StringTable",
                newName: "TblStringTable");

            migrationBuilder.RenameIndex(
                name: "IX_StringTable_RowText",
                table: "TblStringTable",
                newName: "IX_TblStringTable_RowText");

            migrationBuilder.RenameIndex(
                name: "IX_StringTable_ObjectId",
                table: "TblStringTable",
                newName: "IX_TblStringTable_ObjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblStringTable",
                table: "TblStringTable",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblStringTable_Objects_ObjectId",
                table: "TblStringTable",
                column: "ObjectId",
                principalTable: "Objects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
