using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Database.Migrations;

    /// <inheritdoc />
    public partial class AddObjectLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropColumn(
			name: "Availability",
			table: "Objects");

		_ = migrationBuilder.CreateTable(
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
				_ = table.PrimaryKey("PK_ObjectDatLookups", x => x.Id);
				_ = table.ForeignKey(
					name: "FK_ObjectDatLookups_Objects_ObjectId",
					column: x => x.ObjectId,
					principalTable: "Objects",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjectDatLookups_DatName_DatChecksum",
			table: "ObjectDatLookups",
			columns: new[] { "DatName", "DatChecksum" },
			unique: true,
			descending: new[] { true, false });

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjectDatLookups_ObjectId",
			table: "ObjectDatLookups",
			column: "ObjectId");

		_ = migrationBuilder.CreateIndex(
			name: "IX_ObjectDatLookups_xxHash3",
			table: "ObjectDatLookups",
			column: "xxHash3",
			unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
		_ = migrationBuilder.DropTable(
			name: "ObjectDatLookups");

		_ = migrationBuilder.AddColumn<int>(
			name: "Availability",
			table: "Objects",
			type: "INTEGER",
			nullable: false,
			defaultValue: 0);
        }
    }
