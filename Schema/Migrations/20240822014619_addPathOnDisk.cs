using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoco.Schema.Migrations
{
	/// <inheritdoc />
	public partial class addPathOnDisk : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.AddColumn<string>(
				name: "PathOnDisk",
				table: "Objects",
				type: "TEXT",
				nullable: false,
				defaultValue: "");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Objects_PathOnDisk",
				table: "Objects",
				column: "PathOnDisk",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropIndex(
				name: "IX_Objects_PathOnDisk",
				table: "Objects");

			_ = migrationBuilder.DropColumn(
				name: "PathOnDisk",
				table: "Objects");
		}
	}
}
