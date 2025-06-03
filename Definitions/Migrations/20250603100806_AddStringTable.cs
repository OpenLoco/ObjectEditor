using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Definitions.Migrations
{
    /// <inheritdoc />
    public partial class AddStringTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectDatLookups");

            migrationBuilder.DropIndex(
                name: "IX_Objects_DatName_DatChecksum",
                table: "Objects");

            migrationBuilder.DropColumn(
                name: "DatChecksum",
                table: "Objects");

            migrationBuilder.DropColumn(
                name: "DatName",
                table: "Objects");

            migrationBuilder.CreateTable(
                name: "DatObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatName = table.Column<string>(type: "TEXT", nullable: false),
                    DatChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
                    xxHash3 = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ObjectId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatObjects_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblStringTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StringIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    Language = table.Column<byte>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    ObjectId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblStringTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblStringTable_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_Name",
                table: "Authors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatObjects_DatName_DatChecksum",
                table: "DatObjects",
                columns: new[] { "DatName", "DatChecksum" },
                unique: true,
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_DatObjects_ObjectId",
                table: "DatObjects",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DatObjects_xxHash3",
                table: "DatObjects",
                column: "xxHash3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblStringTable_ObjectId",
                table: "TblStringTable",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TblStringTable_Text",
                table: "TblStringTable",
                column: "Text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatObjects");

            migrationBuilder.DropTable(
                name: "TblStringTable");

            migrationBuilder.DropIndex(
                name: "IX_Authors_Name",
                table: "Authors");

            migrationBuilder.AddColumn<uint>(
                name: "DatChecksum",
                table: "Objects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<string>(
                name: "DatName",
                table: "Objects",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ObjectDatLookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    DatChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
                    DatName = table.Column<string>(type: "TEXT", nullable: false),
                    xxHash3 = table.Column<long>(type: "INTEGER", nullable: false)
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
                name: "IX_Objects_DatName_DatChecksum",
                table: "Objects",
                columns: new[] { "DatName", "DatChecksum" },
                unique: true,
                descending: new[] { true, false });

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
    }
}
