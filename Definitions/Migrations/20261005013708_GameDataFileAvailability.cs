using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class GameDataFileAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Availability",
                table: "Tutorials",
                type: "INTEGER",
                nullable: false,
                defaultValue: (int)ObjectAvailability.Available);

            migrationBuilder.AddColumn<int>(
                name: "Availability",
                table: "SoundEffects",
                type: "INTEGER",
                nullable: false,
                defaultValue: (int)ObjectAvailability.Available);

            migrationBuilder.AddColumn<int>(
                name: "Availability",
                table: "Scenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: (int)ObjectAvailability.Available);

            migrationBuilder.AddColumn<int>(
                name: "Availability",
                table: "Music",
                type: "INTEGER",
                nullable: false,
                defaultValue: (int)ObjectAvailability.Available);

            migrationBuilder.AddColumn<int>(
                name: "Availability",
                table: "Graphics",
                type: "INTEGER",
                nullable: false,
                defaultValue: (int)ObjectAvailability.Available);

            // Availability is only meaningful for Custom content (ObjectSource = 0): vanilla Locomotion
            // (Steam/GoG) and OpenLoco content is placed on the server by hand and is never downloadable.
            // Normalise any rows that predate this rule so they are never reported as available.
            migrationBuilder.Sql("UPDATE \"Objects\" SET \"Availability\" = 0 WHERE \"ObjectSource\" <> 0;");
            migrationBuilder.Sql("UPDATE \"Scenarios\" SET \"Availability\" = 0 WHERE \"ObjectSource\" <> 0;");
            migrationBuilder.Sql("UPDATE \"Music\" SET \"Availability\" = 0 WHERE \"ObjectSource\" <> 0;");
            migrationBuilder.Sql("UPDATE \"SoundEffects\" SET \"Availability\" = 0 WHERE \"ObjectSource\" <> 0;");
            migrationBuilder.Sql("UPDATE \"Tutorials\" SET \"Availability\" = 0 WHERE \"ObjectSource\" <> 0;");
            migrationBuilder.Sql("UPDATE \"Graphics\" SET \"Availability\" = 0 WHERE \"ObjectSource\" <> 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Tutorials");

            migrationBuilder.DropColumn(
                name: "Availability",
                table: "SoundEffects");

            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Music");

            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Graphics");
        }
    }
}
