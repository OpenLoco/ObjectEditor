using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSchema.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.CreateTable(
				name: "Authors",
				columns: table => new
				{
					TblAuthorId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => table.PrimaryKey("PK_Authors", x => x.TblAuthorId));

			_ = migrationBuilder.CreateTable(
				name: "Tags",
				columns: table => new
				{
					TblTagId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => table.PrimaryKey("PK_Tags", x => x.TblTagId));

			_ = migrationBuilder.CreateTable(
				name: "TblModpack",
				columns: table => new
				{
					TblModpackId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table => table.PrimaryKey("PK_TblModpack", x => x.TblModpackId));

			_ = migrationBuilder.CreateTable(
				name: "Objects",
				columns: table => new
				{
					TblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					OriginalName = table.Column<string>(type: "TEXT", nullable: false),
					OriginalChecksum = table.Column<uint>(type: "INTEGER", nullable: false),
					OriginalBytes = table.Column<byte[]>(type: "BLOB", nullable: false),
					SourceGame = table.Column<byte>(type: "INTEGER", nullable: false),
					ObjectType = table.Column<byte>(type: "INTEGER", nullable: false),
					VehicleType = table.Column<byte>(type: "INTEGER", nullable: true),
					Description = table.Column<string>(type: "TEXT", nullable: true),
					AuthorTblAuthorId = table.Column<int>(type: "INTEGER", nullable: true),
					CreationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
					LastEditDate = table.Column<DateTime>(type: "TEXT", nullable: true)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_Objects", x => x.TblLocoObjectId);
					_ = table.ForeignKey(
						name: "FK_Objects_Authors_AuthorTblAuthorId",
						column: x => x.AuthorTblAuthorId,
						principalTable: "Authors",
						principalColumn: "TblAuthorId");
				});

			_ = migrationBuilder.CreateTable(
				name: "ObjectTagLinks",
				columns: table => new
				{
					TblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: false),
					TblTagId = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_ObjectTagLinks", x => new { x.TblLocoObjectId, x.TblTagId });
					_ = table.ForeignKey(
						name: "FK_ObjectTagLinks_Objects_TblLocoObjectId",
						column: x => x.TblLocoObjectId,
						principalTable: "Objects",
						principalColumn: "TblLocoObjectId",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_ObjectTagLinks_Tags_TblTagId",
						column: x => x.TblTagId,
						principalTable: "Tags",
						principalColumn: "TblTagId",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateTable(
				name: "TblModpackTagLink",
				columns: table => new
				{
					TblLocoObjectId = table.Column<int>(type: "INTEGER", nullable: false),
					TblModpackId = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					_ = table.PrimaryKey("PK_TblModpackTagLink", x => new { x.TblLocoObjectId, x.TblModpackId });
					_ = table.ForeignKey(
						name: "FK_TblModpackTagLink_Objects_TblLocoObjectId",
						column: x => x.TblLocoObjectId,
						principalTable: "Objects",
						principalColumn: "TblLocoObjectId",
						onDelete: ReferentialAction.Cascade);
					_ = table.ForeignKey(
						name: "FK_TblModpackTagLink_TblModpack_TblModpackId",
						column: x => x.TblModpackId,
						principalTable: "TblModpack",
						principalColumn: "TblModpackId",
						onDelete: ReferentialAction.Cascade);
				});

			_ = migrationBuilder.CreateIndex(
				name: "IX_Objects_AuthorTblAuthorId",
				table: "Objects",
				column: "AuthorTblAuthorId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_ObjectTagLinks_TblTagId",
				table: "ObjectTagLinks",
				column: "TblTagId");

			_ = migrationBuilder.CreateIndex(
				name: "IX_TblModpackTagLink_TblModpackId",
				table: "TblModpackTagLink",
				column: "TblModpackId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			_ = migrationBuilder.DropTable(
				name: "ObjectTagLinks");

			_ = migrationBuilder.DropTable(
				name: "TblModpackTagLink");

			_ = migrationBuilder.DropTable(
				name: "Tags");

			_ = migrationBuilder.DropTable(
				name: "Objects");

			_ = migrationBuilder.DropTable(
				name: "TblModpack");

			_ = migrationBuilder.DropTable(
				name: "Authors");
		}
	}
}
