using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class RenameStringTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropForeignKey(
			name: "FK_TblStringTable_Objects_ObjectId",
			table: "TblStringTable");

		_ = migrationBuilder.DropPrimaryKey(
			name: "PK_TblStringTable",
			table: "TblStringTable");

		_ = migrationBuilder.RenameTable(
			name: "TblStringTable",
			newName: "StringTable");

		_ = migrationBuilder.RenameIndex(
			name: "IX_TblStringTable_RowText",
			table: "StringTable",
			newName: "IX_StringTable_RowText");

		_ = migrationBuilder.RenameIndex(
			name: "IX_TblStringTable_ObjectId",
			table: "StringTable",
			newName: "IX_StringTable_ObjectId");

		_ = migrationBuilder.AddPrimaryKey(
			name: "PK_StringTable",
			table: "StringTable",
			column: "Id");

		_ = migrationBuilder.AddForeignKey(
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
		_ = migrationBuilder.DropForeignKey(
			name: "FK_StringTable_Objects_ObjectId",
			table: "StringTable");

		_ = migrationBuilder.DropPrimaryKey(
			name: "PK_StringTable",
			table: "StringTable");

		_ = migrationBuilder.RenameTable(
			name: "StringTable",
			newName: "TblStringTable");

		_ = migrationBuilder.RenameIndex(
			name: "IX_StringTable_RowText",
			table: "TblStringTable",
			newName: "IX_TblStringTable_RowText");

		_ = migrationBuilder.RenameIndex(
			name: "IX_StringTable_ObjectId",
			table: "TblStringTable",
			newName: "IX_TblStringTable_ObjectId");

		_ = migrationBuilder.AddPrimaryKey(
			name: "PK_TblStringTable",
			table: "TblStringTable",
			column: "Id");

		_ = migrationBuilder.AddForeignKey(
			name: "FK_TblStringTable_Objects_ObjectId",
			table: "TblStringTable",
			column: "ObjectId",
			principalTable: "Objects",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);
        }
    }
