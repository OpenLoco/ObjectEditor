using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoco.Definitions.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Objects");

            migrationBuilder.CreateTable(
                name: "ObjectDatLookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatName = table.Column<string>(type: "TEXT", nullable: false),
                    DatChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
                    xxHash3 = table.Column<long>(type: "INTEGER", nullable: false),
                    ObjectId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectDatLookups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectDatLookups_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectDatLookups_DatName_DatChecksum",
                table: "ObjectDatLookups",
                columns: new[] { "DatName", "DatChecksum" },
                unique: true,
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectDatLookups_ObjectId",
                table: "ObjectDatLookups",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectDatLookups_xxHash3",
                table: "ObjectDatLookups",
                column: "xxHash3",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectDatLookups");

            migrationBuilder.AddColumn<int>(
                name: "Availability",
                table: "Objects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
