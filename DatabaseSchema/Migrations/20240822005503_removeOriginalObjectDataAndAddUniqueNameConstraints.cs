using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSchema.Migrations
{
	/// <inheritdoc />
	public partial class removeOriginalObjectDataAndAddUniqueNameConstraints : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropColumn(
				name: "OriginalBytes",
				table: "Objects");

			_ = migrationBuilder.CreateIndex(
				name: "IX_Objects_OriginalName_OriginalChecksum",
				table: "Objects",
				columns: new[] { "OriginalName", "OriginalChecksum" },
				unique: true,
				descending: new[] { true, false });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropIndex(
				name: "IX_Objects_Name",
				table: "Objects");

			_ = migrationBuilder.AddColumn<byte[]>(
				name: "OriginalBytes",
				table: "Objects",
				type: "BLOB",
				nullable: false,
				defaultValue: new byte[0]);
		}
	}
}
