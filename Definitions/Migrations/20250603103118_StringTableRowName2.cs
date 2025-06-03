using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
	/// <inheritdoc />
	public partial class StringTableRowName2 : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropColumn(
				name: "Language",
				table: "TblStringTable");

			_ = migrationBuilder.RenameColumn(
				name: "Text",
				table: "TblStringTable",
				newName: "RowText");

			_ = migrationBuilder.RenameColumn(
				name: "Language",
				table: "TblStringTable",
				newName: "RowLanguage");

			_ = migrationBuilder.RenameIndex(
				name: "IX_TblStringTable_Text",
				table: "TblStringTable",
				newName: "IX_TblStringTable_RowText");

			_ = migrationBuilder.AddColumn<string>(
				name: "RowName",
				table: "TblStringTable",
				type: "TEXT",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropColumn(
				name: "RowName",
				table: "TblStringTable");

			_ = migrationBuilder.RenameColumn(
				name: "RowText",
				table: "TblStringTable",
				newName: "Text");

			_ = migrationBuilder.RenameColumn(
				name: "RowLanguage",
				table: "TblStringTable",
				newName: "Language");

			_ = migrationBuilder.RenameIndex(
				name: "IX_TblStringTable_RowText",
				table: "TblStringTable",
				newName: "IX_TblStringTable_Text");

			_ = migrationBuilder.AddColumn<byte>(
				name: "StringIndex",
				table: "TblStringTable",
				type: "INTEGER",
				nullable: false,
				defaultValue: (byte)0);
		}
	}
}
