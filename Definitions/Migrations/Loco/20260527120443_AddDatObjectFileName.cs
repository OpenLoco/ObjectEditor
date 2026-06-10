using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations.Loco
{
	/// <inheritdoc />
	public partial class AddDatObjectFileName : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "FileName",
				table: "DatObjects",
				type: "TEXT",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "FileName",
				table: "DatObjects");
		}
	}
}
