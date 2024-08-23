using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoco.Schema.Migrations
{
	/// <inheritdoc />
	public partial class addUniqueIndexOnNames : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.CreateIndex(
				name: "IX_Tags_Name",
				table: "Tags",
				column: "Name",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_Modpacks_Name",
				table: "Modpacks",
				column: "Name",
				unique: true);

			_ = migrationBuilder.CreateIndex(
				name: "IX_Licences_Name",
				table: "Licences",
				column: "Name",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropIndex(
				name: "IX_Tags_Name",
				table: "Tags");

			_ = migrationBuilder.DropIndex(
				name: "IX_Modpacks_Name",
				table: "Modpacks");

			_ = migrationBuilder.DropIndex(
				name: "IX_Licences_Name",
				table: "Licences");
		}
	}
}
