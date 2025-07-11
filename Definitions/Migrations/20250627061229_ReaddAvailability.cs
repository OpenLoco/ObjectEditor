using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReaddAvailability : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<int>(
				name: "Availability",
				table: "Objects",
				type: "INTEGER",
				nullable: false,
				defaultValue: 0);

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
				name: "Availability",
				table: "Objects");
	}
}
