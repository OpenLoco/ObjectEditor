using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSchema.Migrations
{
    /// <inheritdoc />
    public partial class addPathOnDisk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathOnDisk",
                table: "Objects",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_PathOnDisk",
                table: "Objects",
                column: "PathOnDisk",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Objects_PathOnDisk",
                table: "Objects");

            migrationBuilder.DropColumn(
                name: "PathOnDisk",
                table: "Objects");
        }
    }
}
